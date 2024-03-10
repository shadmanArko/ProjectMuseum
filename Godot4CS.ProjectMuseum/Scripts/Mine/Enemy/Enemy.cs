using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces.Movement;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Enemy : CharacterBody2D, IUnit, IMovement, IAttack, IDamagable, IDeath
{
    public string Id { get; set; }
    [Export] public NavigationAgent2D NavAgent { get; set; }
    [Export] public Timer TrackTimer { get; set; }
    [Export] protected AnimationTree AnimTree;
    
    public EnemyState State { get; set; }
    public bool IsAffectedByGravity { get; set; }
    
    protected AnimationNodeStateMachinePlayback StateMachine;
    protected Vector2 MoveDirection;
    
    [Export] public bool IsGoingToStartingPosition;
    [Export] public bool IsGoingToEndingPosition;
    protected Vector2 StartingMovementPosition;
    protected Vector2 EndingMovementPosition;
    
    [Export] public float MoveSpeed = 20;
    [Export] public float AggroRange = 140f;
    [Export] public float KnockBackPower = 500f;
    
    [Export] public TextureProgressBar HealthBar;
    [Export] public EnemyAnimationController AnimationController;

    #region Actions

    public Action OnSpawn;
    public Action OnDeath;
    public Action OnAggroChanged;
    public Action OnAttackChanged;
    public Action OnCanMoveChanged;

    #endregion

    #region Stats

    #region IsDead

    [Export] private bool _isDead;
    public bool IsDead
    {
        get => _isDead;
        set
        {
            if(_isDead == value) return;
            _isDead = value;
            OnDeath?.Invoke();
        }
    }

    #endregion

    #region Is Aggro

    [Export] private bool _isAggro;
    public bool IsAggro
    {
        get => _isAggro;
        set
        {
            if (_isAggro == value) return;
            _isAggro = value;
            OnAggroChanged?.Invoke();
        }
    }

    #endregion

    #region Can Move

    [Export] private bool _canMove;

    public bool CanMove
    {
        get => _canMove;
        set
        {
            if(_canMove == value) return;
            _canMove = value;
            OnCanMoveChanged?.Invoke();
        }
    }

    #endregion

    #region Is Attacking

    [Export] private bool _isAttacking;
    public bool IsAttacking {
        get=> _isAttacking;
        set
        {
            if(_isAttacking == value) return;
            _isAttacking = value;
            OnAttackChanged?.Invoke();
        }
    }

    #endregion

    #region Health

    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            HealthBar.Value = _health;
            
            if(_health <= 0)Death();
        }
    }

    #endregion

    #endregion
    
    public virtual void Chase()
    {
        
    }

    public virtual void Attack()
    {
        
    }

    public virtual void TakeDamage()
    {
        
    }

    public virtual void Death()
    {
        GD.Print("Enemy Death Called");
    }
}