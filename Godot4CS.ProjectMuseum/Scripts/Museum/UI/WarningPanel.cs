using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class WarningPanel : Control
{
	[Export] private Label _warningText;
	[Export] private float _warningShowingDuration = 5.0f;
	[Export] private AnimationPlayer _animationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animationPlayer.Play("slide_out");

		MuseumActions.OnNeedOfWarning += OnNeedOfWarning;
	}

	private async void OnNeedOfWarning(string warningText)
	{
		_warningText.Text = warningText;
		_animationPlayer.Play("slide_in");
		await Task.Delay((int)_warningShowingDuration * 1000);
		_animationPlayer.Play("slide_out");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnNeedOfWarning -= OnNeedOfWarning;
	}
}
