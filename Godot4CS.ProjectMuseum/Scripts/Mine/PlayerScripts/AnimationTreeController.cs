using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class AnimationTreeController : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	
	
	private AnimationNodeStateMachinePlayback StateMachine;
	[Export] private AnimationTree _animationTree;

	private const string MoveCondition = "parameters/conditions/is_moving";
	private const string HangingCondition = "parameters/conditions/is_hanging";
	
	
	private const string MoveVelocity = "parameters/move/blend_position";
	private const string HangingVelocity = "parameters/climb/blend_position";


	public override void _EnterTree()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	public override void _Ready()
	{
		//_animationTree = GetNode<AnimationTree>("AnimationTree");
		StateMachine = _animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
	}
    
	public override void _Process(double delta)
	{
		SetAnimation();
	}

	public void SetAnimation()
	{
		if(_playerControllerVariables.State == MotionState.Hanging)
			PlayHangingAnimation();
		else
			PlayMovementAnimation();
	}

	private void PlayHangingAnimation()
	{
		var velocity = _playerControllerVariables.Velocity;
		GD.Print("Animation tree hanging being called");
		
		_animationTree.Set(MoveCondition, false);
		_animationTree.Set(HangingCondition, true);
		_animationTree.Set(HangingVelocity, velocity);
		StateMachine.Travel("hang");
	}

	private void PlayMovementAnimation()
	{
		var velocity = _playerControllerVariables.Velocity.Normalized();
		Set(HangingCondition, false);
		Set(MoveCondition, true);
		Set(MoveVelocity, velocity);
		StateMachine.Travel("move");
	}
}