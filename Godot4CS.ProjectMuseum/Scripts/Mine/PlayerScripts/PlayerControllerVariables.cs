using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public class PlayerControllerVariables
{
    public PlayerController Player;
    
    #region Movement Variables

    public const int MaxSpeed = 2500;
    public int Acceleration = 2500;
    public const int Friction = 500;

    #endregion

    #region Gravity Variables

    public float Gravity = 30f;
    
    #endregion

    #region Action Variables
    
    public MotionState State;
    public bool CanMove { get; set; }
    public bool IsAffectedByGravity { get; set; }
    
    public bool IsDead { get; set; }

    private bool _isAttacking;
    public bool IsAttacking
    {
        get => _isAttacking;
        set
        {
            if(_isAttacking != value && _isAttacking == false && CurrentEquippedItem == Equipables.Sword)
                MineActions.OnMeleeAttackActionEnded?.Invoke();
            
            _isAttacking = value;
            if (_isAttacking)
            {
                if (!CanMove) return;
                switch (_currentEquippedItem)
                {
                    case Equipables.PickAxe:
                        MineActions.OnDigActionStarted?.Invoke();
                        break;
                    case Equipables.Brush:
                        MineActions.OnBrushActionStarted?.Invoke();
                        break;
                    case Equipables.Sword:
                        MineActions.OnMeleeAttackActionStarted?.Invoke();
                        break;
                    default:
                        MineActions.OnMeleeAttackActionStarted?.Invoke();
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
            MineActions.OnToolbarSlotChanged?.Invoke(_currentEquippedItem);
        }
    }

    public Vector2 Position;
    public Vector2 Velocity;

    public Vector2I MouseDirection;

    #endregion
}