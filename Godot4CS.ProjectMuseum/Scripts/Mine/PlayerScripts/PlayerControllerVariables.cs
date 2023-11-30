using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public class PlayerControllerVariables
{
    public PlayerController Player;
    
    #region Movement Variables

    public const int MaxSpeed = 100;
    public int Acceleration = 100;
    public const int Friction = 200;

    #endregion

    #region Gravity Variables

    public float Gravity = 25f;
    
    #endregion

    #region Action Variables
    
    public MotionState State;
    public bool CanMove { get; set; }

    private bool _isAttacking;
    public bool IsAttacking
    {
        get => _isAttacking;
        set
        {
            _isAttacking = value;
            if (_isAttacking)
            {
                if (!CanMove) return;
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
    
    #endregion

    #region Health Bar Variables

    private int _playerHealth;

    public int PlayerHealth
    {
        get => _playerHealth;
        set
        {
            _playerHealth = value;
            MineActions.OnPlayerHealthValueChanged?.Invoke();
        }
    }

    #endregion

    #region Energy Bar Variables

    private int _playerEnergy;

    public int PlayerEnergy
    {
        get => _playerEnergy;
        set
        {
            _playerEnergy = value;
            MineActions.OnPlayerEnergyValueChanged?.Invoke();
        }
    }

    #endregion

    #region Other Variables

    private Equipables _currentEquippedItem;

    public Equipables CurrentEquippedItem
    {
        get => _currentEquippedItem;
        set
        {
            _currentEquippedItem = value;
            //MineActions.OnToolbarSlotChanged?.Invoke(_currentEquippedItem);
        }
    }

    public Vector2 Position;
    public Vector2 Velocity;

    public Vector2I MouseDirection;

    #endregion
}