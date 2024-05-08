using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerInputHandler : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    public override void _Ready()
    {
        InitializeDiReferences();
        SubscribeToActions();
    }

    #region Initializers

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        
    }
    
    

    #endregion

    public override void _Input(InputEvent inputEvent)
    {
        MouseActivities(inputEvent);
        SwitchEquipables(inputEvent);
    }

    private void MouseActivities(InputEvent inputEvent)
    {
        var totalSlots = 12;
        
        if (inputEvent.IsActionReleased("ui_left_click"))
            MineActions.OnLeftMouseClickAction?.Invoke();
        
        if(inputEvent.IsActionPressed("ui_left_click"))
            MineActions.OnLeftMouseHeldActionStarted?.Invoke();
        
        if(inputEvent.IsActionReleased("ui_left_click"))
            MineActions.OnLeftMouseHeldActionEnded?.Invoke();
            
        if (inputEvent.IsActionReleased("ui_right_click"))
            MineActions.OnRightMouseClickAction?.Invoke();

        if (inputEvent.IsActionReleased("ui_wheel_up"))
        {
            var currentSlot = (_playerControllerVariables.CurrentEquippedItemSlot + 1) % totalSlots;
            _playerControllerVariables.CurrentEquippedItemSlot = currentSlot;
        }

        if (inputEvent.IsActionReleased("ui_wheel_down"))
        {
            var currentSlot = _playerControllerVariables.CurrentEquippedItemSlot - 1;
            var newSlotNumber = currentSlot < 0 ? currentSlot + totalSlots : currentSlot;
            _playerControllerVariables.CurrentEquippedItemSlot = newSlotNumber;
        }
    }

    private void SwitchEquipables(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventKey) return;
        if (inputEvent.IsActionReleased("eq1"))
            _playerControllerVariables.CurrentEquippedItemSlot = 0;
        else if (inputEvent.IsActionReleased("eq2"))
            _playerControllerVariables.CurrentEquippedItemSlot = 1;
        else if (inputEvent.IsActionReleased("eq3"))
            _playerControllerVariables.CurrentEquippedItemSlot = 2;
        else if (inputEvent.IsActionReleased("eq4"))
            _playerControllerVariables.CurrentEquippedItemSlot = 3;
        else if (inputEvent.IsActionReleased("eq5"))
            _playerControllerVariables.CurrentEquippedItemSlot = 4;
        else if (inputEvent.IsActionReleased("eq6"))
            _playerControllerVariables.CurrentEquippedItemSlot = 5;
        else if (inputEvent.IsActionReleased("eq7"))
            _playerControllerVariables.CurrentEquippedItemSlot = 6;
        else if (inputEvent.IsActionReleased("eq8"))
            _playerControllerVariables.CurrentEquippedItemSlot = 7;
        else if (inputEvent.IsActionReleased("eq9"))
            _playerControllerVariables.CurrentEquippedItemSlot = 8;
        else if (inputEvent.IsActionReleased("eq10"))
            _playerControllerVariables.CurrentEquippedItemSlot = 9;
        else if (inputEvent.IsActionReleased("eq11"))
            _playerControllerVariables.CurrentEquippedItemSlot = 10;
        else if (inputEvent.IsActionReleased("eq12"))
            _playerControllerVariables.CurrentEquippedItemSlot = 11;
    }
}