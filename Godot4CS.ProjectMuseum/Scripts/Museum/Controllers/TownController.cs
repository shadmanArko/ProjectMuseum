using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class TownController : Node2D
{
	[Export] private Camera2D _camera2D;
	[Export] private Node2D _townScene;

	private bool _townMapEnabled;

	private Vector2 _cameraStartZoom;
	private Vector2 _cameraStartPosition;
	
	private Vector2 _cameraLastZoom;
	private Vector2 _cameraLastPosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_cameraStartZoom = _camera2D.Zoom;
		_cameraStartPosition = _camera2D.Position;
		MuseumActions.OnTownMapButtonClicked += EnableTownMap;
		// DisableTownMap();
	}

	private void EnableTownMap()
	{
		_cameraLastZoom = _camera2D.Zoom;
		_cameraLastPosition = _camera2D.Position;
		_camera2D.Zoom = _cameraStartZoom;
		_camera2D.Position = _cameraStartPosition;
		_townMapEnabled = true;
		_camera2D.ProcessMode = ProcessModeEnum.Disabled;
		_townScene.ProcessMode = ProcessModeEnum.Inherit;
		_townScene.Scale = new Vector2(1, 1);
		_townScene.Position = new Vector2(0, -210);
		_townScene.Visible = true;
	}
	private void DisableTownMap()
	{
		_camera2D.Zoom = _cameraLastZoom;
		_camera2D.Position = _cameraLastPosition;
		_townMapEnabled = false;
		_camera2D.ProcessMode = ProcessModeEnum.Inherit;
		_townScene.ProcessMode = ProcessModeEnum.Disabled;
		_townScene.Visible = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (_townMapEnabled && @event.IsActionPressed("Escape"))
		{
			DisableTownMap();
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnTownMapButtonClicked -= EnableTownMap;

	}
}
