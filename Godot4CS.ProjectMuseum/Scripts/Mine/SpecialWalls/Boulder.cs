using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.SpecialWalls;

public partial class Boulder : RigidBody2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    [Export] private AnimatedSprite2D _anim;

    private bool _isFalling;

    public override void _Ready()
    {
        InitializeDiReference();
        SubscribeToActions();
        CheckBoulderFallEligibility(Vector2I.Down);
    }
    
    private void InitializeDiReference()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }
    
    private void SubscribeToActions()
    {
        MineActions.OnMineCellBroken += CheckBoulderFallEligibility;
    }

    private async void CheckBoulderFallEligibility(Vector2I brokenCellPos)
    {
        var currentCellPos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
        var cell = _mineGenerationVariables.GetCell(currentCellPos);
        if (cell == null) return;
        
        var bottomCellPos = new Vector2I(cell.PositionX, cell.PositionY) + Vector2I.Down;
        if(bottomCellPos != brokenCellPos) return;
        
        _anim.Play("boulderShake");
        await Task.Delay(1200);
        Freeze = false;
        _isFalling = true;
    }

    private void OnBodyEnter(Node2D body)
    {
        if(!_isFalling) return;
        var unit = body as IDamageable;
        if(unit == null) return;
        LinearVelocity = Vector2.Zero;
        unit.TakeDamage(180);
        _isFalling = false;
        Freeze = true;
        _anim.Play("boulderBreak");
    }

    private void OnBreakAnimationComplete()
    {
        if(_anim.GetAnimation() != "boulderBreak") return;
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