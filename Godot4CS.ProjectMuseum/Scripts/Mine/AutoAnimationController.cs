using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class AutoAnimationController : Node2D
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables mineGenerationVariables;

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
		_animationController = _playerControllerVariables.Player.animationController;
		_playerController = _playerControllerVariables.Player;
		_playerControllerVariables.Player.Position = new Vector2(250, -58);
		_playerControllerVariables.CanMove = false;
		_playerControllerVariables.IsAffectedByGravity = false;
		_time = 0;
		_playerControllerVariables.Gravity = 0;
		_playerControllerVariables.State = MotionState.Grounded;
		SetProcess(true);
		SetPhysicsProcess(false);
		GD.Print("inside here");
	}

	private Vector2 _newPos = new(420,-58);
	
	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public override void _Process(double delta)
	{
		AutoMoveToPosition();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerControllerVariables.Player.Position = AutoJumpIntoMine((float) _time);
		_time += delta;
		
		if(_playerControllerVariables.Position.Y < _p2.Y+10) return;
		ActionsToPerformAfterPlayerLandsIntoTheMine();
	}
    
	#region Auto Animations

	private void AutoMoveToPosition()
	{
		if(_playerControllerVariables.Player.Position.X <= _newPos.X)
		{
			_animationController.Play("run");
			_playerControllerVariables.Player.Translate(new Vector2(0.05f,0));
		}
		else
		{
			var cell = mineGenerationVariables.GetCell(new Vector2I(24, 0));
			var cellSize = mineGenerationVariables.Mine.CellSize;
			_p0 = _playerControllerVariables.Player.Position;
			_p2 = new Vector2(cell.PositionX * cellSize, cell.PositionY * cellSize);	//_playerControllerVariables.Player.Position + new Vector2(60, 0);
			_p1 = new Vector2((_p0.X + _p2.X) / 2, _p0.Y - 75);

			SetProcess(false);
			SetPhysicsProcess(true);
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

	private void ActionsToPerformAfterPlayerLandsIntoTheMine()
	{
		SetProcess(false);
		SetPhysicsProcess(false);
		_playerControllerVariables.CanMove = true;
		_playerControllerVariables.IsAffectedByGravity = true;
		_playerControllerVariables.Gravity = 25f;
	
		//TODO: TURN ON THIS IF YOU WANT TUTORIALS
		ReferenceStorage.Instance.MineTutorial.PlayMineTutorials();
		
		//TODO: For testing purposes turn off tutorial and turn these on
		// _playerControllerVariables.CanMoveLeftAndRight = true;
		// _playerControllerVariables.CanAttack = true;
		// _playerControllerVariables.CanBrush = true;
		// _playerControllerVariables.CanDig = true;
		// _playerControllerVariables.CanToggleClimb = true;
		ReferenceStorage.Instance.MineTimeSystem.PlayTimer();
	}
}