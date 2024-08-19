using Godot;
using System;

public partial class AudioManager : Node
{
	public void PlayAudio(AudioStream audioStream)
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
}
