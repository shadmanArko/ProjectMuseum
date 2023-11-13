using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class AnimationController : AnimationPlayer
{
	private PlayerControllerVariables _playerControllerVariables;

	[Export] private Sprite2D _sprite;
	
	public override void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerAttackActionPressed += PlayAttackAnimation;
		MineActions.OnMouseMotionAction += SpriteFlipBasedOnMousePosition;
		MineActions.OnPlayerGrabActionPressed += ToggleHangOnWall;
	}
	public void SetAnimation(bool isAttacking)
	{
		var tempVelocity = _playerControllerVariables.Velocity;
		if (tempVelocity.Y >= 10)
			_playerControllerVariables.IsFalling = true;
		
		if(_playerControllerVariables.IsHanging)
			PlayHangingAnimations(tempVelocity, isAttacking);
		else
			PlayMovementAnimations(tempVelocity, isAttacking);
	}

	private void PlayMovementAnimations(Vector2 velocity, bool isAttacking)
	{
		switch (velocity.X)
		{
			case > 0:
				_sprite.FlipH = true;
				PlayAnimation("run");
				break;
			case < 0:
				_sprite.FlipH = false;
				PlayAnimation("run");
				break;
			default:
				_sprite.FlipH = _sprite.FlipH;
				PlayAnimation("idle");
				break;
		}
	}

	private void PlayHangingAnimations(Vector2 velocity, bool isAttacking)
	{
		switch (velocity.X)
		{
			case > 0:
				_sprite.FlipH = true;
				PlayAnimation("climb_horizontal");
				break;
			case < 0:
				_sprite.FlipH = false;
				PlayAnimation("climb_horizontal");
				break;
			default:
				_sprite.FlipH = _sprite.FlipH;
				PlayAnimation("climb_idle");
				break;
		}
		
		PlayAnimation(velocity.Y is > 0 or < 0 ? "climb_vertical" : "climb_idle");
	}

	private void PlayAttackAnimation()
	{
		var mouseDirection = _playerControllerVariables.MouseDirection;
		switch (_playerControllerVariables.CurrentEquippedItem)
		{
			case Equipables.Sword:
				PlayAnimation("attack");
				break;
			case Equipables.PickAxe:
				if (mouseDirection == Vector2I.Up)
					PlayAnimation("mining_up");
				else if (mouseDirection == Vector2I.Down)
					PlayAnimation("mining_down");
				else
					PlayAnimation("mining_horizontal");
				break;
			case Equipables.Brush:
				PlayAnimation("brush");
				break;
		}
	}

	public void PlayAnimation(string state)
	{
		if (_playerControllerVariables.IsHanging)
		{
			if(state == "climb_idle")
			{
				if (CurrentAnimation == "")
				{
					Play(state);
				}
			}
			else
			{
				Play(state);
			}
		}
		else
		{
			if (state == "idle")
			{
				if(CurrentAnimation == "")
					Play(state);
			}
			else if (state == "run")
			{
				if(CurrentAnimation != "attack" && !CurrentAnimation.Contains("mining") && !CurrentAnimation.Contains("brush"))
					Play(state);
			}
			else
				Play(state);
		}
	}

	private void ToggleHangOnWall()
	{
		PlayAnimation(_playerControllerVariables.IsHanging ? "idle_to_climb" : "climb_to_idle");
	}

	private void SpriteFlipBasedOnMousePosition(double mousePos)
	{
		if(!_playerControllerVariables.CanMove) return;
		_sprite.FlipH = mousePos is < 90 and >= -90;
	}
}