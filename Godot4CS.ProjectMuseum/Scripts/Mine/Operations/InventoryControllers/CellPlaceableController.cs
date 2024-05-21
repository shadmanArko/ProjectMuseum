using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
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
        CreateHttpRequests();
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
        GetAllCellPlaceables();
    }
    
    private void CreateHttpRequests()
    {
        _getCellPlaceablesHttpRequest = new HttpRequest();
        AddChild(_getCellPlaceablesHttpRequest);
        _getCellPlaceablesHttpRequest.RequestCompleted +=
            OnGetCellPlaceablesHttpRequestComplete;
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

    #region Populate Cell Placeable DTO

    private void GetAllCellPlaceables()
    {
        var url = ApiAddress.MineApiPath + "GetAllCellPlaceables";
        _getCellPlaceablesHttpRequest.CancelRequest();
        _getCellPlaceablesHttpRequest.Request(url);
    }
    
    private void OnGetCellPlaceablesHttpRequestComplete(long result, long responseCode,
        string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var cellPlaceables = JsonSerializer.Deserialize<List<CellPlaceable>>(jsonStr);
        
        if (cellPlaceables == null)
        {
            GD.PrintErr("Wall placeables is null in wall placeable DTO");
            return;
        }
        _cellPlaceableDto.CellPlaceables = cellPlaceables;
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
        
        cellPlaceable.Id = Guid.NewGuid().ToString();
        cellPlaceable.OccupiedCellId = cell.Id!;
        cellPlaceable.PositionX = cell.PositionX;
        cellPlaceable.PositionY = cell.PositionY;
        _mineGenerationVariables.Mine.CellPlaceables.Add(cellPlaceable);
        
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellOffset = new Vector2(cellSize, cellSize) / 2;
        var cellPos = new Vector2(cell.PositionX, cell.PositionY) * cellSize;
        InstantiateCellPlaceable(cellPlaceable.ScenePath, cellPos);
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #endregion

    
    #region Set Sprite

    private void SetSpritePosition()
    {
        var cell = GetTargetCell();
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize / 2f, cellSize / 4f);
        _cellPlaceableSprite.Position = new Vector2(cell.PositionX, cell.PositionY) * cellSize + offset;
    }

    private void SetSpriteColorToGreen()
    {
        _cellPlaceableSprite.Modulate = Colors.Green;
    }

    private void SetSpriteColorToRed()
    {
        _cellPlaceableSprite.Modulate = Colors.Red;
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
        var offset = new Vector2(cellSize / 2f, cellSize / 4f);
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

        if (cell.HasCellPlaceable)
        {
            GD.Print("Cell Already has a Cell placeable");
            return false;
        }

        GD.Print("Cell Placeable can be placed");
        return true;
    }

    #endregion


}