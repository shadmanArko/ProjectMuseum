using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;
using WallPlaceableObject = Godot4CS.ProjectMuseum.Scripts.Mine.Objects.Types.WallPlaceable.WallPlaceableObject;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class WallPlaceableController : Node2D
{
    private HttpRequest _sendWallPlaceableFromInventoryToMineHttpRequest;
    
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    private WallPlaceableObject _wallPlaceableObject;

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
        _sendWallPlaceableFromInventoryToMineHttpRequest.RequestCompleted += OnSendWallPlaceableFromInventoryToMineHttpRequestComplete;
    }

    private void SubscribeToActions()
    {
        MineActions.OnMouseMotionAction += ShowWallPlaceableEligibilityVisualizer;
        MineActions.OnLeftMouseClickActionStarted += PlaceWallPlaceableInMine;
        MineActions.OnRightMouseClickActionEnded += DestroyWallPlaceableAndDeselect;
    }

    private void UnsubscribeToActions()
    {
        MineActions.OnMouseMotionAction -= ShowWallPlaceableEligibilityVisualizer;
        MineActions.OnLeftMouseClickActionStarted -= PlaceWallPlaceableInMine;
        MineActions.OnRightMouseClickActionEnded -= DestroyWallPlaceableAndDeselect;
    }
    
    public override void _Ready()
    {
        InitializeDiInstaller();
    }

    #endregion

    #region Select and Deselect

    public void OnSelectWallPlaceableFromInventory(string scenePath)
    {
        _wallPlaceableObject = InstantiateWallPlaceable(scenePath, Vector2I.Zero);
        SubscribeToActions();
    }

    private void OnDeselectedWallPlaceableFromInventory()
    {
        UnsubscribeToActions();
        _wallPlaceableObject = null;
    }

    private void DestroyWallPlaceableAndDeselect()
    {
        UnsubscribeToActions();
        _wallPlaceableObject.QueueFree();
    }

    #endregion

    #region Remove Wall Placeable from Mine

    private void SendWallPlaceableFromInventoryToMine(InventoryItem inventoryItem)
    {
        var url = ApiAddress.MineApiPath+"SendWallPlaceableFromInventoryToMine/"+inventoryItem;
        _sendWallPlaceableFromInventoryToMineHttpRequest.Request(url);
    }
    
    private void OnSendWallPlaceableFromInventoryToMineHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var wallPlaceable = JsonSerializer.Deserialize<WallPlaceable>(jsonStr);
        //todo: convert wall placeable into a wall placeable object;
    }

    #endregion

    private void ShowWallPlaceableEligibilityVisualizer(double value)
    {
        var eligibility = CheckEligibility();
        if (eligibility)
            _wallPlaceableObject.SetSpriteColorToGreen();
        else
            _wallPlaceableObject.SetSpriteColorToRed();
        
        var cell = GetTargetCell();
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize / 2f, cellSize / 4f);
        _wallPlaceableObject.Position = new Vector2(cell.PositionX, cell.PositionY) * cellSize + offset;
    }
    private void PlaceWallPlaceableInMine()
    {
        var checkEligibility = CheckEligibility();
        if (checkEligibility)
        {
            var cellSize = _mineGenerationVariables.Mine.CellSize;
            var cell = GetTargetCell();
            var cellPos = new Vector2(cellSize * cell.PositionX, cellSize * cell.PositionY);
            var offset = new Vector2(cellSize /2f, cellSize/4f);
            _wallPlaceableObject.Position = cellPos + offset;
            _wallPlaceableObject.SetSpriteColorToDefault();
            OnDeselectedWallPlaceableFromInventory();
        }
    }
    
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
    
    private WallPlaceableObject InstantiateWallPlaceable(string scenePath, Vector2 pos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var cellSize = mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize /2f, cellSize/4f);
        var wallPlaceableObject = SceneInstantiator.InstantiateScene(scenePath, mineGenerationVariables.MineGenView, pos + offset) as WallPlaceableObject;
        return wallPlaceableObject;
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