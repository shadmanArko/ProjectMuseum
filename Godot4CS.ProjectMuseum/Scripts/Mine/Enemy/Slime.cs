using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces.Movement;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Slime : CharacterBody2D, IUnit, IMovement, IAttack
{
    private PlayerControllerVariables _playerControllerVariables;
    [Export] private float _moveSpeed = 20;
    
    public string Id { get; set; }
    public NavigationAgent2D NavAgent { get; set; }
    public Timer TickTimer { get; set; }
    public EnemyState State { get; set; }
    
    public override void _EnterTree()
    {
        InitializeDiReferences();
    }

    public override void _Ready()
    {
        NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        TickTimer = GetNode<Timer>("Timer");
        State = EnemyState.Idle;
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    public void OnTimerTimeOut()
    {
        NavAgent.TargetPosition = _playerControllerVariables.Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = ToLocal(NavAgent.GetNextPathPosition()).Normalized();
        Velocity = direction * _moveSpeed * Vector2.Right;
        Velocity.MoveToward(direction, 0.2f);
        MoveAndCollide(Velocity);
    }

    public void Move()
    {
        
    }
    
    public void Attack()
    {
        
    }

    #region For Testing

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("Enemy"))
        {
            Position = _playerControllerVariables.Position;
        }
    }

    #endregion

    
}