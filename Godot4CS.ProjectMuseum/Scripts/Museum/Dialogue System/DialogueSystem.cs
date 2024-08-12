using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Managers;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

public partial class DialogueSystem : Control
{
	[Export] private float _delayBetweenLetters;
	[Export] private float _delayForFullStop;
	[Export] private float _delayForComma;
	[Export] private float _delayForPause; 
	[Export] private int _currentStorySceneNumber; 
	[Export] private RichTextLabel _dialogueRichTextLabel;
	[Export] private Button _nextDialogueButton;
	[Export] private TextureRect _characterPortrait;
	[Export] private TextureRect _cutsceneArt;
	[Export] private AnimationPlayer _dialogueSystemAnimationPlayer;
	[Export] private Control _dialogueBox;
	private StoryScene _storyScene;
	private int _storyEntryCount = 0;
	private HttpRequest _httpRequestForGettingStory;
	private HttpRequest _httpRequestForCompletingStory;
	private Task _dialogueShowingTask;
	private CancellationTokenSource _cancellationTokenSource;// Called when the node enters the scene tree for the first time.
	private bool _finishedCurrentDialogue = false;
	private string _playerName;
	private Vector2 _slideOutPosition;
	private Vector2 _slideInPosition;
	private PlayerInfo _playerInfo;
	public override async void _Ready()
	{
		// fullDialogue = $"My name is {PLAYER_NAME()} {PAUSE()}";
		// GD.Print(fullDialogue);
		_cancellationTokenSource = new CancellationTokenSource();
		_httpRequestForGettingStory = new HttpRequest();
		_httpRequestForCompletingStory = new HttpRequest();
		AddChild(_httpRequestForGettingStory);
		AddChild(_httpRequestForCompletingStory);
		_httpRequestForGettingStory.RequestCompleted += HttpRequestForGettingStoryOnRequestCompleted;
		_httpRequestForCompletingStory.RequestCompleted += HttpRequestForCompletingStoryOnRequestCompleted;
		MuseumActions.PlayStoryScene += LoadStoryScene;
		MuseumActions.OnPlayerInfoUpdated += OnPlayerInfoUpdated;
		_nextDialogueButton.Pressed += NextDialogueButtonOnPressed;
		// SlideIn();
		//
		// await Task.Delay(3000);
		// SlideOut();
		// await Task.Delay(3000);
		//
		// SlideIn();
		_slideInPosition = new Vector2(_dialogueBox.Position.X, _dialogueBox.Position.Y - _dialogueBox.Size.Y);
		_slideOutPosition = _dialogueBox.Position;
		_playerInfo = SaveLoadService.Load().PlayerInfo;
		_playerName = _playerInfo.Name;
	}

	private void OnPlayerInfoUpdated(PlayerInfo obj)
	{
		_playerInfo = obj;
	}

	private void HttpRequestForCompletingStoryOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		//GD.Print(jsonStr);
		var playerInfo = JsonSerializer.Deserialize<PlayerInfo>(jsonStr);
		_playerName = playerInfo.Name;
		//GD.Print($"Story completion updated to {playerInfo.CompletedStoryScene}");
	}

	private async void NextDialogueButtonOnPressed()
	{
		GD.Print("next button clicked");
		// if (!_finishedCurrentDialogue) return;
		
		_cancellationTokenSource.Cancel();
		try
		{
			// Wait for the task to complete
			await _dialogueShowingTask;
		}
		catch (TaskCanceledException)
		{
			if (!_finishedCurrentDialogue )
			{
				CompleteDialogueWithoutWaiting(_storyEntryCount);
				//GD.Print("Printing cancelled.");
				return;
			}


		}
		var storySceneEntry = _storyScene.StorySceneEntries[_storyEntryCount];

		 _cancellationTokenSource = new CancellationTokenSource();
		MuseumActions.StorySceneEntryEnded?.Invoke(storySceneEntry.EntryNo);
		if (_storyEntryCount < _storyScene.StorySceneEntries.Count -1)
		{
			_storyEntryCount++;
			ShowNextStoryEntry();
		}
		else
		{
			HandleSceneEnd();
		}
	}

	private void CompleteDialogueWithoutWaiting( int entry)
	{
		_finishedCurrentDialogue = false;
		bool skipIteration = false;
		string tag = "";
		string dialogue = _storyScene.StorySceneEntries[entry].Dialogue;
		_dialogueRichTextLabel.Text = "";
		foreach (var letter in dialogue.ToCharArray())
		{
			// cancellationToken.ThrowIfCancellationRequested();
			
			 if (letter == '[')
			{
				skipIteration = true;
				tag = "";
				continue;
			}else if (letter == ']')
			{
				skipIteration = false;
				if (tag == "PLAYERNAME")
				{
					_dialogueRichTextLabel.Text += _playerName;
				}
				continue;
			}
			if (skipIteration)
			{
				tag += letter;
				continue;
			}
			
			_dialogueRichTextLabel.Text += letter;
			// cancellationToken.ThrowIfCancellationRequested();
			
		}

		_finishedCurrentDialogue = true;

	}

	private async void HandleSceneEnd()
	{
		_httpRequestForCompletingStory.CancelRequest();
		// _httpRequestForCompletingStory.Request(ApiAddress.PlayerApiPath +
		//                                        $"UpdateCompletedStory/{_currentStorySceneNumber}");
		// MuseumReferenceManager.Instance.PlayerInfoServices.UpdateCompletedStory(_currentStorySceneNumber);
		_playerInfo.CompletedStoryScene = _currentStorySceneNumber;
		MuseumActions.OnPlayerInfoUpdated?.Invoke(_playerInfo);
		await SlideOut();
		
		_cutsceneArt.Visible = false;
		Visible = false;
		if (_storyScene.HasTutorial)
		{
			MuseumActions.PlayTutorial?.Invoke(_storyScene.TutorialNumber);
		}
		else
		{
			MuseumActions.StorySceneEnded?.Invoke(_currentStorySceneNumber);
		}
	}

	// public PlayerInfo UpdateCompletedStory(int completedStoryNumber)
	// {
	// 	var playerInfo =  SaveLoadService.Load().PlayerInfo;
	// 	playerInfo!.CompletedStoryScene = completedStoryNumber;
	// 	return  playerInfo;
	// }

	private void LoadAndSetCharacterPortrait()
	{
		// Path to the folder containing your PNG file
		string folderPath = "res://Assets/2D/Sprites/Portraits/";
		var storySceneEntry = _storyScene.StorySceneEntries[_storyEntryCount];

		// Name of your PNG file
		string fileName = $"{storySceneEntry.Speaker} {storySceneEntry.SpeakerEmotion}.png";

		// Combine the folder path and file name to create the full path
		string fullPath = folderPath + fileName;

		// Print the full path to help with debugging
		//GD.Print("Attempting to load texture from path: " + fullPath);

		// Load the texture from the file
		try
		{
			// Load the texture from the file
			Texture2D texture = GD.Load<Texture2D>(fullPath);

			if (texture != null)
			{
				// Set the loaded texture to the TextureRect
				_characterPortrait.Texture = texture;
			}
			else
			{
				_characterPortrait.Texture = null;
				//GD.Print("Failed to load texture: " + fullPath);
			}
		}
		catch (Exception e)
		{
			//GD.Print("Error loading texture: " + e.Message);
		}
	}



	private void LoadStoryScene(int storySceneNumber )
	{
		_currentStorySceneNumber = storySceneNumber;
		var url = ApiAddress.StoryApiPath + $"GetStoryScene/{storySceneNumber}";
		// _httpRequestForGettingStory.CancelRequest();
  //       _httpRequestForGettingStory.Request(url);
        _storyScene = GetByScene(storySceneNumber);
        AfterGettingStory();
	}
	
	private StoryScene GetByScene(int sceneNo)
	{
		var storyScenesJson = Godot.FileAccess.Open("res://Game Data/StoryData/StoryScene.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
		var storyScenes = JsonSerializer.Deserialize<List<StoryScene>>(storyScenesJson);
		var storyScene = storyScenes!.FirstOrDefault(story => story.SceneNo == sceneNo);
		return storyScene;
	}


	private async void HttpRequestForGettingStoryOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		 string jsonStr = Encoding.UTF8.GetString(body);
		 //GD.Print(jsonStr);
		 _storyScene = JsonSerializer.Deserialize<StoryScene>(jsonStr);
		 AfterGettingStory();
	}

	private void AfterGettingStory()
	{
		_storyEntryCount = 0;
		SlideIn();
		ShowNextStoryEntry();
	}

	private async void SlideIn()
	{
		var startPosition = _dialogueBox.Position;
		Vector2 targetPosition = _slideInPosition;
		float duration = 1.0f;
		double elapsed = 0.0f;

		while (elapsed < duration)
		{
			elapsed +=  GetProcessDeltaTime();
			float t = (float)elapsed / duration ;
			_dialogueBox.Position = startPosition.Lerp(targetPosition, t);
			await ToSignal(GetTree().CreateTimer(0.01f), "timeout"); // wait for a short time before continuing the loop
		}

		_dialogueBox.Position = targetPosition; // ensure it ends exactly at the target position

		
	}
	private async Task SlideOut()
	{
		var startPosition = _dialogueBox.Position;
		Vector2 targetPosition = _slideOutPosition;
		float duration = 1.0f;
		double elapsed = 0.0f;

		while (elapsed < duration)
		{
			elapsed +=  GetProcessDeltaTime();
			float t = (float)elapsed / duration ;
			_dialogueBox.Position = startPosition.Lerp(targetPosition, t);
			await ToSignal(GetTree().CreateTimer(0.01f), "timeout"); // wait for a short time before continuing the loop
		}

		_dialogueBox.Position = targetPosition; // ensure it ends exactly at the target position

	}
	private void ShowNextStoryEntry()
	{
		Visible = true;
		LoadAndSetCharacterPortrait();
		LoadAndSetCutsceneArt();
		MuseumActions.StorySceneEntryStarted?.Invoke(_storyScene.StorySceneEntries[_storyEntryCount].EntryNo);
		_dialogueShowingTask = ShowDialogue(_storyEntryCount, _cancellationTokenSource.Token);
	}

	private void LoadAndSetCutsceneArt()
	{
		var storySceneEntry = _storyScene.StorySceneEntries[_storyEntryCount];
		if (!storySceneEntry.HasCutscene)
		{
			_cutsceneArt.Visible = false;
			return;
		}
		else
		{
			_cutsceneArt.Visible = true;
		}

		if (!storySceneEntry.HasCutsceneArt)
		{
			_cutsceneArt.Texture = null;
			return;
		}
		// Path to the folder containing your PNG file
		string folderPath = "res://Assets/2D/Sprites/Illustrations/";
		
		// Name of your PNG file
		string fileName = $"{storySceneEntry.IllustrationName}.png";

		// Combine the folder path and file name to create the full path
		string fullPath = folderPath + fileName;

		// Load the texture from the file
		try
		{
			// Load the texture from the file
			Texture2D texture = GD.Load<Texture2D>(fullPath);

			if (texture != null)
			{
				// Set the loaded texture to the TextureRect
				_cutsceneArt.Texture = texture;
			}
			else
			{
				_cutsceneArt.Texture = null;
				//GD.Print("Failed to load cutscene texture: " + fullPath);
			}
		}
		catch (Exception e)
		{
			//GD.Print("Error loading cutscene texture: " + e.Message);
		}
	}

	
	
	async Task ShowDialogue(int entry, CancellationToken cancellationToken)
	{
		_finishedCurrentDialogue = false;
		bool skipIteration = false;
		string tag = "";
		string dialogue = _storyScene.StorySceneEntries[entry].Dialogue;
		_dialogueRichTextLabel.Text = "";
		foreach (var letter in dialogue.ToCharArray())
		{
			// cancellationToken.ThrowIfCancellationRequested();
			float delayTime = _delayBetweenLetters;
			if (letter == ',')
			{
				delayTime = _delayForComma;
			}else if (letter == '.' || letter == '?')
			{
				delayTime = _delayForFullStop;
			}else if (letter == '[')
			{
				skipIteration = true;
				tag = "";
				continue;
			}else if (letter == ']')
			{
				skipIteration = false;
				if (tag == "PAUSE")
				{
					await Task.Delay((int)(_delayForPause * 1000), cancellationToken);
				}else if (tag == "PLAYERNAME")
				{
					_dialogueRichTextLabel.Text += _playerName;
				}
				continue;
			}
			if (skipIteration)
			{
				tag += letter;
				continue;
			}
			_dialogueRichTextLabel.Text += letter;
			// cancellationToken.ThrowIfCancellationRequested();
			await Task.Delay((int)(delayTime * 1000), cancellationToken);
		}

		_finishedCurrentDialogue = true;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_nextDialogueButton.Pressed -= NextDialogueButtonOnPressed;		
		MuseumActions.PlayStoryScene -= LoadStoryScene;
		MuseumActions.OnPlayerInfoUpdated -= OnPlayerInfoUpdated;
		_httpRequestForGettingStory.RequestCompleted -= HttpRequestForGettingStoryOnRequestCompleted;
	}
}
