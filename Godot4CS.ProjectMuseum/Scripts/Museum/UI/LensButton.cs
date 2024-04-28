using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class LensButton : Button
{
	[Export] private Control _lensPanel;
	[Export] private Button _artifactsLens;

	private bool _panelOn = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnPressed;
		_artifactsLens.Pressed += ArtifactsLensOnPressed;
	}

	private void ArtifactsLensOnPressed()
	{
		MuseumActions.OnClickArtifactsLensButton?.Invoke();
	}

	private void OnPressed()
	{
		_panelOn = !_panelOn;
		_lensPanel.Visible = _panelOn;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Pressed -= OnPressed;
		_artifactsLens.Pressed -= ArtifactsLensOnPressed;

	}
}
