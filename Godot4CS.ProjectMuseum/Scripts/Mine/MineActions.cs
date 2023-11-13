using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineActions : Node
{
	public static Action OnPlayerAttackActionPressed;
	public static Action OnPlayerMiningActionPressed;

	public static Action<KinematicCollision2D> OnPlayerCollisionDetection;

	public static Action<double> OnMouseMotionAction;

	public static Action OnPlayerGrabActionPressed;

}