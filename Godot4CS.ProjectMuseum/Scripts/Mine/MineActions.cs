using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineActions : Node
{
	public static Action OnPlayerDigActionPressed;
	public static Action OnPlayerBrushActionPressed;
	
	public static Action OnPlayerMeleeAttackActionStarted;
	public static Action OnPlayerMeleeAttackActionEnded;
	
    public static Action OnPlayerRangedAttackActionPressed;

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