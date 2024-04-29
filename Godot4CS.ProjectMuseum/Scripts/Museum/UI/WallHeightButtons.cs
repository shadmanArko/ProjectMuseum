using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class WallHeightButtons : Button
{
	[Export] private Control _wallHeightButtonsPanel;
	[Export] private Button _mainWallHeightButton;
	[Export] private Button _midWallHeightButton;
	[Export] private Button _smallWallHeightButton;

	private bool _panelOn = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnPressed;
		_mainWallHeightButton.Pressed += MainWallHeightButtonOnPressed;
		_midWallHeightButton.Pressed += MidWallHeightButtonOnPressed;
		_smallWallHeightButton.Pressed += SmallWallHeightButtonOnPressed;
	}

	private void SmallWallHeightButtonOnPressed()
	{
		MuseumActions.OnClickWallHeightChangeButton?.Invoke(WallHeightEnum.Small);
	}

	private void MidWallHeightButtonOnPressed()
	{
		MuseumActions.OnClickWallHeightChangeButton?.Invoke(WallHeightEnum.Mid);
	}

	private void MainWallHeightButtonOnPressed()
	{
		MuseumActions.OnClickWallHeightChangeButton?.Invoke(WallHeightEnum.Original);

	}
	

	private void OnPressed()
	{
		_panelOn = !_panelOn;
		_wallHeightButtonsPanel.Visible = _panelOn;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Pressed -= OnPressed;
		_mainWallHeightButton.Pressed -= MainWallHeightButtonOnPressed;
		_midWallHeightButton.Pressed -= MidWallHeightButtonOnPressed;
		_smallWallHeightButton.Pressed -= SmallWallHeightButtonOnPressed;

	}
}


