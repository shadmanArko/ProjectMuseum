using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Enemy;

public partial class EnemyAnimationController : AnimationPlayer
{
    [Export] public Sprite2D Sprite;

    public void PlayAnimation(string animName)
    {
        if(animName == "damage") Play(animName);
        else if (animName == "attack" && CurrentAnimation is not "damage")
            Play(animName);
        else if(animName is "move" && CurrentAnimation is not "attack" and "damage")
            Play(animName);
        else
            Play(animName);
    }
    
    

    
}