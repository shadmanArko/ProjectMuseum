using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;
using WallPlaceableObject = Godot4CS.ProjectMuseum.Scripts.Mine.Objects.Types.WallPlaceable.WallPlaceableObject;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class WallPlaceableController : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    private WallPlaceableObject _wallPlaceable;

    #region Initializers

    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
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
        _wallPlaceable = InstantiateTorch(scenePath, Vector2I.Zero);
        SubscribeToActions();
    }

    private void OnDeselectedWallPlaceableFromInventory()
    {
        UnsubscribeToActions();
        _wallPlaceable = null;
    }

    private void DestroyWallPlaceableAndDeselect()
    {
        UnsubscribeToActions();
        _wallPlaceable.QueueFree();
    }

    #endregion

    private void ShowWallPlaceableEligibilityVisualizer(double value)
    {
        var eligibility = CheckEligibility();
        if (eligibility)
            _wallPlaceable.SetSpriteColorToGreen();
        else
            _wallPlaceable.SetSpriteColorToRed();
        
        var cell = GetTargetCell();
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize / 2f, cellSize / 4f);
        _wallPlaceable.Position = new Vector2(cell.PositionX, cell.PositionY) * cellSize + offset;
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
            _wallPlaceable.Position = cellPos + offset;
            _wallPlaceable.SetSpriteColorToDefault();
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
    
    private WallPlaceableObject InstantiateTorch(string scenePath, Vector2 pos)
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