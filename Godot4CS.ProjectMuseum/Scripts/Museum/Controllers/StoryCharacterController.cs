using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.Museum.ProfessorScripts;

public partial class StoryCharacterController : Node2D
{
	[Export] private UncontrolledCharacter _storyPlayer;
	[Export] private UncontrolledCharacter _storyProfessor;
	[Export] private UncontrolledCharacter _storyAlex;
	[Export] private UncontrolledCharacter _storyEmily;
	[Export] private CharacterBody2DIsometric _gameCharacter;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.StorySceneEntryEnded += StorySceneEntryEndedPlaying;
		MuseumActions.StorySceneEntryStarted += StorySceneEntryStarted;
		_storyPlayer.Visible = false;
		_storyProfessor.Visible = false;
		_storyAlex.Visible = false;
		_storyEmily.Visible = false;
		_gameCharacter.Visible = true;
		MuseumActions.PlayStoryScene += PlayStoryScene;
	}

	private void PlayStoryScene(int obj)
	{
		if (obj <= 4)
		{
			_storyPlayer.Visible = true;
			_storyProfessor.Visible = true;
			_gameCharacter.Visible = false;
		}
	}

	private async void StorySceneEntryStarted(string obj)
	{
		if (obj == "12a")
		{
			await Task.Delay(1000);
			_storyProfessor.StartFollowingDirection();
		}
		if (obj == "9a")
		{
			_storyProfessor.StartFollowingDirection();
		}
	}



	

	private void StorySceneEntryEndedPlaying(string obj)
	{
		if (obj == "4a")
		{
			_storyPlayer.StartFollowingDirection();	
			_storyProfessor.StartFollowingDirection();
		}

		if (obj == "4d")
		{
			_storyPlayer.Visible = false;
			_gameCharacter.Position = _storyPlayer.Position;
			_gameCharacter.Visible = true;
		}
		if (obj == "6e")
		{
			_storyProfessor.ExitMuseum();
		}

		if (obj == "9b")
		{
			_storyAlex.StartFollowingDirection();	
		}
		if (obj == "12c")
		{
			_storyEmily.StartFollowingDirection();	
		}
		
	}

	private void StorySceneEnded(int obj)
	{
		throw new NotImplementedException();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.StorySceneEntryEnded -= StorySceneEntryEndedPlaying;
		MuseumActions.StorySceneEntryStarted -= StorySceneEntryStarted;
		MuseumActions.PlayStoryScene -= PlayStoryScene;
		
	}
}
