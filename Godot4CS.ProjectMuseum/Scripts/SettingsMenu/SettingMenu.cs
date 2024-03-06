using Godot;
using System;

public partial class SettingMenu : Node
{
	[Export] private CanvasItem _settingsMenuCanvasItem;

	[Export] private Button _exitButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_exitButton.Pressed += HideSettingsMenu;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ShowSettingsMenu()
	{
		_settingsMenuCanvasItem.Visible = true;
	}

	public void HideSettingsMenu()
	{
		_settingsMenuCanvasItem.Visible = false;
	}
}
