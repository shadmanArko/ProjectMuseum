using Godot;
using System;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class GuestPopUpUi : Control
{
	[Export] private Button _closingButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnClickGuestAi+= OnClickGuestAi;
		_closingButton.Pressed += ClosingButtonOnPressed;
	}

	private void ClosingButtonOnPressed()
	{
		Visible = false;
	}

	private void OnClickGuestAi(GuestAi obj)
	{
		Visible = true;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickGuestAi-= OnClickGuestAi;
		_closingButton.Pressed -= ClosingButtonOnPressed;
	}
}
