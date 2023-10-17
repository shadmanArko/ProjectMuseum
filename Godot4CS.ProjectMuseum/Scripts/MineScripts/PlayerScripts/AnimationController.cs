using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class AnimationController : AnimationPlayer
{
	[Export] private PlayerController _playerController;
	public override void _Ready()
	{
		_playerController.OnMouseMotion += SpriteFlipBasedOnMousePosition;
	}

	[Export] private Sprite2D _sprite;
	public void SetAnimation(Vector2 velocity, bool playerAttack)
	{
		var tempVelocity = velocity.Normalized();
		if(tempVelocity.X == 0 && !playerAttack)
			PlayAnimation("idle");
		else
		{
			switch (tempVelocity.X)
			{
				case > 0:
					_sprite.FlipH = false;
					PlayAnimation("run");
					break;
				case < 0:
					_sprite.FlipH = true;
					PlayAnimation("run");
					break;
				default:
					_sprite.FlipH = _sprite.FlipH;
					break;
			}
		}
	}

	public void PlayAnimation(string state)
	{
		if (state == "idle")
		{
			if(CurrentAnimation == "")
				Play(state);
		}
		else if (state == "run")
		{
			if(CurrentAnimation != "attack")
				Play(state);
		}
		else
			Play(state);
	}

	private void SpriteFlipBasedOnMousePosition(double mousePos)
	{
		_sprite.FlipH = mousePos is >= 90 or < -90;
	}
}