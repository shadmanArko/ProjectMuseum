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
    public EnemyState State { get; set; }
    public bool IsAffectedByGravity { get; set; }
    
    
    [Export] public float MoveSpeed = 20;
    [Export] public float AggroRange = 140f;
    [Export] public bool IsAggro;
    
    public Vector2 MoveDirection;
    
    [Export] public TextureProgressBar _healthBar;
    [Export] public EnemyAnimationController _animationController;
        
        
    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            _healthBar.Value = _health;
            
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