using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class DialogueSystem : Control
{
	[Export] private float _delayBetweenLetters;
	[Export] private float _delayForFullStop;
	[Export] private float _delayForComma;
	[Export] private float _delayForPause; 
	[Export] private int _storySceneNumber; 
	[Export] private RichTextLabel _dialogueRichTextLabel;
	[Export] private Button _nextDialogueButton;
	[Export] private TextureRect _characterPortrait;
	[Export] private TextureRect _cutsceneArt;
	[Export] private AnimationPlayer _dialogueSystemAnimationPlayer;
	private StoryScene _storyScene;
	private int _storyEntryCount = 0;
	
	private HttpRequest _httpRequestForGettingStory;
	private Task _dialogueShowingTask;
	private CancellationTokenSource _cancellationTokenSource;// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		// fullDialogue = $"My name is {PLAYER_NAME()} {PAUSE()}";
		// GD.Print(fullDialogue);
		_cancellationTokenSource = new CancellationTokenSource();
		_httpRequestForGettingStory = new HttpRequest();
		AddChild(_httpRequestForGettingStory);
		_httpRequestForGettingStory.RequestCompleted += HttpRequestForGettingStoryOnRequestCompleted;
		LoadStoryScene();
		_nextDialogueButton.Pressed += NextDialogueButtonOnPressed;
	}

	private async void NextDialogueButtonOnPressed()
	{
		_cancellationTokenSource.Cancel();
		try
		{
			// Wait for the task to complete
			await _dialogueShowingTask;
		}
		catch (TaskCanceledException)
		{
			GD.Print("Printing canceled.");
		}

		_cancellationTokenSource = new CancellationTokenSource();
		if (_storyEntryCount < _storyScene.StorySceneEntries.Count -1)
		{
			_storyEntryCount++;
			ShowNextStoryEntry();
		}
		else
		{
			_dialogueSystemAnimationPlayer.Play("Slide_Out");
		}
	}

	private void LoadAndSetCharacterPortrait()
	{
		// Path to the folder containing your PNG files
		string folderPath = "res://Assets/2D/Sprites/Characters/";

		// Get all files in the folder
		string[] files = System.IO.Directory.GetFiles(folderPath);

		// Iterate through each file
		foreach (string filePath in files)
		{
			// Load the resource from the file
			Resource resource = ResourceLoader.Load(filePath);

			// Check if the loaded resource is a Texture2D
			if (resource is Texture2D texture)
			{
				// Assume the file name is in the format "Speaker Emotion.png"
				string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
				string[] nameParts = fileName.Split(' ');

				if (nameParts.Length == 2)
				{
					string speaker = nameParts[0];
					string emotion = nameParts[1];

					var storySceneEntry = _storyScene.StorySceneEntries[_storyEntryCount];

					if (storySceneEntry.Speaker == speaker && storySceneEntry.SpeakerEmotion == emotion)
					{
						// Set the loaded texture to the TextureRect
						_characterPortrait.Texture = texture;
						return; // Exit the loop since we found the matching texture
					}
				}
			}
		}

		// If no matching texture is found
		_characterPortrait.Texture = null;
		GD.Print("Matching texture not found in folder: " + folderPath);
	}


	private void LoadStoryScene()
	{
		var url = ApiAddress.StoryApiPath + $"GetStoryScene/{_storySceneNumber}";
		_httpRequestForGettingStory.Request(url);
	}

	private async void HttpRequestForGettingStoryOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		 string jsonStr = Encoding.UTF8.GetString(body);
		 GD.Print(jsonStr);
		 _storyScene = JsonSerializer.Deserialize<StoryScene>(jsonStr);
		 _storyEntryCount = 0;
		 _dialogueSystemAnimationPlayer.Play("Slide_In");
		 ShowNextStoryEntry();
	}

	private void ShowNextStoryEntry()
	{
		LoadAndSetCharacterPortrait();
		// LoadAndSetCutsceneArt();
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
		string folderPath = "res://Assets/2D/Sprites/Cutscenes/";
		
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
				GD.Print("Failed to load cutscene texture: " + fullPath);
			}
		}
		catch (Exception e)
		{
			GD.Print("Error loading cutscene texture: " + e.Message);
		}
	}

	
	
	async Task ShowDialogue(int entry, CancellationToken cancellationToken)
	{
		
		bool skipIteration = false;
		string tag = "";
		string dialogue = _storyScene.StorySceneEntries[entry].Dialogue;
		_dialogueRichTextLabel.Text = "";
		foreach (var letter in dialogue.ToCharArray())
		{
			cancellationToken.ThrowIfCancellationRequested();
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
				}
				continue;
			}
			if (skipIteration)
			{
				tag += letter;
				continue;
			}
			_dialogueRichTextLabel.Text += letter;
			cancellationToken.ThrowIfCancellationRequested();
			await Task.Delay((int)(delayTime * 1000), cancellationToken);
		}
	}
	
}
