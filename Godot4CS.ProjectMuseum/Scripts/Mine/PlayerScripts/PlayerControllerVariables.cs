using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public class PlayerControllerVariables
{
	#region Movement Variables

	public int MaxSpeed = 100;
	public int Acceleration = 100;
	public int Friction = 200;

	#endregion
	
	#region Gravity Variables

	public float Gravity = 25f;
    
	private bool _isGrounded = false;
	private bool _isAttacking = false;
	private bool _isHanging = false;
	private bool _isFalling = false;

	public bool IsGrounded
	{
		get => _isGrounded;
		set => _isGrounded = value;
	}

	public bool IsAttacking
	{
		get => _isAttacking;
		set => _isAttacking = value;
	}

	public bool IsHanging
	{
		get => _isHanging;
		set
		{
			if (_isHanging != value)
			{
				_isHanging = value;
				MineActions.OnPlayerGrabActionPressed?.Invoke();
			}
		}
	}

	public bool IsFalling
	{
		get => _isFalling;
		set => _isFalling = value;
	}

	public Equipables CurrentEquippedItem
	{
		get => _currentEquippedItem;
		set => _currentEquippedItem = value;
	}

	#endregion

	private Equipables _currentEquippedItem;

	#region Other Variables

	public Vector2 Position;
	public Vector2 Velocity;

	public Vector2I MouseDirection;

	#endregion
}