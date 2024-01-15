using Godot;
using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Godot.Collections;

public partial class MuseumBackgroundMusicPlayer : AudioStreamPlayer
{
	[Export] private Array<AudioStream> _bgMusics;

	private int _currentStreamIndex = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_currentStreamIndex = GD.RandRange(0, _bgMusics.Count - 1);
		Stream = _bgMusics[_currentStreamIndex];
		Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Playing)
		{

			if (_bgMusics.Count > 1)
			{
				var newStreamIndex = GD.RandRange(0, _bgMusics.Count - 1);
				if (newStreamIndex != _currentStreamIndex)
				{
					_currentStreamIndex = newStreamIndex;
					Stream = _bgMusics[_currentStreamIndex];
					Play();
				}
			}
			else
			{
				Stream = _bgMusics.First();
				Play();
			}
		}
	}
}
