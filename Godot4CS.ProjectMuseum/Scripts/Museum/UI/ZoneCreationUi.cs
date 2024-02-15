using Godot;
using System;
using System.Collections.Generic;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class ZoneCreationUi : Control
{
	[Export] private Label _zoneName;
	[Export] private ColorPickerButton _colorPickerButton;
	[Export] private Button _createZoneButton;
	[Export] private Button _cancelZoneButton;

	private List<string> _selectedTileIds = new List<string>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_createZoneButton.Pressed += DisableZoneCreationUi;
		_cancelZoneButton.Pressed += DisableZoneCreationUi;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void EnableZoneCreationUi()
	{
		Visible = true;
	}

	private void DisableZoneCreationUi()
	{
		Visible = false;
		MuseumActions.OnZoneCreationUiClosed?.Invoke();
	}
	private void OnSelectTilesForZone(List<string> obj)
	{
		EnableZoneCreationUi();
		_selectedTileIds = obj;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		MuseumActions.OnSelectTilesForZone += OnSelectTilesForZone;
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnSelectTilesForZone -= OnSelectTilesForZone;
	}
}
