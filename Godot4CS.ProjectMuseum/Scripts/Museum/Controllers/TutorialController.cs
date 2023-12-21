using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Controllers;

public partial class TutorialController : ColorRect
{
	[Export] private RichTextLabel _tutorialBody;
	[Export] private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		base._Ready();
		MuseumActions.OnTutorialUpdated += OnTutorialUpdated;
		MuseumActions.OnTutorialEnded += OnTutorialEnded;
	}

	private async void OnTutorialEnded()
	{
		_animationPlayer.Play("Slide_Out");
		// Visible = false;
	}

	private async void OnTutorialUpdated(string tutorialBodyText)
	{
		await Task.Delay(1000);
		_animationPlayer.Play("Slide_In");
		Visible = true;
		_tutorialBody.Text = tutorialBodyText;
	}

	public override void _ExitTree()
	{
		MuseumActions.OnTutorialUpdated -= OnTutorialUpdated;
		MuseumActions.OnTutorialEnded -= OnTutorialEnded;
	}
}