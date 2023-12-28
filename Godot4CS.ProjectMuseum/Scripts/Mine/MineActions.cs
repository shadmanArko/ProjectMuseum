using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;
using ProjectMuseum.Models;
using Equipables = Godot4CS.ProjectMuseum.Scripts.Mine.Enum.Equipables;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineActions : Node
{
	public static Action OnDigActionStarted;
	public static Action OnDigActionEnded;
	
	public static Action OnBrushActionStarted;
	public static Action OnBrushActionEnded;
	
	public static Action OnMeleeAttackActionStarted;
	public static Action OnMeleeAttackActionEnded;
	
    public static Action OnRangedAttackActionStarted;
    public static Action OnRangedAttackActionEnded;

    public static Action OnRollStarted;
    public static Action OnRollEnded;

    //for starting the enemy damage at the right moment
    // public static Action OnPlayerAttackAnimationStarted; 
    // public static Action OnPlayerAttackAnimationEnded; 

    public static Action OnSuccessfulDigActionCompleted;

    public static Action OnPlayerHealthValueChanged;
    public static Action OnPlayerEnergyValueChanged;

    public static Action<Equipables> OnToolbarSlotChanged;

    public static Action<Vector2I> OnMiniGameLoad;
    public static Action OnMiniGameWon;
    public static Action OnMiniGameLost;
    public static Action<Vector2I> OnArtifactCellBroken;
    public static Action<Artifact> OnArtifactSuccessfullyRetrieved;

	public static Action<KinematicCollision2D> OnPlayerCollisionDetection;

	public static Action<double> OnMouseMotionAction;

	/// <summary>
	/// Mine Time System
	/// </summary>
	public static Action<int> OnEachMinutePassed;
	public static Action<int> OnEachHourPassed;

	public static Action OnPlayerGrabActionStarted;
	public static Action OnPlayerGrabActionEnded;
	
	public static Action<int, int, int, int, int> OnTimeUpdated;
}