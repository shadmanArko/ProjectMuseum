using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public class PlayerControllerVariables
{
	#region Movement Variables

	public const int MaxSpeed = 100;
	public int Acceleration = 100;
	public const int Friction = 200;

	#endregion
	
	#region Gravity Variables

	public float Gravity = 25f;

	private bool _isAttacking;
	private bool _isHanging;
	private bool _isFalling;

	public bool CanMove { get; set; }

	public bool IsGrounded { get; set; }

	public bool IsAttacking
	{
		get => _isAttacking;
		set
		{
			_isAttacking = value;
			if (_isAttacking)
			{
				switch (_currentEquippedItem)
				{
					case Equipables.PickAxe:
						MineActions.OnPlayerDigActionPressed?.Invoke();
						break;
					case Equipables.Brush:
						MineActions.OnPlayerBrushActionPressed?.Invoke();
						break;
					case Equipables.Sword:
						MineActions.OnPlayerMeleeAttackActionPressed?.Invoke();
						break;
					default:
						MineActions.OnPlayerMeleeAttackActionPressed?.Invoke();
						break;
				}
			}
		}
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

				if (!_isHanging) return;
				IsFalling = false;
				IsGrounded = false;
			}
		}
	}

	public bool IsFalling
	{
		get => _isFalling;
		set
		{
			_isFalling = value;

			if (!_isFalling) return;
			IsGrounded = false;
			IsHanging = false;
		}
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