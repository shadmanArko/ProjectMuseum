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
        else if (animName == "aggro")
        {
            if(CurrentAnimation is "damage" or "death") return;
            Play(animName);
        }
        else if (animName == "attack")
        {
            if(CurrentAnimation is "damage" or "death" or "aggro") return;
            Play(animName);
        }
        else if (animName is "digOut")
        {
            if(CurrentAnimation is "attack" or "damage" or "death" or "aggro") return;
            Play(animName);
        }
        else if (animName is "digIn")
        {
            if(CurrentAnimation is "attack" or "damage" or "death" or "attack"or "digOut" or "aggro") return;
            Play(animName);
        }
        else if (animName == "move")
        {
            if(CurrentAnimation is "attack" or "damage" or "death" or "attack"or "digIn" or "digOut" or "aggro") return;
            Play(animName);
        }
        else if (animName == "idle")
        {
            if(CurrentAnimation is "attack" or "damage" or "death" or "attack"or "digIn" or "digOut" or "move" or "aggro") return;
            Play(animName);
        }
        else
            Play(animName);
    }

    public void MoveDirection(Vector2 direction)
    {
        Sprite.FlipH = direction != Vector2.Left;
    }
}