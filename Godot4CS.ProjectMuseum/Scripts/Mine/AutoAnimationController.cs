using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class AutoAnimationController : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
	
	[Export] private PlayerController _playerController;
	[Export] private AnimationController _animationController;
	public override void _EnterTree()
	{
		InitializeDiReferences();
	}
	
	public override void _Ready()
	{
		_playerController.Position = new Vector2(230, -60);
		_playerControllerVariables.CanMove = false;
		var targetCell = _mineGenerationVariables.Cells[0, _mineGenerationVariables.GridWidth];
		var targetCellPos = new Vector2(targetCell.PositionX, targetCell.PositionY);
		
		var previousTargetCell = _mineGenerationVariables.Cells[0, _mineGenerationVariables.GridWidth - 3];
		var previousTargetPos = new Vector2(previousTargetCell.PositionX, previousTargetCell.PositionY);

		//_newPos = new Vector2(previousTargetPos.X, _playerController.Position.Y);
		SetProcess(true);
	}

	private Vector2 _newPos = new Vector2(420,-60);
	
	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public override void _Process(double delta)
	{
		AutoMoveToPosition();
	}


	#region Auto Animations

	// private void AutoMoveIntoMine()
	// {
	// 	AutoMoveToPosition();
	// }

	private void AutoMoveToPosition()
	{
		
		if(_playerController.Position.X < _newPos.X)
		{
			_playerController.Translate(new Vector2(0.5f,0));
			_animationController.PlayAnimation("walk");
			GD.Print("Player controller:"+_playerController.Position);
			GD.Print("Target controller:"+_newPos);
		}
		else
		{
			GD.Print("Moved to position");
			SetProcess(false);
		}
		
	}

	#endregion
}