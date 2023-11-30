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
    [Export] private EnemyAnimationController _enemyAnimationController;
    [Export] private Timer _stateChangeTimer;
    
    [Export] private float _moveSpeed = 20;
    
    public string Id { get; set; }
    [Export] public NavigationAgent2D NavAgent { get; set; }
    [Export] public Timer TrackTimer { get; set; }
    public EnemyState State { get; set; }
    
    public override void _EnterTree()
    {
        InitializeDiReferences();
    }

    public override void _Ready()
    {
       Spawn();
    }

    private void InitializeDiReferences()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    public void OnTimerTimeOut()
    {
        NavAgent.TargetPosition = _playerControllerVariables.Position;
    }

    public void OnStateChangeTimeOut()
    {
        if (State == EnemyState.Idle)
            State = EnemyState.Move;
        else if (State == EnemyState.Move)
            State = EnemyState.DigIn;
        else if (State == EnemyState.DigIn)
            State = EnemyState.DigOut;
        else if (State == EnemyState.DigOut)
            State = EnemyState.Idle;
        GD.Print($"State changed to {State}");
    }

    public override void _PhysicsProcess(double delta)
    {
        if(State == EnemyState.Move)
            Move();
        else if(State == EnemyState.Idle)
            Idle();
        else if (State == EnemyState.Attack)
            Attack();
        else if (State == EnemyState.DigIn)
        {
            if(_enemyAnimationController.CurrentAnimation == "digIn") return;
            _enemyAnimationController.Play("digIn");
        }
        else if(State == EnemyState.DigOut)
            DigOut();
        
        PlayAnimation();
    }

    #region Slime Possible States

    
    
    public void Spawn()
    {
        State = EnemyState.DigOut;
        Position = _playerControllerVariables.Position;
        Visible = true;
        _enemyAnimationController.Play("digOut");
        SetPhysicsProcess(true);
    }

    public void DigOut()
    {
        if (!Visible)
        {
            _enemyAnimationController.Play("digOut");
            Visible = true;
            SetPhysicsProcess(true);
        }
        else
        {
            if(_enemyAnimationController.CurrentAnimation == "digOut") return;
            _stateChangeTimer.Paused = false;
            State = EnemyState.Idle;
        }
    }

    public void OnDigInAnimationFinished(string animName)
    {
        if(animName != "digIn") return;
        DigIn();
    }

    public void DigIn()
    {
        NavAgent.TargetPosition = Position;
        Velocity = Vector2.Zero;
        SetPhysicsProcess(false);
        _stateChangeTimer.Paused = true;
        State = EnemyState.DigOut;
        Visible = false;
    }

    public void Move()
    {
        var direction = ToLocal(NavAgent.GetNextPathPosition()).Normalized();
        Velocity = direction * _moveSpeed * Vector2.Right;
        Velocity.MoveToward(direction, 0.2f);
        MoveAndCollide(Velocity);
    }

    public void Idle()
    {
        //todo: do idle work
    }
    
    public void Attack()
    {
        
    }

    #endregion

    private void PlayAnimation()
    {
        switch (State)
        {
            case EnemyState.Move:
                _enemyAnimationController.Play("move");
                var directionBool = NavAgent.TargetPosition.X <= Position.X;
                _enemyAnimationController.Sprite.FlipH = directionBool;
                break;
            // case EnemyState.Attack:
            //     _enemyAnimationController.Play("attack");
            //     break;
            // case EnemyState.TakeDamage:
            //     _enemyAnimationController.Play("damage");
            //     break;
            case EnemyState.DigIn:
                _enemyAnimationController.Play("digIn");
                break;
            case EnemyState.DigOut:
                if(_enemyAnimationController.CurrentAnimation == "digOut") return;
                if (_enemyAnimationController.CurrentAnimation == "")
                    State = EnemyState.Idle;
                break;
            // case EnemyState.Fall:
            //     _enemyAnimationController.Play("fall");
            //     break;
            // case EnemyState.Aggro:
            //     _enemyAnimationController.Play("aggro");
            //     break;
            default:
                _enemyAnimationController.Play("idle");
                break;
        }
    }

    // #region For Testing
    //
    // public override void _Input(InputEvent @event)
    // {
    //     if (@event.IsActionReleased("Enemy"))
    //     {
    //         Position = _playerControllerVariables.Position;
    //     }
    // }
    //
    // #endregion

    
}