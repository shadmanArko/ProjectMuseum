using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class AutoAnimationController : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	
	public override void _EnterTree()
	{
		InitializeDiReferences();
	}
	
	public override void _Ready()
	{
		
	}
	
	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}

	
	public override void _Process(double delta)
	{
		
	}
	
	#region Auto Animations

	public void AutoMoveIntoMine()
	{
		_playerControllerVariables.CanMove = false;
		
	}

	private void AutoMoveToPosition(Vector2 targetPos)
	{
		
	}

	#endregion
}