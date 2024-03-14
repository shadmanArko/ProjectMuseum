using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class EnemyAnimationController : AnimationPlayer
{
    [Export] public Sprite2D Sprite;

    public void PlayAnimation(string animName)
    {
        if(animName == CurrentAnimation) return;
        
        if(animName == "death") Play(animName);
        else if(animName == "damage" && CurrentAnimation != "death") 
            Play(animName);
        else if (animName == "attack" && CurrentAnimation is not "damage" and "death")
            Play(animName);
        else if(animName is "move" && CurrentAnimation is not "attack" and not "damage" and not "death")
            Play(animName);
        else if(animName == "idle")
            Play(animName);
        else
            Play(animName);
    }

    public void MoveDirection(Vector2 direction)
    {
        Sprite.FlipH = direction != Vector2.Left;
    }
}