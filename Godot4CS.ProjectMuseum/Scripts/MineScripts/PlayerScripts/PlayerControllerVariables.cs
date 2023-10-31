using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public class PlayerControllerVariables
{
	#region Movement Variables

	public int MaxSpeed = 100;
	public int Acceleration = 100;
	public int Friction = 200;
	
	public int Deceleration = 2;
	public float InterpolationTime = 0.5f;

	#endregion
	
	#region Gravity Variables

	public float Gravity;
	public float InitialGravity = 800f;
	public float MaxGravity = 800f;
	public bool IsGrounded;

	public bool IsHanging;

	#endregion
}