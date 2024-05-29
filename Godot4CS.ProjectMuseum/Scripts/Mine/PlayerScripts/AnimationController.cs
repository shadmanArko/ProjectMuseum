using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class AnimationController : AnimationPlayer
{
	private PlayerControllerVariables _playerControllerVariables;

	[Export] private AudioStreamPlayer2D _audioStreamPlayer2D;
	[Export] private Sprite2D _sprite;

	#region Initializers

	public override void _Ready()
	{
		//_audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("");
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
		MineActions.OnRollStarted += PlayRollAnimation;
        
		MineActions.OnMouseMotionAction += SpriteFlipBasedOnMousePosition;
		MineActions.OnPlayerGrabActionStarted += ToggleHangOnWall;

		_sprite.FrameChanged += OnDigAnimationStrikeStarted;
	}

	private void UnsubscribeToActions()
	{
		MineActions.OnDigActionStarted -= PlayDigAnimation;
		MineActions.OnMeleeAttackActionStarted -= PlayMeleeAttackAnimation;
		MineActions.OnRollStarted -= PlayRollAnimation;
		
		MineActions.OnMouseMotionAction -= SpriteFlipBasedOnMousePosition;
		MineActions.OnPlayerGrabActionStarted -= ToggleHangOnWall;
		
		_sprite.FrameChanged -= OnDigAnimationStrikeStarted;
	}

	#endregion
	
	public void SetAnimation()
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
    
	#region Dig Animation

	private void PlayDigAnimation()
	{
		var mouseDirection = _playerControllerVariables.MouseDirection;
		var isHanging = _playerControllerVariables.State == MotionState.Hanging;
		if (mouseDirection == Vector2I.Up)
			PlayAnimation(isHanging ? "climb_mine_up" : "mining_up");
		else if (mouseDirection == Vector2I.Down && !isHanging)
			PlayAnimation("mining_down");
		else
			PlayAnimation(isHanging ? "climb_mine_horizontal" : "mining_horizontal");
	}
    
	private void OnDigAnimationStrikeStarted()
	{
		if (_sprite.Frame is not (52 or 62 or 72 or 92 or 222 or 232)) return;
		MineActions.OnDigActionStarted?.Invoke();
	}

	private void OnDigAnimationEnded(string animName)
	{
		if(!animName.Contains("mining") && !animName.Contains("climb_mine")) return;
		//todo add sound
		_audioStreamPlayer2D.Play();
		MineActions.OnDigActionEnded?.Invoke();
	}

	#endregion

	#region Melee Attack Animation

	private void PlayMeleeAttackAnimation()
	{
		var mouseDirection = _playerControllerVariables.MouseDirection;
		var isHanging = _playerControllerVariables.State == MotionState.Hanging;

		if (isHanging)
		{
			if(mouseDirection == Vector2I.Up)
				PlayAnimation("climb_attack_up");
			else if(mouseDirection == Vector2I.Down)
				PlayAnimation("climb_attack_down");
			else
				PlayAnimation("climb_attack_horizontal");
		}
		else
			PlayAnimation("attack");
	}

	private void OnMeleeAttackAnimationStarted(string animName)
	{
		if (animName is not ("climb_attack_up" or "climb_attack_down" or "climb_attack_horizontal" or "attack")) return;
		_playerControllerVariables.IsAttacking = true;
	}
	
	private void OnMeleeAttackAnimationEnded(string animName)
	{
		if (animName is not ("climb_attack_up" or "climb_attack_down" or "climb_attack_horizontal" or "attack")) return;
		_playerControllerVariables.IsAttacking = false;
		MineActions.OnMeleeAttackActionEnded?.Invoke();
	}

	#endregion

	#region Roll Animation

	private void PlayRollAnimation()
	{
		Play("roll");
	}

	private void OnRollAnimationStarted(string animName)
	{
		if(animName != "roll") return;
		
	}

	private void OnRollAnimationEnded(string animName)
	{
		if(animName != "roll") return;
		
		MineActions.OnRollEnded?.Invoke();
	}

	#endregion

	public void PlayAnimation(string state)
	{
		if(state == CurrentAnimation) return;
		if (state == "death")
		{
			Play(state);
		}
		else if (state == "damage1")
		{
			if(CurrentAnimation == "death") return;
			Play(state);
		}
		else if (state.Contains("celebrate"))
		{
			if(CurrentAnimation is "damage1" or "death") return;
			Play(state);
		}
		else if (state.Contains("brush"))
		{
			if(CurrentAnimation is "damage1" or "death" or "celebrate") return;
			Play(state);
		}
		else if (state.Contains("attack"))
		{
			if(CurrentAnimation is "damage1" or "death" or "celebrate" or "brush") return;
			Play(state);
		}
		else if (state.Contains("mine") || state.Contains("mining"))
		{
			if(CurrentAnimation is "damage1" or "death" or "celebrate" or "brush" or "attack") return;
			Play(state);
		}
		else
		{
			if (_playerControllerVariables.State == MotionState.Hanging)
			{
				if(state == "climb_idle")
				{
				
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
		
		
	}

	private void ToggleHangOnWall()
	{
		PlayAnimation(_playerControllerVariables.State == MotionState.Hanging ? "idle_to_climb" : "climb_to_idle");
	}

	private void SpriteFlipBasedOnMousePosition(double mousePos)
	{
		if(!_playerControllerVariables.CanMove) return;
		if(_playerControllerVariables.Velocity.X != 0) return;
		_sprite.FlipH = mousePos is < 90 and >= -90;
		_playerControllerVariables.PlayerDirection = new Vector2I(_sprite.FlipH ? 1 : -1, 0);
	}

	public override void _ExitTree()
	{
		UnsubscribeToActions();
	}
}