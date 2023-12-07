using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces.Movement;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class Enemy : CharacterBody2D, IUnit, IMovement, IAttack, IDamagable
{
    public string Id { get; set; }
    [Export] public NavigationAgent2D NavAgent { get; set; }
    [Export] public Timer TrackTimer { get; set; }
    [Export] protected AnimationTree AnimTree;
    
    public EnemyState State { get; set; }
    public bool IsAffectedByGravity { get; set; }
    
    protected AnimationNodeStateMachinePlayback StateMachine;
    protected Vector2 MoveDirection;
    
    [Export] public float MoveSpeed = 20;
    [Export] public float AggroRange = 140f;
    [Export] public bool IsAggro;

    
    
    [Export] public TextureProgressBar HealthBar;
    [Export] public EnemyAnimationController AnimationController;
        
        
    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            HealthBar.Value = _health;
            
            // if(_health <= 0)
            //     Die();
        }
    }
    
    
    public virtual void Chase()
    {
        
    }

    public virtual void Attack()
    {
        
    }

    public virtual void TakeDamage()
    {
        
    }
}