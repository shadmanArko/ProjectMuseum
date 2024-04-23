using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;

public partial class GuestNeedCard : Control
{
	[Export] private ColorRect GuestNeedSlider;

	[Export] private GuestNeedsEnum _guestNeedsEnum;
	[Export] private String _needCorespondentName;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
