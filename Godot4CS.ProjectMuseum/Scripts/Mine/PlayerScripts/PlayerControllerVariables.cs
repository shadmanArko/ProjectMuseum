using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public class PlayerControllerVariables
{
    public PlayerController Player;

    #region Tutorial Booleans

    public bool CanMoveLeftAndRight;
    public bool CanAttack;
    public bool CanBrush;
    public bool CanDig;
    public bool CanToggleClimb;

    #endregion
    
    #region Movement Variables

    public const int MaxSpeed = 30;
    public int Acceleration = 30;
    public const int Friction = 5;

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
                        if(!CanDig) return;
                        MineActions.OnLeftMouseClickActionStarted?.Invoke();
                        MineActions.OnDigActionStarted?.Invoke();
                        MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("DigActionCompleted");
                        break;
                    case Equipables.Sword:
                        if(!CanAttack) return;
                        MineActions.OnLeftMouseClickActionStarted?.Invoke();
                        MineActions.OnMeleeAttackActionStarted?.Invoke();
                        MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("AttackActionCompleted");
                        break;
                    default:
                        MineActions.OnLeftMouseClickActionStarted?.Invoke();
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
            if(_currentEquippedItem == Equipables.Sword)
                MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("SwordSelected");
            // else if(_currentEquippedItem == Equipables.Brush)
            //     MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("BrushSelected");
            else if(_currentEquippedItem == Equipables.PickAxe)
                MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("PickaxeSelected");
        }       
    }

    private Vector2 _position;
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            MineActions.OnPlayerPositionUpdated?.Invoke();
        }
    }

    private Vector2 _velocity;
    public Vector2 Velocity
    {
        get => _velocity;
        set
        {
            _velocity = value;
            MineActions.OnPlayerVelocityUpdated?.Invoke();
        }
    }

    public Vector2I MouseDirection;
    public Vector2I PlayerDirection;

    #endregion
}