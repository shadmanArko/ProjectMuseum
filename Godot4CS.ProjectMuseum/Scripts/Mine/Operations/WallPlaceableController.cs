using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Items;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class WallPlaceableController : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    private FireTorch _fireTorch;
    
    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }
    
    private void SubscribeToActions()
    {
        MineActions.OnMouseMotionAction += ShowTorchEligibilityVisualizer;
        MineActions.OnLeftMouseClickActionStarted += PlaceWallPlaceableInMine;
        MineActions.OnRightMouseClickActionEnded += DestroyWallPlaceableAndDeselect;
    }

    private void UnsubscribeToActions()
    {
        MineActions.OnMouseMotionAction -= ShowTorchEligibilityVisualizer;
        MineActions.OnLeftMouseClickActionStarted -= PlaceWallPlaceableInMine;
        MineActions.OnRightMouseClickActionEnded -= DestroyWallPlaceableAndDeselect;
    }
    
    public override void _Ready()
    {
        InitializeDiInstaller();
    }
    
    public void OnSelectWallPlaceableFromInventory(string scenePath)
    {
        SubscribeToActions();
        _fireTorch = InstantiateTorch(scenePath, Vector2I.Zero);
    }

    private void OnDeselectedWallPlaceableFromInventory()
    {
        UnsubscribeToActions();
        _fireTorch = null;
    }

    private void DestroyWallPlaceableAndDeselect()
    {
        UnsubscribeToActions();
        _fireTorch.QueueFree();
    }

    private void ShowTorchEligibilityVisualizer(double value)
    {
        var eligibility = CheckEligibility();

        if (eligibility)
            _fireTorch.SetSpriteColorToGreen();
        else
            _fireTorch.SetSpriteColorToRed();
        
        var cell = GetTargetCell();
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize / 2f, cellSize / 4f);
        _fireTorch.Position = new Vector2(cell.PositionX, cell.PositionY) * cellSize + offset;
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
            _fireTorch.Position = cellPos + offset;
            _fireTorch.SetSpriteColorToDefault();
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
    
    private FireTorch InstantiateTorch(string scenePath, Vector2 pos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var cellSize = mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize /2f, cellSize/4f);
        var torch = SceneInstantiator.InstantiateScene(scenePath, mineGenerationVariables.MineGenView, pos + offset) as FireTorch;
        return torch;
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