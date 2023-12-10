using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

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

    //for starting the enemy damage at the right moment
    // public static Action OnPlayerAttackAnimationStarted; 
    // public static Action OnPlayerAttackAnimationEnded; 

    public static Action OnSuccessfulDigActionCompleted;

    public static Action OnPlayerHealthValueChanged;
    public static Action OnPlayerEnergyValueChanged;

    public static Action<Equipables> OnToolbarSlotChanged;

    public static Action<Node2D> OnMiniGameLoad;
    public static Action OnMiniGameWon;
    public static Action OnMiniGameLost;
    public static Action OnArtifactDiscoveryOkayButtonPressed;

	public static Action<KinematicCollision2D> OnPlayerCollisionDetection;

	public static Action<double> OnMouseMotionAction;

	public static Action OnPlayerGrabActionPressed;

}