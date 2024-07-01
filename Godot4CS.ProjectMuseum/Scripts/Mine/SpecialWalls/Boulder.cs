using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.SpecialWalls;

public partial class Boulder : RigidBody2D
{
    private MineGenerationVariables _mineGenerationVariables;

    [Export] private AnimatedSprite2D _anim;
    [Export] private CollisionShape2D _collider;

    private List<IDamageable> _damageables;
    private List<IItemizable> _itemizables;

    private bool _isFalling;

    public override void _Ready()
    {
        InitializeDiReference();
        SubscribeToActions();
        _damageables = new List<IDamageable>();
        _itemizables = new List<IItemizable>();
        CheckBoulderFallEligibility(Vector2I.Down);
    }
    
    private void InitializeDiReference()
    {
        ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }
    
    private void SubscribeToActions()
    {
        MineActions.OnMineCellBroken += CheckBoulderFallEligibility;
    }

    private void CheckBoulderFallEligibility(Vector2I brokenCellPos)
    {
        var currentCellPos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
        var cell = _mineGenerationVariables.GetCell(currentCellPos);
        if (cell == null) return;
        
        var bottomCellPos = new Vector2I(cell.PositionX, cell.PositionY) + Vector2I.Down;
        if(bottomCellPos != brokenCellPos) return;
        _isFalling = false;
        Freeze = true;
        BoulderShakeAndFall();
    }

    private async void BoulderShakeAndFall()
    {
        _anim.Play("boulderShake");
        await Task.Delay(1200);
        Freeze = false;
        _isFalling = true;
    }

    private void OnBodyEnter(Node2D body)
    {
        if(!_isFalling) return;
        if (body is IDamageable damageable)
        {
            if (_damageables.Contains(damageable)) return;
            GD.Print("Added a unit to damageables");
            _damageables.Add(damageable);
        }
        
        if(_damageables.Count > 0)
            DestroyBoulder();

        if (body is IItemizable itemizable)
        {
            itemizable.ConvertToInventoryItem();
            DestroyBoulder();
        }
    }

    private void DestroyBoulder()
    {
        LinearVelocity = Vector2.Zero;
        _isFalling = false;
        _collider.SetDeferred("disabled", true);
        GD.Print("Playing boulder break animation");
        _anim.Play("boulderBreak");
    }

    private void ItemizeItemizables()
    {
        LinearVelocity = Vector2.Zero;
        _isFalling = false;
        _collider.SetDeferred("disabled", true);
        GD.Print("Playing boulder break animation");
        _anim.Play("boulderBreak");
    }

    private void OnBodyExit(Node2D body)
    {
        if (body is IDamageable damageable)
            _damageables.Remove(damageable);
    }

    private void OnBreakAnimationComplete()
    {
        if(_anim.GetAnimation() != "boulderBreak") return;
        foreach (var damageable in _damageables)
        {
            GD.Print("damaged a unit");
            damageable.TakeDamage(180);
        }
        QueueFree();
    }
    
    private void UnsubscribeToActions()
    {
        MineActions.OnMineCellBroken -= CheckBoulderFallEligibility;
    }

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}