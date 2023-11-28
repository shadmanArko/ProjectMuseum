using System;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public partial class PlayerController : CharacterBody2D
{
	[Export] private AnimationController _animationController;

	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	[Export] private float _maxVerticalVelocity;

	[Export] private string _lampScenePath;

	public override void _EnterTree()
	{
		InitializeDiReferences();
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public override void _Ready()
	{
		_playerControllerVariables.State = MotionState.Falling;
	}

	public override void _PhysicsProcess(double delta)
	{
        PlayerMovement(delta);
        
        GD.Print(_playerControllerVariables.State);
        
        if (Input.IsActionJustReleased("Test"))
        {
	        GD.Print($"State: {_playerControllerVariables.State}");
	        //GD.Print($"isFalling: {_playerControllerVariables.IsFalling}");
        }

        if (Input.IsActionJustReleased("Lamp"))
        {
	        var scene = ResourceLoader.Load<PackedScene>(_lampScenePath).Instantiate();
	        GD.Print($"lamp scene instantiated {scene is null}");
	        _mineGenerationVariables.MineGenView.TileMap.AddChild(scene);
	        scene!.Set("position", Position);
	        GD.Print("Lamp instantiated");
        }
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
		//if(_playerControllerVariables.CanMove) PlayerGrab();
		_animationController.SetAnimation(PlayerAttack());
		DetectCollision();
		ModifyPlayerVariables();
	}

	private void ApplyGravity()
	{
		if(_playerControllerVariables.State is MotionState.Grounded or MotionState.Hanging) return;

		var previousVerticalVelocity = Velocity.Y;
		var currentVerticalVelocity = Mathf.Clamp(previousVerticalVelocity + _playerControllerVariables.Gravity, 0, _maxVerticalVelocity);

		Velocity = new Vector2(Velocity.X, currentVerticalVelocity);
	}

	private void ModifyPlayerVariables()
	{
		_playerControllerVariables.Position = Position;
		_playerControllerVariables.Velocity = Velocity;
	}

	private void DetectCollision()
	{
		var collision = MoveAndCollide(Velocity, recoveryAsCollision: true);
		
		if(Input.IsActionJustReleased("Test"))
			GD.Print($"{collision.GetCollider()}");
		if (collision == null)
		{
			if (_playerControllerVariables.State != MotionState.Hanging)
				_playerControllerVariables.State = MotionState.Falling;
			
			return;
		}
        
        MineActions.OnPlayerCollisionDetection?.Invoke(collision);
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
		var input = Input.IsActionJustReleased("ui_left_click");
		_playerControllerVariables.IsAttacking = input;
		return input;
	}

	private void PlayerGrab()
	{
		//GD.Print("Player Grab method called");
		//var grab = Input.IsActionJustReleased("toggle_grab");
		//if (!grab) return;
		_playerControllerVariables.State = _playerControllerVariables.State == MotionState.Hanging ? 
			MotionState.Falling : MotionState.Hanging;
		_animationController.PlayAnimation("idle_to_climb");
		// await Task.Delay(500);
		// Position += new Vector2(0, -10);
		_playerControllerVariables.Acceleration = _playerControllerVariables.State == MotionState.Hanging ? 
			PlayerControllerVariables.MaxSpeed / 2 : PlayerControllerVariables.MaxSpeed;
	}
	
	public override void _Input(InputEvent @event)
	{
		if(!_playerControllerVariables.CanMove) return;
		MouseMotion(@event);
		if(@event.IsActionReleased("toggle_grab"))
			PlayerGrab();
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
		
		MineActions.OnMouseMotionAction?.Invoke(degree);
	}

	#endregion
}