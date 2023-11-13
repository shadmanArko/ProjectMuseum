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

	private bool _autoMoveToPos;
	private bool _jumpIntoMine;

	[Export] private Vector2 p0;
	[Export] private Vector2 p1;
	[Export] private Vector2 p2;

	[Export] private double _time;
	
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

	private Vector2 _newPos = new(420,-60);
	
	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public override void _Process(double delta)
	{
		if(!_autoMoveToPos)
			AutoMoveToPosition();
	}

	private Vector2 Bezier(float t)
	{
		var q0 = p0.Lerp(p1, t);
		var q1 = p1.Lerp(p2, t);
		var r = q0.Lerp(q1, t);
		return r;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(_autoMoveToPos && !_jumpIntoMine)
		{
			_playerController.Position = Bezier((float) _time);
			_time += delta;

			if (_time >= 1.2f)
			{
				_animationController.Play("idle");
				SetProcess(false);
				SetPhysicsProcess(false);
				GD.Print("Added jump");
				_playerControllerVariables.CanMove = true;
				_playerControllerVariables.Gravity = 25f;
			}
		}
		
	}

	#region Auto Animations

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
			p0 = _playerController.Position;
			p2 = _playerController.Position + new Vector2(60, 0);
			p1 = new Vector2((p0.X + p2.X) / 2, p0.Y - 75);
			_autoMoveToPos = true;
		}
	}

	#endregion
}