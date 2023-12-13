using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.UI;

public partial class PopUpUi : Control
{
	[Export] private RichTextLabel _popUpText;

	[Export] private Button _yesButton;
	[Export] private Button _noButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_yesButton.Pressed += YesButtonOnPressed;
		_noButton.Pressed += NoButtonOnPressed;
		MuseumActions.OnNeedOfPopUpUi += SetPopup;
	}

	private void NoButtonOnPressed()
	{
		MuseumActions.OnClickNoOfPopUpUi?.Invoke();
		Visible = false;
	}

	private void YesButtonOnPressed()
	{
		MuseumActions.OnClickYesOfPopUpUi?.Invoke();
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void SetPopup(string text)
	{
		_popUpText.Text = text;
		Visible = true;

	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_yesButton.Pressed -= YesButtonOnPressed;
		_noButton.Pressed -= NoButtonOnPressed;
		MuseumActions.OnNeedOfPopUpUi -= SetPopup;
	}
}