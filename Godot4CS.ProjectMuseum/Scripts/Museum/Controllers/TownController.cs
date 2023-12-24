using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class TownController : Node2D
{
	[Export] private Camera2D _camera2D;
	[Export] private Node2D _townScene;

	private bool _townMapEnabled;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnTownMapButtonClicked += EnableTownMap;
		DisableTownMap();
	}

	private void EnableTownMap()
	{
		_townMapEnabled = true;
		_camera2D.ProcessMode = ProcessModeEnum.Disabled;
		_townScene.ProcessMode = ProcessModeEnum.Inherit;
		_townScene.Visible = true;
	}
	private void DisableTownMap()
	{
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
