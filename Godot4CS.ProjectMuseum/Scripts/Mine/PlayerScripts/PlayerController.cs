using System;
using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerController : CharacterBody2D, IDamageable, IDeath
{
	[Export] public AnimationController AnimationController;

	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	[Export] private float _maxVerticalVelocity;
	[Export] private float _fallTime;
	[Export] private float _fallTimeThreshold;
	
	[Export(PropertyHint.Range, "1,200,1")]
	public int MovementFactor = 100;

	#region Initializers

	public override void _EnterTree()
	{
		InitializeDiReferences();
		SubscribeToActions();
		_playerControllerVariables.Player = this;
		_playerControllerVariables.PlayerHealth = 200;
		_playerControllerVariables.PlayerEnergy = 200;
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
		_playerControllerVariables.Player = this;
	}

	private void SubscribeToActions()
	{
		MineActions.OnSuccessfulDigActionCompleted += ReducePlayerEnergy;
		MineActions.OnPlayerHealthValueChanged += Death;
		MineActions.OnTakeDamageStarted += TakeDamage;
		MineActions.OnPlayerPositionUpdated += OnPlayerClimbingVine;
	}

	#endregion

	public override void _PhysicsProcess(double delta)
	{
        if(_playerControllerVariables.CanMove)
	        PlayerMovement(delta);

        if (_playerControllerVariables.IsAffectedByGravity)
        {
	        ApplyGravity();
	        CheckFallTime(delta);
	        DetectCollision();
        }

        PlayerAttack();
        AnimationController.SetAnimation();
        ModifyPlayerVariables();
	}
    
	private void PlayerMovement(double delta)
	{
		if (_playerControllerVariables.CanMove)
		{
			var input = GetInputKeyboard();
			if (input == Vector2.Zero)
			{
				if (Velocity.Length() > PlayerControllerVariables.Friction * delta * MovementFactor)
					Velocity -= Velocity.Normalized() * (PlayerControllerVariables.Friction * (float)delta * MovementFactor);
				else
					Velocity = Vector2.Zero;
			}
			else
			{
				if (!_playerControllerVariables.CanMoveLeftAndRight) return;
				if (_playerControllerVariables.State == MotionState.Falling)
				{
					var vel = Velocity;
					Velocity = new Vector2(0, vel.Y);
				}
				else
				{
					Velocity = input * _playerControllerVariables.Acceleration * (float)delta * MovementFactor;
					Velocity = Velocity.LimitLength(PlayerControllerVariables.MaxSpeed * MovementFactor);
				}
			}
		}
	}
	
	private void ApplyGravity()
	{
		if (_playerControllerVariables.State is MotionState.Grounded or MotionState.Hanging)
		{
			_fallTime = 0;
			return;
		}
        
		var previousVerticalVelocity = Velocity.Y;
		var currentVerticalVelocity = Mathf.Clamp(previousVerticalVelocity + _playerControllerVariables.Gravity, 0, _maxVerticalVelocity);
		Velocity = new Vector2(Velocity.X, currentVerticalVelocity);
	}

	private void CheckFallTime(double delta)
	{
		if(_fallTime >= _fallTimeThreshold)
			AnimationController.PlayAnimation("fall");
		else
			_fallTime += (float) delta;
	}

	private void ModifyPlayerVariables()
	{
		_playerControllerVariables.Position = Position;
		_playerControllerVariables.Velocity = Velocity;
	}

	private void DetectCollision()
	{
		MoveAndSlide();
		if (GetSlideCollisionCount() <= 0)
		{
			if (_playerControllerVariables.State != MotionState.Hanging)
				_playerControllerVariables.State = MotionState.Falling;
		}
		else
			MineActions.OnPlayerCollisionDetection?.Invoke(GetSlideCollision(0));
	}

	private void ReducePlayerEnergy()
	{
		EnergySystem.ReduceEnergy(1, 200, _playerControllerVariables);
	}

	#region Input

	private Vector2 GetInputKeyboard()
	{
		Vector2 motion;
		if (_playerControllerVariables.State == MotionState.Hanging)
		{
			motion = new Vector2
			{
				X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
				Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up")
			};
		}
		else
		{
			motion = new Vector2
			{
				X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left")
			};
		}
		
		return motion.Normalized();
	}
	
	private bool PlayerAttack()
	{
		var input = Input.IsActionJustReleased("ui_left_click") && _playerControllerVariables.PlayerEnergy > 0;
		return input;
	}

	private void PlayerRoll()
	{
		if(_playerControllerVariables.State != MotionState.Grounded) return;
		MineActions.OnRollStarted?.Invoke();
	}

	private void PlayerGrab()
	{
		if(!_playerControllerVariables.CanToggleClimb) return;

		if (_playerControllerVariables.State == MotionState.Hanging)
		{
			AnimationController.PlayAnimation("climb_to_idle");
			_playerControllerVariables.State = MotionState.Falling;
		}
		else
		{
			AnimationController.PlayAnimation("idle_to_climb");
			_playerControllerVariables.State = MotionState.Hanging;
			MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("ToggleGrab");
		}
		
		_playerControllerVariables.Acceleration = _playerControllerVariables.State == MotionState.Hanging ? 
			PlayerControllerVariables.MaxSpeed / 2 : PlayerControllerVariables.MaxSpeed;
	}
	
	public override void _Input(InputEvent @event)
	{
		if(!_playerControllerVariables.CanMove) return;
		MouseMotion(@event);
		if(@event.IsActionReleased("toggle_grab"))
			PlayerGrab();
		if (@event.IsActionReleased("Test"))
		{
			GD.Print($"can move {_playerControllerVariables.CanMove}");
			GD.Print($"can attack {_playerControllerVariables.CanAttack}");
			GD.Print($"is attack {_playerControllerVariables.IsAttacking}");
			GD.Print($"can dig {_playerControllerVariables.CanDig}");
		}
	}
    
	private void MouseMotion(InputEvent @event)
	{
		if(@event is not InputEventMouseMotion) return;
		var mousePos = GetGlobalMousePosition();
		var angle = GetAngleTo(mousePos);
		var degree = angle * (180 / Math.PI);
		
		_playerControllerVariables.MouseDirection = degree switch
		{
			<= 45 and > -45 => Vector2I.Right,
			<= -45 and > -135 => Vector2I.Up,
			> 45 and <= 135 => Vector2I.Down,
			_ => Vector2I.Left
		};
		
		if(GetInputKeyboard().Normalized().X != 0) return;
		MineActions.OnMouseMotionAction?.Invoke(degree);
	}

	#endregion

	private void OnPlayerClimbingVine()
	{
		if(_playerControllerVariables.State != MotionState.Hanging) return;
		var vineInfos = _mineGenerationVariables.Mine.VineInformations;
		var currentCellPos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
		var cell = _mineGenerationVariables.GetCell(currentCellPos);
		if(cell == null) return;
		
		var playerHangingOnVine = vineInfos.Any(vineInfo =>
			vineInfo.VineCellPositions.Any(vineId => vineId == cell.Id));
		_playerControllerVariables.Acceleration = playerHangingOnVine ? 
			PlayerControllerVariables.MaxSpeed : PlayerControllerVariables.MaxSpeed / 2;
	}

	public void TakeDamage(int damageValue)
	{
		if(_playerControllerVariables.IsDead) return;
		var velocity = Velocity;
		velocity.X = 0;
		Velocity = velocity;
		AnimationController.PlayAnimation("damage1");
		HealthSystem.ReducePlayerHealth(damageValue, _playerControllerVariables);
		if (_playerControllerVariables.State == MotionState.Hanging)
			_playerControllerVariables.State = MotionState.Falling;
	}

	public void Attack()
	{
		
	}

	public void Death()
	{
		if(_playerControllerVariables.PlayerHealth > 0) return;
		if(_playerControllerVariables.IsDead) return;
		var velocity = Velocity;
		velocity.X = 0;
		Velocity = velocity;
		AnimationController.Play("death");
		_playerControllerVariables.CanMove = false;
	}

	private void OnDeathAnimationFinished(string animName)
	{
		if(animName != "death") return;
		_playerControllerVariables.IsDead = true;
		SetPhysicsProcess(false);
		MineActions.OnPlayerDead?.Invoke();
		_ExitTree();
	}
	
	private void UnsubscribeToActions()
	{
		MineActions.OnSuccessfulDigActionCompleted -= ReducePlayerEnergy;
		MineActions.OnPlayerHealthValueChanged -= Death;
		MineActions.OnTakeDamageStarted -= TakeDamage;
		MineActions.OnPlayerPositionUpdated -= OnPlayerClimbingVine;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		UnsubscribeToActions();
	}
}