using Godot;
using System;

public partial class Audio : Node
{
	private int _sfxBusId = AudioServer.GetBusIndex("SFX");
	private int _musicBusId = AudioServer.GetBusIndex("Music");
	private int _masterBusId = AudioServer.GetBusIndex("Master");

	[Export] private HSlider _masterVolumeSlider;
	[Export] private HSlider _sfxVolumeSlider;
	[Export] private HSlider _musicVolumeSlider;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnMasterSliderValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(_masterBusId, value);
		//AudioServer.SetBusMute(_masterBusId, value<0.5);
	}
	
	public void OnMusicSliderValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(_musicBusId, value);
		//AudioServer.SetBusMute(_masterBusId, value<0.5);
	}
	
	public void OnSFXSliderValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(_sfxBusId, value);
		//AudioServer.SetBusMute(_masterBusId, value<0.5);
	}
}
