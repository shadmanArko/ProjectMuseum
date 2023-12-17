using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class AnimationTreeController : AnimationTree
{
    private PlayerControllerVariables _playerControllerVariables;
	
	
    private AnimationNodeStateMachinePlayback _stateMachine;
    //[Export] private AnimationTree _animationTree;

    private const string MoveCondition = "parameters/conditions/is_moving";
    private const string HangCondition = "parameters/conditions/is_hanging";
    private const string AttackCondition = "parameters/conditions/is_attacking";
    private const string BrushCondition = "parameters/conditions/is_brushing";
    private const string MineCondition = "parameters/conditions/is_mining";
    private const string TakeDamageCondition = "parameters/conditions/is_taking_damage";
    private const string DeadCondition = "parameters/conditions/is_dead";
	
	
    private const string MoveVelocity = "parameters/move/blend_position";
    private const string HangingVelocity = "parameters/climb/blend_position";


    public override void _EnterTree()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    public override void _Ready()
    {
        Active = true;
        _stateMachine = Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
    }
    
    
    public void SetAnimation()
    {
        if(_playerControllerVariables.State == MotionState.Hanging)
            PlayHangingAnimation();
        else
            PlayMovementAnimation();
    }
    
    public void PlayHangingAnimation()
    {
        var velocity = _playerControllerVariables.Velocity;
        GD.Print("Animation tree hanging being called");
		  
        Set(MoveCondition, false);
        Set(HangCondition, true);
        Set(HangingVelocity, velocity);
        _stateMachine.Travel("climb");
    }
    
    public void PlayMovementAnimation()
    {
        var velocity = _playerControllerVariables.Velocity;
        
        Set(MoveCondition, true);
        Set(HangCondition, false);
        Set(AttackCondition, false);
        Set(BrushCondition, false);
        Set(MineCondition, false);
        Set(TakeDamageCondition, false);
        Set(DeadConditiondd, false);
        GD.Print($"Velocity: {velocity}");
        Set(MoveVelocity, velocity);
        _stateMachine.Travel("move");
    }
}