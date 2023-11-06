using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

public class PlayerControllerVariables
{
	#region Movement Variables

	public int MaxSpeed = 100;
	public int Acceleration = 100;
	public int Friction = 200;

	#endregion
	
	#region Gravity Variables

	public float Gravity = 25f;
    
	public bool IsGrounded = false;
	public bool IsAttacking = false;
	public bool IsHanging = false;
	public bool IsFalling = false;

	#endregion

	#region Other Variables

	public Vector2 Position;
	public Vector2 Velocity;

	#endregion
}