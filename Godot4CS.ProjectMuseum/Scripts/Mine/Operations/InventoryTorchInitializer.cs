using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class InventoryTorchInitializer : Node2D
{
    // private PlayerControllerVariables _playerControllerVariables;
    // private MineGenerationVariables _mineGenerationVariables;
    //
    // private void InitializeDiInstaller()
    // {
    //     _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    //     _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    // }
    //
    // private void SubscribeToActions()
    // {
    //     MineActions.OnMouseMotionAction +=
    //         MineActions.
    // }
    //
    // private void UnsubscribeToActions()
    // {
    //     MineActions.OnMouseMotionAction -= 
    // }
    //
    // public override void _Ready()
    // {
    //     InitializeDiInstaller();
    //     SubscribeToActions();
    // }
    //
    // public void OnSelectWallPlaceableFromInventory(string scenePath)
    // {
    //     SubscribeToActions();
    // }
    //
    // public void OnDeselectedWallPlaceableFromInventory()
    // {
    //     UnsubscribeToActions();
    // }
    //
    // private void PlaceWallPlaceableInMine(string scenePath)
    // {
    //     var checkEligibility = CheckEligibility();
    //     if (checkEligibility)
    //     {
    //         var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
    //         var cellSize = mineGenerationVariables.Mine.CellSize;
    //         var cell = GetTargetCell();
    //         var cellPos = new Vector2(cellSize * cell.PositionX, cellSize * cell.PositionY);
    //         InstantiateTorch(scenePath, cellPos);
    //     }
    // }
    //
    //
    //
    // #region Utilities
    //
    // private Cell GetTargetCell()
    // {
    //     var playerControllerVariables = ReferenceStorage.Instance.PlayerControllerVariables;
    //     var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
    //     var mouseDirection = playerControllerVariables.MouseDirection;
    //     var cellPos =
    //         mineGenerationVariables.MineGenView.LocalToMap(playerControllerVariables.Position);
    //     cellPos += mouseDirection;
    //     var cell = mineGenerationVariables.GetCell(cellPos);
    //     return cell;
    // }
    //
    // private void InstantiateTorch(string scenePath, Vector2 pos)
    // {
    //     var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
    //     var offset = new Vector2(); //todo: add offset to cell position
    //     SceneInstantiator.InstantiateScene(scenePath, mineGenerationVariables.MineGenView, pos);
    // }
    //
    // private bool CheckEligibility()
    // {
    //     var cell = GetTargetCell();
    //     if (!cell.IsBreakable || !cell.IsBroken || !cell.IsRevealed)
    //     {
    //         GD.Print($"Cell eligibility is false");
    //         return false;
    //     }
    //     
    //     if (cell.HasWallPlaceable)
    //     {
    //         GD.Print("Cell Already has a wall placeable");
    //         return false;
    //     }
    //     
    //     GD.Print("Torch can be placed");
    //     return true;
    // }
    //
    // #endregion
    //
    // public void PlaceTorchInMine()
    // {
    //     
    // }
    //
    // public void RemoveTorchFromMine()
    // {
    //     
    // }
}