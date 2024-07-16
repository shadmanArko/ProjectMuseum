using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces.Movement;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class FlyingEnemy : CharacterBody2D, IDamageable
{
    public virtual void TakeDamage(int damageValue)
    {
        
    }
}