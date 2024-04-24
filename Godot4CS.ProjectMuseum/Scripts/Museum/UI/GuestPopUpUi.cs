using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class GuestPopUpUi : Control
{
	[Export] private Button _closingButton;
	[Export] private Label _guestInterestTags;
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
		if (!obj.insideMuseum)
		{
			Visible = false;
			return;
		}
		
		Visible = true;
		_guestInterestTags.Text = GetCommaSeparatedString(obj.interestedInTags);
	}
	private static string GetCommaSeparatedString(List<string> items)
	{
		if (items == null || items.Count == 0)
			return "";

		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < items.Count - 1; i++)
		{
			sb.Append(items[i]);
			sb.Append(", ");
		}
		sb.Append(items[^1]);

		return sb.ToString();
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickGuestAi-= OnClickGuestAi;
		_closingButton.Pressed -= ClosingButtonOnPressed;
	}
}
