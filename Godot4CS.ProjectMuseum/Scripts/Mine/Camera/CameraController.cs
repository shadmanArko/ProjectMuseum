using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Camera;

public partial class CameraController : Node2D
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	[Export] private Camera2D _camera;
	[Export] private bool _canFollowPlayer;
	
	public override void _Ready()
	{
		InitializeDiInstaller();
		SubscribeToActions();
	}

	private void InitializeDiInstaller()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerSpawned += SetCameraToFollowPlayer;
	}
    
	public override void _Process(double delta)
	{
		if(_canFollowPlayer)
			CheckCameraPosition();
	}

	public void SetCameraToFollowPlayer()
	{
		GD.Print("SET CAMERA FOLLOW PLAYER CALLED");
		SetCameraLimits();
		_canFollowPlayer = true;
	}

	private void SetCameraLimits()
	{
		var cellSize = _mineGenerationVariables.Mine.CellSize;
		var gridLength = _mineGenerationVariables.Mine.GridLength;
		var gridWidth = _mineGenerationVariables.Mine.GridWidth;

		_camera.LimitLeft = 0;
		_camera.LimitRight = cellSize * gridWidth;
		_camera.LimitTop = -200;
		_camera.LimitBottom = cellSize * gridLength;
		
		GD.Print("Setting Camera limits");
	}

	private void CheckCameraPosition()
	{
		_camera.Position = _playerControllerVariables.Position;
		var upperBoundX = _camera.Position.X + _camera.DragRightMargin;
		var lowerBoundX = _camera.Position.X - _camera.DragLeftMargin;
		var upperBoundY = _camera.Position.Y + _camera.DragTopMargin;
		var lowerBoundY = _camera.Position.Y + _camera.DragBottomMargin;
		
		GD.Print("Checking Camera position");

		var playerPos = _playerControllerVariables.Position;
		if(playerPos.X > upperBoundX || playerPos.X < lowerBoundX || playerPos.Y > upperBoundY || playerPos.Y < lowerBoundY)
			FollowPlayer();
	}

	private void FollowPlayer()
	{
		var cameraPos = _camera.Position;
		var playerPos = _playerControllerVariables.Position;
		var smoothLerpX = Mathf.Lerp(cameraPos.X, playerPos.X, 0.5f);
		var smoothLerpY = Mathf.Lerp(cameraPos.Y, playerPos.Y, 0.5f);
		_camera.Position = new Vector2(smoothLerpX, smoothLerpY);
		
		GD.Print("Following Player");
	}

	public override void _ExitTree()
	{
		MineActions.OnPlayerSpawned -= SetCameraToFollowPlayer;
	}
}