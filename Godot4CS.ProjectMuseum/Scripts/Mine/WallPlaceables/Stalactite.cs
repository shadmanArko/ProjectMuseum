using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.WallPlaceables;

public partial class Stalactite : Node2D, IDamagable
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	[Export] private AnimationPlayer _animationPlayer;
	
	private int _hitPoint;
	private bool _isBroken;

	private const float Gravity = 0.8f;
	private bool _isFalling;

	#region Initializers

	public override void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
		_isFalling = false;
		_hitPoint = 1;
		_isBroken = false;
	}
	
	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}
	private void SubscribeToActions()
	{
		MineActions.OnMineCellBroken += CheckIfStalactiteRootCellBroken;
	}

	#endregion
	
	private void CheckIfStalactiteRootCellBroken()
	{
		if (_isFalling)return;
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
		var cell = _mineGenerationVariables.GetCell(tilePos);
		var rootCell = _mineGenerationVariables.GetCell(new Vector2I(cell.PositionX, cell.PositionY - 1));
		if(!rootCell.IsBroken) return;

		var cave = _mineGenerationVariables.Mine.Caves.FirstOrDefault(tempCave =>
			tempCave.StalagmiteCellIds.Contains(cell.Id));
		cave?.StalagmiteCellIds.Remove(cell.Id);
		PlayAnimation("stalactite_collapse");
	}
	
	private void PlayAnimation(string state)
	{
		_animationPlayer.Play(state);
	}

	private void OnCollapseAnimationComplete(string animName)
	{
		if(animName != "stalactite_collapse") return;
		Set("gravity_scale", Gravity);
		_animationPlayer.Play("stalactite_fall");
		_isFalling = true;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body == _playerControllerVariables.Player)
		{
			GD.Print("Collided with player");
			if(_playerControllerVariables.IsDead) return;
			HealthSystem.ReducePlayerHealth(10, _playerControllerVariables);
			if(_isFalling)
				QueueFree();
		}

		if (body == _mineGenerationVariables.MineGenView.TileMap)
		{
			GD.Print("Collided with tilemap");
			if(!_isFalling) return;
			QueueFree();
		}
	}

	private void UnsubscribeToActions()
	{
		MineActions.OnMineCellBroken -= CheckIfStalactiteRootCellBroken;
	}
	public override void _ExitTree()
	{
		UnsubscribeToActions();
		GD.Print("Stalactite destroyed");
	}

	public void TakeDamage()
	{
		if(_isBroken) return;
		if (_hitPoint <= 0)
		{
			_isBroken = true;
			_isFalling = true;
			PlayAnimation("stalactite_collapse");
			GD.Print($"Stalagmite broken. hit point: {_hitPoint}");
		}
		else
		{
			_hitPoint--;
			GD.Print($"Stalagmite damaged. hit point: {_hitPoint}");
		}

	}
}