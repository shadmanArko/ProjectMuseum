using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class FlyingEnemy : CharacterBody2D, IDamageable
{
    #region Phase

    [Export] private FlyingEnemyPhase _phase;
    public FlyingEnemyPhase Phase 
    { 
        get=> _phase;
        set
        {
            if(_phase != value)
                GD.Print($"Phase changed to: {value}");
            _phase = value;
        }
    }

    #endregion

    #region Actions

    public Action OnSpawn;
    public Action OnDeath;
    public Action OnAggroChanged;
    public Action OnAttackChanged;
    public Action OnCanMoveChanged;
    public Action OnTakeDamage;

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
            Death();
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
    
    #region Health

    protected int FullHealthValue;
    
    private int _health;
    protected int Health
    {
        get => _health;
        set
        {
            var newHealth = value;
            var currentHealth = _health;
            _health = Mathf.Clamp(value, 0, FullHealthValue);
            HealthBar.Value = _health;
            
            if(currentHealth > newHealth)
                OnTakeDamage?.Invoke();
            if (_health <= 0) IsDead = true;
        }
    }

    #endregion


    #endregion
    
    [Export] public float ChaseSpeed = 30;
    [Export] public float ExploreSpeed = 15;
    [Export] public float SearchRadius = 60f;
    [Export] public float AttackRadius = 10f;
    [Export] public float KnockBackPower = 100f;
    
    [Export] public TextureProgressBar HealthBar;
    [Export] public AnimationPlayer AnimPlayer;

    
    public virtual void TakeDamage(int damageValue)
    {
        
    }
    
    public virtual void Death()
    {
        GD.Print("Enemy Death Called");
    }
}