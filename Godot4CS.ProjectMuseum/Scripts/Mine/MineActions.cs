using System;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineActions : Node
{
	#region Play Pause

	public static Action OnGamePaused;
	public static Action OnGameUnpaused;

	#endregion
	
	#region Spawn Death Actions

	public static Action OnPlayerSpawned;
	public static Action OnPlayerDead;

	#endregion

	#region Mouse Actions

	public static Action OnLeftMouseClickAction;
	
	public static Action OnLeftMouseHeldActionStarted;
	public static Action OnLeftMouseHeldActionEnded;
	
	public static Action OnRightMouseClickAction;
	
	public static Action<double> OnMouseMotionAction;

	#endregion

	#region Dig Actions

	public static Action OnDigActionStarted;
	public static Action OnDigActionEnded;

	#endregion

	#region Melee Attack Actions

	public static Action OnMeleeAttackActionStarted;
	public static Action OnMeleeAttackActionEnded;

	#endregion

	#region Ranged Attack Actions

	public static Action OnRangedAttackActionStarted;
	public static Action OnRangedAttackActionEnded;

	#endregion

	#region Take Damage

	public static Action<int> OnTakeDamageStarted;

	#endregion

    #region Roll Actions

    public static Action OnRollStarted;
    public static Action OnRollEnded;

    #endregion

    #region Mine Actions

    public static Action OnMineGenerated;
    public static Action<Vector2I> OnMineCellBroken;

    #endregion

    public static Action OnSuccessfulDigActionCompleted;

    public static Action<InventoryItem> OnCollectItemDrop;

    #region Health and Energy Actions

    public static Action OnPlayerHealthValueChanged;
    public static Action OnPlayerEnergyValueChanged;

    #endregion

    
    #region Mini Game Actions

    public static Action<Vector2I> OnMiniGameLoad;
    public static Action OnMiniGameWon;
    public static Action OnMiniGameLost;

    #endregion

    #region Artifact Actions

    public static Action<Vector2I> OnArtifactCellBroken;
    public static Action<Artifact> OnArtifactSuccessfullyRetrieved;

    #endregion
    
    public static Action OnPlayerReachBackToCamp;

    public static Action OnPlayerReachFirstWarning;

    #region Inventory Actions

    public static Action<int> OnToolbarSlotChanged;
    public static Action DeselectAllInventoryControllers;
    public static Action OnInventoryUpdate;
    public static Action<int, Texture2D> OnInventoryItemDragged;

    #endregion

	public static Action<KinematicCollision2D> OnPlayerCollisionDetection;
    
	#region Time System Actions

	public static Action<int> OnTenMinutesPassed;
	public static Action<int> OnOneHourPassed;
	public static Action<int> OnOneDayPassed;
	public static Action<int, int, int, int, int> OnTimeUpdated;

	#endregion

	#region Grab Actions

	public static Action OnPlayerGrabActionStarted;
	public static Action OnPlayerGrabActionEnded;

	#endregion
	

	#region Position and Velocity Actions

	public static Action OnPlayerPositionUpdated;
	public static Action OnPlayerVelocityUpdated;

	#endregion
	
	public static Action OnRawArtifactDTOInitialized;	//TODO: has to be changed later
}