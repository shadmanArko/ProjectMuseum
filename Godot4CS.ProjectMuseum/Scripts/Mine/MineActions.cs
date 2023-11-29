using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineActions : Node
{
	public static Action OnPlayerDigActionPressed;
	public static Action OnPlayerBrushActionPressed;
	public static Action OnPlayerMeleeAttackActionPressed;
    public static Action OnPlayerRangedAttackActionPressed;

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