using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

public partial class PlayerCollisionDetector : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
	public override void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerCollisionDetection += DetectCollision;
	}
    
	private void DetectCollision(KinematicCollision2D collision)
	{
		var tileMap = _mineGenerationVariables.MineGenView.TileMap;
		if (collision.GetCollider() == tileMap)
		{
			var tilePos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(_playerControllerVariables.Position);
			var playerPos = _mineGenerationVariables.MineGenView.TileMap.LocalToMap(_playerControllerVariables.Position);
			tilePos -= (Vector2I) collision.GetNormal();

			if (tilePos.Y > playerPos.Y)
				_playerControllerVariables.IsGrounded = true;
            
			GD.Print($"tilePos: {tilePos.X}, {tilePos.Y} | PlayerPos: {playerPos.X}, {playerPos.Y}");
		}
	}

}