using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public class PlayerControllerVariables
{
    public PlayerController Player;

    #region Tutorial Booleans

    public bool CanMoveLeftAndRight;
    public bool CanAttack;
    public bool CanDig;
    public bool CanToggleClimb;

    #endregion
    
    #region Movement Variables

    public const int MaxSpeed = 30;
    public int Acceleration = 30;
    public const int Friction = 5;

    #endregion

    #region Gravity Variables

    public float Gravity = 1f;
    
    #endregion

    #region Action Variables
    
    public MotionState State;

    public bool CanMove { get; set; }

    public bool IsAffectedByGravity { get; set; }
    
    public bool IsDead { get; set; }

    public bool IsAttacking { get; set; }
    public bool IsDigging { get; set; }

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

    #region Damage Points and Cooldown

    public int EnemyDamagePoint;
    public int CellDamagePoint;

    public float EnemyHitCoolDown;
    public float CellHitCoolDown;

    #endregion

    #region Other Variables

    private int _currentEquippedItemSlot;
    public int CurrentEquippedItemSlot
    {
        get => _currentEquippedItemSlot;
        set
        {
            _currentEquippedItemSlot = value;
            MineActions.DeselectAllInventoryControllers?.Invoke();
            MineActions.OnToolbarSlotChanged?.Invoke(_currentEquippedItemSlot);
            
            // if(_currentEquippedItemSlot == 0)
            //     MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("SwordSelected");
            // else if(_currentEquippedItemSlot == 1)
            //     MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("PickaxeSelected");
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