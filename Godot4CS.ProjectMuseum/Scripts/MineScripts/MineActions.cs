using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

public partial class MineActions : Node
{
	public static Action OnPlayerAttackAction;

	public static Action<double> OnMouseMotionAction;

	public static Action<bool> OnPlayerGrabAction;

}