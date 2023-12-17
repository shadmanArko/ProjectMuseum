using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Controllers;

public partial class TutorialController : ColorRect
{
	[Export] private RichTextLabel _tutorialBody;

	public override void _Ready()
	{
		base._Ready();
		MuseumActions.OnTutorialUpdated += OnTutorialUpdated;
		MuseumActions.OnTutorialEnded += OnTutorialEnded;
	}

	private void OnTutorialEnded()
	{
		Visible = false;
	}

	private void OnTutorialUpdated(string tutorialBodyText)
	{
		Visible = true;
		_tutorialBody.Text = tutorialBodyText;
	}

	public override void _ExitTree()
	{
		MuseumActions.OnTutorialUpdated -= OnTutorialUpdated;
	}
}