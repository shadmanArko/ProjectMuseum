using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class GuestNeedCard : Control
{
	[Export] private Godot.ColorRect _guestNeedSlider;

	[Export] public GuestNeedsEnum _guestNeedsEnum;
	[Export] private String _needCorespondentName;
	[Export] private Label _guestNeedName;
	// Called when the node enters the scene tree for the first time.
	private GuestAi _selectedGuestAi;
	public override void _Ready()
	{
		_guestNeedName.Text = _needCorespondentName;
		UpdateSliderValue(50);
		MuseumActions.OnClickGuestAi += OnClickGuestAi;
		MuseumActions.OnGuestAiUpdated += UpdateGuestNeedsValueUi;
	}

	private void OnClickGuestAi(GuestAi obj)
	{
		_selectedGuestAi = obj;
		UpdateGuestNeedsValueUi(obj);
	}

	private void UpdateSliderValue(float value)
	{
		// Normalize the value to be between 0 and 192
		float normalizedValue = (value + 100) / 200f;
    
		// Calculate the new size of the slider
		float newSize = normalizedValue * 192;

		// Update the size of the slider
		_guestNeedSlider.Size = new Vector2(newSize, _guestNeedSlider.Size.Y);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.

	public override void _Process(double delta)
	{
	}
	private void UpdateGuestNeedsValueUi(GuestAi guest)
	{
		if (_selectedGuestAi == null || guest != _selectedGuestAi)
		{
			return;
		}
		if (_guestNeedsEnum== GuestNeedsEnum.Bladder)
		{
			UpdateSliderValue(guest.bladderLevel);
		}
		else if (_guestNeedsEnum== GuestNeedsEnum.Hunger)
		{
			UpdateSliderValue(guest.hungerLevel);
		}
		else if (_guestNeedsEnum== GuestNeedsEnum.Thirst)
		{
			UpdateSliderValue(guest.thirstLevel);
		}
		else if (_guestNeedsEnum== GuestNeedsEnum.Energy)
		{
			UpdateSliderValue(guest.energyLevel);
		}
		else if (_guestNeedsEnum== GuestNeedsEnum.Entertainment)
		{
			UpdateSliderValue(guest.entertainmentLevel);
		}
		else if (_guestNeedsEnum== GuestNeedsEnum.Charge)
		{
			UpdateSliderValue(guest.chargeLevel);
		}
		else if (_guestNeedsEnum== GuestNeedsEnum.InterestInArtifact)
		{
			UpdateSliderValue(guest.interestInArtifactLevel);
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickGuestAi -= OnClickGuestAi;
		MuseumActions.OnGuestAiUpdated -= UpdateGuestNeedsValueUi;

	}
}
