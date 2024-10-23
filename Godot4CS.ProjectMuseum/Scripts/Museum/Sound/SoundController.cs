using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public partial class SoundController : Node2D
{
	[Export] private AudioStream _rotateSound;
	[Export] private AudioStream _itemPlacingSound;
	[Export] private AudioStream _footStepSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnItemRotated += OnItemRotated;
		MuseumActions.OnItemPlacedOnTile += OnItemPlacedOnTile;
		MuseumActions.OnPlayerStepped += OnPlayerStepped;
	}

	private void OnPlayerStepped()
	{
		PlayAudio(_footStepSound);
	}

	private void OnItemPlacedOnTile(Item arg1, Vector2 arg2)
	{
		PlayAudio(_itemPlacingSound);
	}

	

	private void OnItemRotated()
	{
		PlayAudio(_rotateSound);
	}
	private void PlayAudio(AudioStream audioStream)
	{
		var instance = new AudioStreamPlayer();
		instance.Stream = audioStream;
		instance.Finished += () => OnAudioStreamFinished(instance);
		AddChild(instance);
		instance.Play();
	}

	private void OnAudioStreamFinished(AudioStreamPlayer audioStreamPlayer)
	{
		audioStreamPlayer.QueueFree();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnItemRotated -= OnItemRotated;
		MuseumActions.OnItemPlacedOnTile -= OnItemPlacedOnTile;
		MuseumActions.OnPlayerStepped -= OnPlayerStepped;

	}
}
