using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.CellPlaceables.Explosives;

public partial class ExplosionEffect : Node2D
{
    [Export] private AnimatedSprite2D _animatedSprite2D;
    [Export] private string _animationName;

    public override void _Ready()
    {
        PlayExplosion();
    }

    private void PlayExplosion()
    {
        _animatedSprite2D.Play(_animationName);
    }

    private void OnTimerEnd()
    {
        QueueFree();
    }
}