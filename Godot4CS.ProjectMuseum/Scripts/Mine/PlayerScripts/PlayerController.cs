using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerController : CharacterBody2D, IDamagable, IAttack
{
	[Export] private AnimationController _animationController;

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
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnSuccessfulDigActionCompleted += ReducePlayerEnergy;
	}

	public override void _Ready()
	{
		_playerControllerVariables.Player = this;
		_playerControllerVariables.State = MotionState.Falling;
		_playerControllerVariables.PlayerHealth = 200;
		_playerControllerVariables.PlayerEnergy = 2000;
	}

	public override void _PhysicsProcess(double delta)
	{
        PlayerMovement(delta);
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
				Velocity = input * _playerControllerVariables.Acceleration * (float)delta;
				Velocity = Velocity.LimitLength(PlayerControllerVariables.MaxSpeed);
			}
		}
        
		ApplyGravity();
		_animationController.SetAnimation(PlayerAttack());
		CheckFallTime(delta);
		DetectCollision();
		ModifyPlayerVariables();
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
			_animationController.PlayAnimation("fall");
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
		var collision = MoveAndCollide(Velocity, recoveryAsCollision: true);

		if (collision == null)
		{
			if (_playerControllerVariables.State != MotionState.Hanging)
				_playerControllerVariables.State = MotionState.Falling;
		}
		else
			MineActions.OnPlayerCollisionDetection?.Invoke(collision);
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

	private void PlayerGrab()
	{
		_playerControllerVariables.State = _playerControllerVariables.State == MotionState.Hanging ? 
			MotionState.Falling : MotionState.Hanging;
		_animationController.PlayAnimation("idle_to_climb");
		_playerControllerVariables.Acceleration = _playerControllerVariables.State == MotionState.Hanging ? 
			PlayerControllerVariables.MaxSpeed / 2 : PlayerControllerVariables.MaxSpeed;
	}
	
	public override void _Input(InputEvent @event)
	{
		if(!_playerControllerVariables.CanMove) return;
		MouseMotion(@event);
		if(@event.IsActionReleased("toggle_grab"))
			PlayerGrab();
		if(@event.IsActionReleased("Lamp"))
			Lamp();
		if(@event.IsActionReleased("Test"))
			Test();
	}

	#region For Testing Purposes

	private void Test()
	{
		GD.Print($"State: {_playerControllerVariables.State}");
	}

	private void Lamp()
	{
		var scene = ResourceLoader.Load<PackedScene>(_lampScenePath).Instantiate();
		GD.Print($"lamp scene instantiated {scene is null}");
		_mineGenerationVariables.MineGenView.TileMap.AddChild(scene);
		var cellPos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(Position);
		//var cell = _mineGenerationVariables.GetCell(cellPos);
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
		_animationController.Play("damage1");
		HealthSystem.ReducePlayerHealth(10,200, _playerControllerVariables);
	}

	public void Attack()
	{
		
	}
}