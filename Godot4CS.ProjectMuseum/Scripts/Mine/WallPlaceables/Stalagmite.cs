using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.WallPlaceables;

public partial class Stalagmite : Node2D, IDamagable
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	[Export] private AnimationPlayer _animationPlayer;

	private int _hitPoint;
	private bool _isBroken;

	#region Initializers

	public override void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
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
		MineActions.OnMineCellBroken += CheckIfStalagmiteRootCellBroken;
	}

	#endregion
    
	private void CheckIfStalagmiteRootCellBroken()
	{
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
		var cell = _mineGenerationVariables.GetCell(tilePos);
		var rootCell = _mineGenerationVariables.GetCell(new Vector2I(cell.PositionX, cell.PositionY + 1));
		if(!rootCell.IsBroken) return;

		var cave = _mineGenerationVariables.Mine.Caves.FirstOrDefault(tempCave =>
			tempCave.StalagmiteCellIds.Contains(cell.Id));
		cave?.StalagmiteCellIds.Remove(cell.Id);
		PlayAnimation("stalagmite_broken");
	}

	private void PlayAnimation(string state)
	{
		_animationPlayer.Play(state);
	}

	private void OnBrokenAnimationComplete(string animName)
	{
		if(animName != "stalagmite_broken") return;
		QueueFree();
	}

	private void OnBodyEntered(Node2D body)
	{
		var player = body as PlayerController;
		if(player == null) return;
		if(_playerControllerVariables.IsDead) return;
		HealthSystem.ReducePlayerHealth(10, 100, _playerControllerVariables);
	}

	private void UnsubscribeToActions()
	{
		MineActions.OnMineCellBroken -= CheckIfStalagmiteRootCellBroken;
	}
	public override void _ExitTree()
	{
		UnsubscribeToActions();
		GD.Print("Stalagmite destroyed");
	}

	public void TakeDamage()
	{
		if(_isBroken) return;
		if (_hitPoint <= 0)
		{
			_isBroken = true;
			PlayAnimation("stalagmite_broken");
			GD.Print($"Stalagmite broken. hit point: {_hitPoint}");
		}
		else
		{
			_hitPoint--;
			GD.Print($"Stalagmite damaged. hit point: {_hitPoint}");
		}
	}
}