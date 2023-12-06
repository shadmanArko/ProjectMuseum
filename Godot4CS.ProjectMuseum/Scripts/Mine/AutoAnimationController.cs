using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class AutoAnimationController : Node
{
	private PlayerControllerVariables _playerControllerVariables;

	[Export] private PlayerController _playerController;
	[Export] private AnimationController _animationController;

	private bool _autoMoveToPos;
	private bool _jumpIntoMine;

	[Export] private Vector2 _p0;
	[Export] private Vector2 _p1;
	[Export] private Vector2 _p2;

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
		ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public override void _Process(double delta)
	{
		if(!_autoMoveToPos)
			AutoMoveToPosition();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(_autoMoveToPos && !_jumpIntoMine)
		{
			_playerController.Position = AutoJumpIntoMine((float) _time);
			_time += delta;

			if (!(_time >= 1.2f)) return;
			_animationController.Play("idle");
			SetProcess(false);
			SetPhysicsProcess(false);
			_playerControllerVariables.CanMove = true;
			_playerControllerVariables.Gravity = 25f;
		}
		
	}
    
	#region Auto Animations

	private void AutoMoveToPosition()
	{
		if(_playerController.Position.X < _newPos.X)
		{
			_playerController.Translate(new Vector2(0.02f,0));
			_animationController.Play("walk");
		}
		else
		{
			_p0 = _playerController.Position;
			_p2 = _playerController.Position + new Vector2(60, 0);
			_p1 = new Vector2((_p0.X + _p2.X) / 2, _p0.Y - 75);
			_autoMoveToPos = true;
		}
	}
	
	private Vector2 AutoJumpIntoMine(float t)
	{
		var q0 = _p0.Lerp(_p1, t);
		var q1 = _p1.Lerp(_p2, t);
		var r = q0.Lerp(q1, t);
		return r;
	}

	#endregion
}