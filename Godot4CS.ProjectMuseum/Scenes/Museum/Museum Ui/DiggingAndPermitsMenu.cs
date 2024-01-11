using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class DiggingAndPermitsMenu : Control
{
	[Export] private Button _closePanelButton;
	[Export] private Button _subcontinentButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_closePanelButton.Pressed += closePanelButtonOnPressed();
		_subcontinentButton.Pressed += SubcontinentButtonOnPressed;
		MuseumActions.OnPlayerGetPlayerInfo+= OnPlayerGetPlayerInfo;

	}

	private void OnPlayerGetPlayerInfo(PlayerInfo obj)
	{
		if (obj.CompletedStoryScene >= 11)
		{
			_subcontinentButton.Visible = false;
		}
	}

	private Action closePanelButtonOnPressed()
	{
		return ()=> Visible = false;
	}

	private void SubcontinentButtonOnPressed()
	{
		MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("SelectedMineSite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_closePanelButton.Pressed -= closePanelButtonOnPressed();
		_subcontinentButton.Pressed -= SubcontinentButtonOnPressed;
		MuseumActions.OnPlayerGetPlayerInfo-= OnPlayerGetPlayerInfo;

	}
}
