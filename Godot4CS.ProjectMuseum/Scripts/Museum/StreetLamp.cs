using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class StreetLamp : Sprite2D
{
	[Export] private PointLight2D _pointLight;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnTimeUpdated += OnTimeUpdated;
	}

	private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
	{
		_pointLight.Enabled = hours >= 18 || hours < 6;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnTimeUpdated -= OnTimeUpdated;
	}
}
