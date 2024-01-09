using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerController : CharacterBody2D, IDamagable, IAttack, IDeath
{
	[Export] public AnimationController animationController;

	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	[Export] private float _maxVerticalVelocity;
	[Export] private float _fallTime;
	[Export] private float _fallTimeThreshold;

	[Export] private string _lampScenePath;

	public override void _EnterTree()
	{
		InitializeDiReferences();
		SubscribeToActions();
		_playerControllerVariables.Player = this;
		_playerControllerVariables.PlayerHealth = 200;
		_playerControllerVariables.PlayerEnergy = 2000000;
		_playerControllerVariables.CurrentEquippedItem = Equipables.Sword;
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
	}

	public override void _Ready()
	{
		GD.Print($"player is null: {_playerControllerVariables.Player is null}");
	}

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
        
        animationController.SetAnimation(PlayerAttack());
        ModifyPlayerVariables();
	}
    
	private void PlayerMovement(double delta)
	{
		if (_playerControllerVariables.CanMove)
		{
			var input = GetInputKeyboard();
			if (input == Vector2.Zero)
			{
				if (Velocity.Length() > PlayerControllerVariables.Friction * delta)
					Velocity -= Velocity.Normalized() * (PlayerControllerVariables.Friction * (float)delta);
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
					Velocity = input * _playerControllerVariables.Acceleration * (float)delta;
					Velocity = Velocity.LimitLength(PlayerControllerVariables.MaxSpeed);
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
			animationController.PlayAnimation("fall");
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
		_playerControllerVariables.IsAttacking = input;
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
			animationController.PlayAnimation("climb_to_idle");
			_playerControllerVariables.State = MotionState.Falling;
			_playerControllerVariables.Acceleration = PlayerControllerVariables.MaxSpeed / 2;
			MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("ToggleGrab");
		}
		else
		{
			animationController.PlayAnimation("idle_to_climb");
			_playerControllerVariables.State = MotionState.Hanging;
			_playerControllerVariables.Acceleration = PlayerControllerVariables.MaxSpeed;
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
		// if(@event.IsActionReleased("Lamp"))
		// 	Lamp();
		// if(@event.IsActionReleased("Test"))
		// 	Test();
	}

	#region For Testing Purposes

	private void Test()
	{
		GD.Print($"State: {_playerControllerVariables.State}");
	}

	private void Lamp()
	{
		var scene = ResourceLoader.Load<PackedScene>(_lampScenePath).Instantiate();
		_mineGenerationVariables.MineGenView.TileMap.AddChild(scene);
		var cellPos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(Position);
		scene!.Set("position", cellPos * _mineGenerationVariables.Mine.CellSize);
		GD.Print("Lamp instantiated");
	}

	#endregion

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

	public void TakeDamage()
	{
		var velocity = Velocity;
		velocity.X = 0;
		Velocity = velocity;
		animationController.Play("damage1");
		HealthSystem.ReducePlayerHealth(10,200, _playerControllerVariables);
	}

	public void Attack()
	{
		
	}

	public void Death()
	{
		if(_playerControllerVariables.PlayerHealth > 0) return;
		if(_playerControllerVariables.IsDead) return;
		GD.Print("Death animation");
		var velocity = Velocity;
		velocity.X = 0;
		Velocity = velocity;
		animationController.Play("death");
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

	public override void _ExitTree()
	{
		base._ExitTree();
	}
}