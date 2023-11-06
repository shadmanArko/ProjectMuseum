using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

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
		MineActions.OnPlayerAttackAction += PlayAttackAnimation;
		MineActions.OnMouseMotionAction += SpriteFlipBasedOnMousePosition;
	}
	public void SetAnimation(bool isAttacking)
	{
		var tempVelocity = _playerControllerVariables.Velocity.Normalized();
		// if(tempVelocity.X == 0 && !isAttacking)
		// 	PlayAnimation("idle");
		// else
		// {
		// 	if(_playerControllerVariables.IsHanging) 
		// 		PlayHangingAnimations(tempVelocity);
		// 	else 
		// 		PlayMovementAnimations(tempVelocity);
		// }

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
				PlayAnimation("run");
				break;
			case < 0:
				_sprite.FlipH = false;
				PlayAnimation("run");
				break;
			default:
				_sprite.FlipH = _sprite.FlipH;
				break;
		}
	}

	private void PlayAttackAnimation()
	{
		PlayAnimation("attack");
	}

	private void PlayAnimation(string state)
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
		_sprite.FlipH = mousePos is < 90 and >= -90;
	}
}