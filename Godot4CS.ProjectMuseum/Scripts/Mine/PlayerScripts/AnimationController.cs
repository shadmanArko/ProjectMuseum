using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;

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
		MineActions.OnDigActionStarted += PlayDigAnimation;
		MineActions.OnMeleeAttackActionStarted += PlayMeleeAttackAnimation;
		MineActions.OnBrushActionStarted += PlayBrushAnimation;
        
		MineActions.OnMouseMotionAction += SpriteFlipBasedOnMousePosition;
		MineActions.OnPlayerGrabActionPressed += ToggleHangOnWall;

		_sprite.FrameChanged += OnDigAnimationStrikeStarted;
	}
	public void SetAnimation(bool isAttacking)
	{
		var tempVelocity = _playerControllerVariables.Velocity;
		
		if(_playerControllerVariables.State == MotionState.Hanging)
			PlayHangingAnimations(tempVelocity);
		else
			PlayMovementAnimations(tempVelocity);
	}

	private void PlayMovementAnimations(Vector2 velocity)
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

	private void PlayHangingAnimations(Vector2 velocity)
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

	private void PlayMeleeAttackAnimation()
	{
		PlayAnimation("attack");
	}

	#region Brush Animation

	private void PlayBrushAnimation()
	{
		PlayAnimation("brush");
	}

	private void OnBrushAnimationStarted(string animName)
	{
		if(!animName.Contains("brush")) return;
		
		
	}

	private void OnBrushAnimationEnded(string animName)
	{
		if(!animName.Contains("brush")) return;
		
		
	}

	#endregion

	#region Dig Animation

	private void PlayDigAnimation()
	{
		var mouseDirection = _playerControllerVariables.MouseDirection;
		if (mouseDirection == Vector2I.Up)
			PlayAnimation("mining_up");
		else if (mouseDirection == Vector2I.Down)
			PlayAnimation("mining_down");
		else
			PlayAnimation("mining_horizontal");
	}
    
	private void OnDigAnimationStrikeStarted()
	{
		if (_sprite.Frame is not (92 or 77 or 107)) return;
		MineActions.OnDigActionStarted?.Invoke();
	}

	private void OnDigAnimationEnded(string animName)
	{
		if(!animName.Contains("mining")) return;
		
		MineActions.OnDigActionEnded?.Invoke();
	}

	#endregion

	#region Melee Attack Animation

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

	private void OnMeleeAttackAnimationStarted(string animName)
	{
		
	}
	
	private void OnMeleeAttackAnimationEnded(string animName)
	{
		
	}

	#endregion

	public void PlayAnimation(string state)
	{
		if (_playerControllerVariables.State == MotionState.Hanging)
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
		PlayAnimation(_playerControllerVariables.State == MotionState.Hanging ? "idle_to_climb" : "climb_to_idle");
	}

	private void SpriteFlipBasedOnMousePosition(double mousePos)
	{
		if(!_playerControllerVariables.CanMove) return;
		
		_sprite.FlipH = mousePos is < 90 and >= -90;
	}
}