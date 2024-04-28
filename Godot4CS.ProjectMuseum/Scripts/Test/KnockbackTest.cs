using System.Threading.Tasks;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Test;

public partial class KnockBackTest : CharacterBody2D
{
    [Export] public float KnockBackPower = 100f;

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("ui_left_click"))
            _isKnockBack = true;
    }

    public override async void _PhysicsProcess(double delta)
    {
        if (_isKnockBack)
        {
            _isKnockBack = false;
            var playerDirection = Vector2.Left;
            GD.Print("knocking back");
            var knockBackDirection = (playerDirection - Velocity).Normalized() * KnockBackPower;
            await ApplyKnockBack(knockBackDirection);
        }
    }

    [Export] private bool _isKnockBack; 
    // private async Task KnockBack()
    // {
    //     
    //     await ApplyKnockBack(knockBackDirection);
    // }

    private async Task ApplyKnockBack(Vector2 knockBackDirection)
    {
        for (var i = 0; i < 10; i++)
        {
            Velocity = Velocity.Lerp(new Vector2(knockBackDirection.X, Velocity.Y), 0.8f);
            MoveAndSlide();
            GD.Print("lerping");
            await Task.Delay(1);
        }
        // Velocity = new Vector2(knockBackDirection.X,Velocity.Y);
        GD.Print($"getting knocked back {knockBackDirection}");
        
    }
}