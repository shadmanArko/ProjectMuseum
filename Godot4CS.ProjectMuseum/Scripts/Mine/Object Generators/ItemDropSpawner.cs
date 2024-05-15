using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Object_Generators;

public partial class ItemDropSpawner : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    private InventoryItem _inventoryItem;

    public override void _Ready()
    {
        InitializeDiInstaller();
    }

    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        // MineActions.OnSuccessfulDigActionCompleted += 
    }

    private void InstantiateItemDrop()
    {
        var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
        tilePos += _playerControllerVariables.MouseDirection;
        var cell = _mineGenerationVariables.GetCell(tilePos);
    }
}