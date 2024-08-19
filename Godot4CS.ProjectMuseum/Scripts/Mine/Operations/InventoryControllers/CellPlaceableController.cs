using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

using Godot4CS.ProjectMuseum.Scripts.Mine.ColorControllers;

using Godot4CS.ProjectMuseum.Scripts.Mine.ItemCollectionLogSystem;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations.InventoryControllers;

public partial class CellPlaceableController : InventoryController
{
    private HttpRequest _getCellPlaceablesHttpRequest;

    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    private InventoryDTO _inventoryDto;
    private CellPlaceableDTO _cellPlaceableDto;

    [Export] private Sprite2D _cellPlaceableSprite;
    private InventoryItem _inventoryItem;
    
    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
        _cellPlaceableDto = ServiceRegistry.Resolve<CellPlaceableDTO>();
    }

    #region Subscribe Unsubscribe To Actions

    private void SubscribeToActions()
    {
        MineActions.OnMouseMotionAction += ShowCellPlaceableEligibilityVisualizer;
        MineActions.OnLeftMouseClickAction += PlaceCellPlaceableInMine;
        MineActions.DeselectAllInventoryControllers += DeactivateController;
    }

    private void UnsubscribeToActions()
    {
        MineActions.OnMouseMotionAction -= ShowCellPlaceableEligibilityVisualizer;
        MineActions.OnLeftMouseClickAction -= PlaceCellPlaceableInMine;
        MineActions.DeselectAllInventoryControllers -= DeactivateController;
    }

    #endregion
    
    public override void _Ready()
    {
        InitializeDiInstaller();
    }
    
    #region Activate Deactivate Controller

    public override void ActivateController(InventoryItem inventoryItem)
    {
        IsControllerActivated = true;
        _inventoryItem = inventoryItem;
        
        var cellPlaceableTexture = ResourceLoader.Load<Texture2D>(inventoryItem.PngPath);
        if (cellPlaceableTexture != null)
        {
            _cellPlaceableSprite.Visible = true;
            _cellPlaceableSprite.Texture = cellPlaceableTexture;
            SetSpritePosition();
        }
        
        SubscribeToActions();
        GD.Print("Cell placeable controller activated");
    }

    public override void DeactivateController()
    {
        if(!IsControllerActivated) return;
        IsControllerActivated = false;
        _cellPlaceableSprite.Visible = false;
        UnsubscribeToActions();
        GD.Print("Wall placeable controller deactivated");
    }

    #endregion
    
    private void ShowCellPlaceableEligibilityVisualizer(double value)
    {
        var eligibility = CheckEligibility();
        SetSpritePosition();
        
        if (eligibility)
            SetSpriteColorToGreen();
        else
            SetSpriteColorToRed();
    }
    
    private void PlaceCellPlaceableInMine()
    {
        var checkEligibility = CheckEligibility();
        if (checkEligibility)
        {
            GD.Print($"inventory item: {_inventoryItem.Variant}, slot:{_inventoryItem.Slot}, stack: {_inventoryItem.Stack}");
            SetCellPlaceableFromInventoryToMine();
        }
    }
    
    #region Set Cell Placeable to Mine from Inventory

    private void SetCellPlaceableFromInventoryToMine()
    {
        if (_inventoryItem.IsStackable)
        {
            if (_inventoryItem.Stack > 1) 
                _inventoryItem.Stack--;
            else 
                _inventoryDto.Inventory.InventoryItems.Remove(_inventoryItem);
        }
        
        var cellPlaceable = _cellPlaceableDto.CellPlaceables.FirstOrDefault(temp => temp.Variant == _inventoryItem.Variant);
        if (cellPlaceable == null)
        {
            GD.PrintErr("Cell Placeable not found in Cell Placeable DTO");
            return;
        }
        
        var cell = GetTargetCell();
        cell.HasCellPlaceable = true;

        var newCellPlaceable = new CellPlaceable
        {
            Id = Guid.NewGuid().ToString(),
            OccupiedCellId = cell.Id!,
            PositionX = cell.PositionX,
            PositionY = cell.PositionY,
            Name = cellPlaceable.Name,
            Type = cellPlaceable.Type,
            Category = cellPlaceable.Category,
            Variant = cellPlaceable.Variant,
            ScenePath = cellPlaceable.ScenePath,
            PngPath = cellPlaceable.PngPath
        };
        _mineGenerationVariables.Mine.CellPlaceables.Add(newCellPlaceable);
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        
        var cellPos = new Vector2(cell.PositionX, cell.PositionY) * cellSize;
        InstantiateCellPlaceable(cellPlaceable.ScenePath, cellPos);
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #endregion

    
    #region Set Sprite

    private void SetSpritePosition()
    {
        var cell = GetTargetCell();
        if (cell == null)
        {
            _cellPlaceableSprite.Visible = false;
            return;
        }
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize / 2f, cellSize / 2f);
        _cellPlaceableSprite.Visible = true;
        _cellPlaceableSprite.Position = new Vector2(cell.PositionX, cell.PositionY) * cellSize + offset;
    }

    private void SetSpriteColorToGreen()
    {
        ColorBlendController.SetColorToGreen(_cellPlaceableSprite);
    }

    private void SetSpriteColorToRed()
    {
        ColorBlendController.SetColorToRed(_cellPlaceableSprite);
    }

    #endregion

    #region Utilities

    private Cell GetTargetCell()
    {
        var mouseDirection = _playerControllerVariables.MouseDirection;
        var cellPos =
            _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
        cellPos += mouseDirection;
        var cell = _mineGenerationVariables.GetCell(cellPos);
        return cell;
    }

    private void InstantiateCellPlaceable(string scenePath, Vector2 pos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var cellSize = mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize / 2f, cellSize / 2f);
        SceneInstantiator.InstantiateScene(scenePath, mineGenerationVariables.MineGenView, pos + offset);
    }

    private bool CheckEligibility()
    {
        var cell = GetTargetCell();
        if (cell is null) return false;
        if (!cell.IsBreakable || !cell.IsBroken || !cell.IsRevealed)
        {
            GD.Print($"Cell eligibility is false");
            return false;
        }

        if (cell.HasCellPlaceable || cell.HasWallPlaceable)
        {
            // GD.Print("Cell Already has a Cell placeable");
            return false;
        }

        if (_inventoryItem.Variant == "DownDrill")
        {
            var bottomCell =
                _mineGenerationVariables.GetCell(new Vector2I(cell.PositionX, cell.PositionY) + Vector2I.Down);
            if (bottomCell == null)
                return false;
            if (!bottomCell.IsBroken || !bottomCell.IsInstantiated || !bottomCell.IsBreakable || !bottomCell.IsRevealed)
            {
                ReferenceStorage.Instance.LogMessageController.ShowLogMessage("Needs to have a broken cell in the direction of operation.");
                return false;
            }
        }

        // GD.Print("Cell Placeable can be placed");
        return true;
    }

    #endregion


}