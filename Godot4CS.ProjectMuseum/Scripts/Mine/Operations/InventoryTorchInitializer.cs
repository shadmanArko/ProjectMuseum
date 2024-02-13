using System.Collections.Generic;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public class InventoryTorchInitializer 
{

    private void SubscribeToActions()
    {
        
    }

    private void InitializeDiInstaller()
    {
        
    }
    
    

    public void SetUpTorchInMine(string scenePath)
    {
        var checkEligibility = CheckEligibility();
        if (checkEligibility)
        {
            var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
            var cellSize = mineGenerationVariables.Mine.CellSize;
            var cell = GetTargetCell();
            var cellPos = new Vector2(cellSize * cell.PositionX, cellSize * cell.PositionY);
            InstantiateTorch(scenePath, cellPos);
        }
    }
    
    public void InstantiateTorch(string scenePath, Vector2 pos)
    {
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var offset = new Vector2(); //todo: add offset to cell position
        SceneInstantiator.InstantiateScene(scenePath, mineGenerationVariables.MineGenView, pos);
    }

    public bool CheckEligibility()
    {
        var cell = GetTargetCell();
        if (cell.HasWallPlaceable)
        {
            GD.Print("Cell Already has a wall placeable");
            return false;
        }
        
        GD.Print("Torch can be placed");
        return true;
    }

    private Cell GetTargetCell()
    {
        var playerControllerVariables = ReferenceStorage.Instance.PlayerControllerVariables;
        var mineGenerationVariables = ReferenceStorage.Instance.MineGenerationVariables;
        var mouseDirection = playerControllerVariables.MouseDirection;
        var cellPos =
            mineGenerationVariables.MineGenView.LocalToMap(playerControllerVariables.Position);
        cellPos += mouseDirection;
        var cell = mineGenerationVariables.GetCell(cellPos);
        return cell;
    }

    public void PlaceTorchInMine()
    {
        
    }

    public void RemoveTorchFromMine()
    {
        
    }
}