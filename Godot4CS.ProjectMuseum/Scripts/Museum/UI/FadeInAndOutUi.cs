using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class FadeInAndOutUi : Control
{
	[Export] private AnimationPlayer _animationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnPlayerSleepAndSavedGame += PlayFadeInAndOut;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	async void PlayFadeInAndOut()
	{
		_animationPlayer.Play("Fade_In");
		await Task.Delay(2000);
		_animationPlayer.Play("Fade_Out");
	}
}
