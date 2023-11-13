using System.Threading.Tasks;
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
		
		_playerControllerVariables.Gravity = 0;
		SetProcess(true);
	}

	private void SetJumpVariables()
	{
		var pcv = _playerControllerVariables;
		pcv.JumpVelocity = (2 * pcv.JumpHeight) / pcv.JumpTimeToPeak * -1;
		pcv.JumpGravity = (-2 * pcv.JumpHeight) / (pcv.JumpTimeToPeak * pcv.JumpTimeToPeak) * -1;
		pcv.FallGravity = (-2 * pcv.JumpHeight) / (pcv.JumpTimeToDescend * pcv.JumpTimeToDescend) * -1;
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

	private async void AutoMoveToPosition()
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
			JumpIntoMine();
			SetProcess(false);
		}
	}

	private async Task JumpIntoMine()
	{
		_playerController.MoveLocalY(-100);
		await Task.Delay(1000);
		//_playerControllerVariables.CanMove = true;
		GD.Print("Added jump");
	}

	private float GetGravity()
	{
		return _playerController.Velocity.Y < 0 ? _playerControllerVariables.JumpGravity : _playerControllerVariables
			.FallGravity;
	}

	#endregion
}