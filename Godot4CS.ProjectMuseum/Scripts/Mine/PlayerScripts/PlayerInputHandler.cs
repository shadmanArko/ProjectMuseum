using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerInputHandler : Node2D
{
	private PlayerControllerVariables _playerControllerVariables;
	public override void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
	}

	#region Initializers

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	private void SubscribeToActions()
	{
		
	}

	#endregion

	public override void _Input(InputEvent inputEvent)
	{
		MouseMotion(inputEvent);
		SwitchEquipables(inputEvent);
	}

	private void SwitchEquipables(InputEvent inputEvent)
	{
		if(inputEvent is not InputEventKey) return;
		if (inputEvent.IsActionReleased("Equipment1"))
			_playerControllerVariables.CurrentEquippedItem = Equipables.Sword;
		else if(inputEvent.IsActionReleased("Equipment2"))
			_playerControllerVariables.CurrentEquippedItem = Equipables.PickAxe;
		else if(inputEvent.IsActionReleased("Equipment3"))
			_playerControllerVariables.CurrentEquippedItem = Equipables.Brush;
	}

	private void MouseMotion(InputEvent @event)
	{
		if(@event is not InputEventMouseMotion) return;
		var mousePos = GetGlobalMousePosition();
		var angle = GetAngleTo(mousePos);
		var degree = angle * (180 / Math.PI);
		
		_playerControllerVariables.MouseDirection = degree switch
		{
			<= 45 and > -45 => Vector2I.Right,
			<= -45 and > -135 => Vector2I.Up,
			> 45 and <= 135 => Vector2I.Down,
			_ => Vector2I.Left
		};
		
		MineActions.OnMouseMotionAction?.Invoke(degree);
	}
}

//TODO: Move all player inputs under this script
// private void PlayerAttack(InputEvent inputEvent)
// {
// 	if(inputEvent is not InputEventKey) return;
// 	// var grab = inputEvent.IsActionReleased("toggle_grab");
// 	var input = inputEvent.IsActionReleased("ui_left_click");
// 	_playerControllerVariables.IsAttacking = input;
// 	if (input) MineActions.OnPlayerAttackActionPressed?.Invoke();
// }
//
// private void PlayerGrab(InputEvent @event)
// {
// 	if(@event is not InputEventKey) return;
// 	var grab = @event.IsActionReleased("toggle_grab");
// 	if (!grab) return;
// 	_playerControllerVariables.IsHanging = !_playerControllerVariables.IsHanging;
// 	_playerControllerVariables.Acceleration = _playerControllerVariables.IsHanging ? _playerControllerVariables.MaxSpeed / 2 : _playerControllerVariables.MaxSpeed;
// }