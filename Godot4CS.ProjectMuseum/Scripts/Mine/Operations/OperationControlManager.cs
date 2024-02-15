using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class OperationControlManager : Node2D
{
    [Export] private WallPlaceableController _wallPlaceableController;
    
    private const string TorchScenePath = "res://Scenes/Mine/Sub Scenes/Props/FireTorch.tscn";
    
    public override void _Input(InputEvent inputEvent)
    {
        if(inputEvent.IsActionReleased("Lamp"))
            _wallPlaceableController.OnSelectWallPlaceableFromInventory(TorchScenePath);
        if(inputEvent.IsActionReleased("ui_right_click"))
            MineActions.OnRightMouseClickActionEnded?.Invoke();
    }
}