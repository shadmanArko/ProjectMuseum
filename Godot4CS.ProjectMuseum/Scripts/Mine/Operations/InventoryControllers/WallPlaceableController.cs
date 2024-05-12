using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations.InventoryControllers;

public partial class WallPlaceableController : InventoryController
{
    private HttpRequest _getWallPlaceablesHttpRequest;

    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    private Inventory _inventory;
    private WallPlaceableDTO _wallPlaceableDto;

    [Export] private Sprite2D _wallPlaceableSprite;
    private InventoryItem _inventoryItem;

    #region Initializers

    private void InitializeDiInstaller()
    {
        CreateHttpRequests();
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _inventory = ServiceRegistry.Resolve<Inventory>();
        _wallPlaceableDto = ServiceRegistry.Resolve<WallPlaceableDTO>();
    }

    private void CreateHttpRequests()
    {
        _getWallPlaceablesHttpRequest = new HttpRequest();
        AddChild(_getWallPlaceablesHttpRequest);
        _getWallPlaceablesHttpRequest.RequestCompleted +=
            OnGetWallPlaceablesHttpRequestComplete;
    }

    private void SubscribeToActions()
    {
        MineActions.OnMouseMotionAction += ShowWallPlaceableEligibilityVisualizer;
        MineActions.OnLeftMouseClickAction += PlaceWallPlaceableInMine;
        MineActions.DeselectAllInventoryControllers += DeactivateController;
    }

    private void UnsubscribeToActions()
    {
        MineActions.OnMouseMotionAction -= ShowWallPlaceableEligibilityVisualizer;
        MineActions.OnLeftMouseClickAction -= PlaceWallPlaceableInMine;
        MineActions.DeselectAllInventoryControllers -= DeactivateController;
    }

    public override void _Ready()
    {
        InitializeDiInstaller();
        GetAllWallPlaceables();
    }

    #endregion

    #region Select and Deselect

    #region Activate and Deactivate

    public override void ActivateController(InventoryItem inventoryItem)
    {
        IsControllerActivated = true;
        _inventoryItem = inventoryItem;
        
        var wallPlaceableTexture = ResourceLoader.Load<Texture2D>(inventoryItem.PngPath);
        if (wallPlaceableTexture != null)
        {
            _wallPlaceableSprite.Visible = true;
            _wallPlaceableSprite.Texture = wallPlaceableTexture;
        }
        
        SubscribeToActions();
        GD.Print("Wall placeable controller activated");
    }

    public override void DeactivateController()
    {
        if(!IsControllerActivated) return;
        IsControllerActivated = false;
        _wallPlaceableSprite.Visible = false;
        UnsubscribeToActions();
        GD.Print("Wall placeable controller deactivated");
    }

    #endregion

    // private void OnDeselectedWallPlaceableFromInventory()
    // {
    //     _wallPlaceableSprite.Visible = false;
    //     _wallPlaceableSprite.Position = new Vector2(0, 0);
    //     UnsubscribeToActions();
    // }

    // private void DestroyWallPlaceableAndDeselect()
    // {
    //     _wallPlaceableSprite.Visible = false;
    //     UnsubscribeToActions();
    // }

    #endregion

    #region From inventory to mine

    private void GetAllWallPlaceables()
    {
        var url = ApiAddress.MineApiPath + "GetAllWallPlaceables";
        _getWallPlaceablesHttpRequest.CancelRequest();
        _getWallPlaceablesHttpRequest.Request(url);
    }
    
    private void OnGetWallPlaceablesHttpRequestComplete(long result, long responseCode,
        string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        GD.PrintErr("wall placeable list"+jsonStr);
        var wallPlaceables = JsonSerializer.Deserialize<List<WallPlaceable>>(jsonStr);
        
        if (wallPlaceables == null)
        {
            GD.PrintErr("Wall placeables is null in wall placeable DTO");
            return;
        }
        _wallPlaceableDto.WallPlaceables = wallPlaceables;
    }

    #endregion

    #region Set Wall Placeable to Mine from Inventory

    private void SetWallPlaceableFromInventoryToMine()
    {
        if (_inventoryItem.IsStackable)
        {
            if (_inventoryItem.Stack > 1) 
                _inventoryItem.Stack--;
            else 
                _inventory.InventoryItems.Remove(_inventoryItem);
        }

        var wallPlaceable = _wallPlaceableDto.WallPlaceables.FirstOrDefault(temp => temp.Variant == _inventoryItem.Variant);
        if (wallPlaceable == null)
        {
            GD.PrintErr("Wall Placeable not found in wall placeable DTO");
            return;
        }
        
        var cell = GetTargetCell();
        cell.HasWallPlaceable = true;
        
        wallPlaceable.Id = Guid.NewGuid().ToString();
        wallPlaceable.OccupiedCellIds = new List<string> { cell.Id };
        wallPlaceable.PositionX = cell.PositionX;
        wallPlaceable.PositionY = cell.PositionY;
        _mineGenerationVariables.Mine.WallPlaceables.Add(wallPlaceable);
        
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cellPos = new Vector2(cellSize * cell.PositionX, cellSize * cell.PositionY);
        InstantiateWallPlaceable(wallPlaceable.ScenePath, cellPos);
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #endregion

    private void ShowWallPlaceableEligibilityVisualizer(double value)
    {
        var eligibility = CheckEligibility();
        
        var cell = GetTargetCell();
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize / 2f, cellSize / 4f);
        _wallPlaceableSprite.Position = new Vector2(cell.PositionX, cell.PositionY) * cellSize + offset;
        
        if (eligibility)
            SetSpriteColorToGreen();
        else
            SetSpriteColorToRed();
    }

    private void PlaceWallPlaceableInMine()
    {
        var checkEligibility = CheckEligibility();
        if (checkEligibility)
        {
            GD.Print($"inventory item: {_inventoryItem.Variant}, slot:{_inventoryItem.Slot}, stack: {_inventoryItem.Stack}");
            SetWallPlaceableFromInventoryToMine();
        }
    }

    #region Set Sprite Color

    private void SetSpriteColorToGreen()
    {
        _wallPlaceableSprite.Modulate = Colors.Green;
    }

    private void SetSpriteColorToRed()
    {
        _wallPlaceableSprite.Modulate = Colors.Red;
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

    private void InstantiateWallPlaceable(string scenePath, Vector2 pos)
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

        if (cell.HasWallPlaceable)
        {
            GD.Print("Cell Already has a wall placeable");
            return false;
        }

        GD.Print("Torch can be placed");
        return true;
    }

    #endregion
}