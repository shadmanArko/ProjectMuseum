using System.Collections.Generic;
using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class WallPlaceableController : Node2D
{
    private HttpRequest _sendWallPlaceableFromInventoryToMineHttpRequest;

    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    [Export] private Sprite2D _wallPlaceableSprite;
    private InventoryItem _inventoryItem;

    #region Initializers

    private void InitializeDiInstaller()
    {
        CreateHttpRequests();
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void CreateHttpRequests()
    {
        _sendWallPlaceableFromInventoryToMineHttpRequest = new HttpRequest();
        AddChild(_sendWallPlaceableFromInventoryToMineHttpRequest);
        _sendWallPlaceableFromInventoryToMineHttpRequest.RequestCompleted +=
            OnSendWallPlaceableFromInventoryToMineHttpRequestComplete;
    }

    private void SubscribeToActions()
    {
        MineActions.OnMouseMotionAction += ShowWallPlaceableEligibilityVisualizer;
        MineActions.OnLeftMouseClickActionEnded += PlaceWallPlaceableInMine;
        MineActions.OnRightMouseClickActionEnded += DestroyWallPlaceableAndDeselect;
    }

    private void UnsubscribeToActions()
    {
        MineActions.OnMouseMotionAction -= ShowWallPlaceableEligibilityVisualizer;
        MineActions.OnLeftMouseClickActionEnded -= PlaceWallPlaceableInMine;
        MineActions.OnRightMouseClickActionEnded -= DestroyWallPlaceableAndDeselect;
    }

    public override void _Ready()
    {
        InitializeDiInstaller();
    }

    #endregion

    #region Select and Deselect

    public void OnSelectWallPlaceableFromInventory(InventoryItem inventoryItem)
    {
        _inventoryItem = inventoryItem;
        
        var wallPlaceableTexture = ResourceLoader.Load<Texture2D>(inventoryItem.PngPath);
        if (wallPlaceableTexture != null)
        {
            _wallPlaceableSprite.Visible = true;
            _wallPlaceableSprite.Texture = wallPlaceableTexture;
        }
        
        SubscribeToActions();
    }

    private void OnDeselectedWallPlaceableFromInventory()
    {
        _wallPlaceableSprite.Visible = false;
        _wallPlaceableSprite.Position = new Vector2(0, 0);
        UnsubscribeToActions();
    }

    private void DestroyWallPlaceableAndDeselect()
    {
        _wallPlaceableSprite.Visible = false;
        UnsubscribeToActions();
    }

    #endregion

    #region Remove Wall Placeable from Mine

    private void SendWallPlaceableFromInventoryToMine(InventoryItem inventoryItem)
    {
        string[] headers = { "Content-Type: application/json"};
        var targetCell = GetTargetCell();
        GD.Print($"target cell id: {targetCell.Id}, posX:{targetCell.PositionX}, posY:{targetCell.PositionY}");
        GD.Print($"inventory item: {inventoryItem.Variant}, stack:{inventoryItem.Stack}, slot:{inventoryItem.Slot}, id:{inventoryItem.Id}");
        var list = new List<string> {targetCell.Id};
        var body = JsonConvert.SerializeObject(list);
        
        var url = ApiAddress.PlayerApiPath + "SendWallPlaceableFromInventoryToMine/" + inventoryItem.Id;
        _sendWallPlaceableFromInventoryToMineHttpRequest.CancelRequest();
        _sendWallPlaceableFromInventoryToMineHttpRequest.Request(url, headers, HttpClient.Method.Post, body);
    }

    private void OnSendWallPlaceableFromInventoryToMineHttpRequestComplete(long result, long responseCode,
        string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var wallPlaceable = JsonSerializer.Deserialize<WallPlaceable>(jsonStr);
        GD.Print($"Wall placeable from inventory to mine completed. wallPlaceable: {wallPlaceable.Id} {wallPlaceable.Variant}");
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var cell = GetTargetCell();
        var cellPos = new Vector2(cellSize * cell.PositionX, cellSize * cell.PositionY);
        InstantiateWallPlaceable(wallPlaceable.ScenePath, cellPos);
        OnDeselectedWallPlaceableFromInventory();
        MineActions.OnInventoryUpdate?.Invoke();
        cell.HasWallPlaceable = true;
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
            SendWallPlaceableFromInventoryToMine(_inventoryItem);
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