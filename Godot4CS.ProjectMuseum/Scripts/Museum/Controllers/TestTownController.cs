using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class TestTownController : Node
{
	[Export] private Camera2D _camera2D;
	[Export] private Control _townScene;

	private bool _townMapEnabled;

	private Vector2 _cameraStartZoom;
	private Vector2 _cameraStartPosition;
	
	private Vector2 _cameraLastZoom;
	private Vector2 _cameraLastPosition;

	private Tween _tween;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tween = GetTree().CreateTween().BindNode(this);
		_cameraStartZoom = _camera2D.Zoom;
		_cameraStartPosition = _camera2D.Position;
		_cameraLastZoom = _cameraStartZoom;
		_cameraLastPosition = _cameraStartPosition;
		MuseumActions.OnTownMapButtonClicked += EnableTownMap;
		MuseumActions.OnClickCloseTownUi += DisableTownMap;
		DisableTownMap();
	}

	private void EnableTownMap()
	{
		GD.Print("Enabled Town map");
		_cameraLastZoom = _camera2D.Zoom;
		_cameraLastPosition = _camera2D.Position;
		_camera2D.Zoom = _cameraStartZoom;
		_camera2D.Position = _cameraStartPosition;
		_townMapEnabled = true;
		_camera2D.ProcessMode = ProcessModeEnum.Disabled;
		_townScene.ProcessMode = ProcessModeEnum.Inherit;
		// _townScene.Scale = new Vector2(1, 1);
		// _townScene.Position = new Vector2(0, -210);
		_townScene.Visible = true;
	}
	private void DisableTownMap()
	{
		_tween.TweenProperty(
			_camera2D,
			"position",
			_cameraLastPosition,
			1f // duration in seconds
		);
		_tween.TweenProperty(
			_camera2D,
			"zoom",
			_cameraLastZoom,
			1f // duration in seconds
		);
		// _camera2D.Zoom = _cameraLastZoom;
		// _camera2D.Position = _cameraLastPosition;
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
		MuseumActions.OnClickCloseTownUi -= DisableTownMap;

	}
}
