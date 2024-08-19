using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.WallPlaceables;

public partial class Stalactite : RigidBody2D, IDamageable
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
	
	private void CheckIfStalactiteRootCellBroken(Vector2I tilePos)
	{
		if (_isFalling) return;
		var stalactitePos = _mineGenerationVariables.MineGenView.LocalToMap(Position);
		var stalactiteCell = _mineGenerationVariables.GetCell(stalactitePos);
		var rootCellPos = new Vector2I(stalactiteCell.PositionX, stalactiteCell.PositionY - 1);
		if(rootCellPos != tilePos) return;
		var rootCell = _mineGenerationVariables.GetCell(rootCellPos);
		if(!rootCell.IsBroken) return;

		var cave = _mineGenerationVariables.Mine.Caves.FirstOrDefault(tempCave =>
			tempCave.StalagmiteCellIds.Contains(stalactiteCell.Id));
		cave?.StalagmiteCellIds.Remove(stalactiteCell.Id);
		stalactiteCell.HasCellPlaceable = false;
		PlayAnimation("stalactite_collapse");
		FreeBlockedCellForPathfinding(stalactitePos);
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
		if (body is IDamageable damageable)
		{
			if(!_isFalling) return;
			damageable.TakeDamage(5);
			QueueFree();
		}
        
		if (body == _mineGenerationVariables.MineGenView.TileMap)
		{
			GD.Print("Collided with tilemap");
			if(!_isFalling) return;
			QueueFree();
		}
	}

	private void FreeBlockedCellForPathfinding(Vector2I stalactiteCellPos)
	{
		var node = _mineGenerationVariables.PathfindingNodes.FirstOrDefault(tempNode =>
			tempNode.TileCoordinateX == stalactiteCellPos.X && tempNode.TileCoordinateY == stalactiteCellPos.Y);
		if(node == null) return;
		node.IsWalkable = true;
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

	public void TakeDamage(int damageValue)
	{
		if(_isBroken) return;
		_hitPoint -= damageValue;
		
		if (_hitPoint <= 0)
		{
			_isBroken = true;
			_isFalling = true;
			PlayAnimation("stalactite_collapse");
			GD.Print($"Stalagmite broken. hit point: {_hitPoint}");
		}
		else
		{
			_hitPoint -= damageValue;
			GD.Print($"Stalagmite damaged. hit point: {_hitPoint}");
		}

	}
}