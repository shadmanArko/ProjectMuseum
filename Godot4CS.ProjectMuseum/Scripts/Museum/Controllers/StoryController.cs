using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class StoryController : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// MuseumActions.PlayStoryScene?.Invoke(1);
		MuseumActions.StorySceneEnded += StorySceneEnded;
	}

	private async void StorySceneEnded(int sceneNumber)
	{
		if (sceneNumber < 4)
		{
			MuseumActions.PlayStoryScene(++sceneNumber);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.StorySceneEnded -= StorySceneEnded;

	}
}
