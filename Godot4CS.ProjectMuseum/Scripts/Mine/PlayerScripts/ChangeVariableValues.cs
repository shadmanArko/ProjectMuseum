using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class ChangeVariableValues : Node
{
	private PlayerControllerVariables _playerControllerVariables;
    
	public override void _Ready()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}
    
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("changeValue"))
		{
			_playerControllerVariables.Acceleration--;
		}
	}
}
