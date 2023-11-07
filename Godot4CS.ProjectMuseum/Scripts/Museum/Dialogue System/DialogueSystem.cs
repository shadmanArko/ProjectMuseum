using Godot;
using System;
using System.Threading.Tasks;

public partial class DialogueSystem : Control
{
	[Export] private float _delayBetweenLetters;
	[Export] private float _delayForFullStop;
	[Export] private float _delayForComma;
	[Export] private float _delayForPause;
	[Export] private RichTextLabel _dialogueRichTextLabel;
	public string fullDialogue = "Time at university was over. [PAUSE] Although unsure about what to do next, a gut feeling made clear that dropping by Professor Eucalyptus's office would yield something useful. ";
	public string playerName = $"Life";
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		// fullDialogue = $"My name is {PLAYER_NAME()} {PAUSE()}";
		// GD.Print(fullDialogue);
		await ShowDialogue();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
	async Task ShowDialogue()
	{
		bool skipIteration = false;
		string tag = "";
		_dialogueRichTextLabel.Text = "";
		foreach (var letter in fullDialogue.ToCharArray())
		{
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
					await Task.Delay((int)(_delayForPause * 1000));
				}
				continue;
			}
			if (skipIteration)
			{
				tag += letter;
				continue;
			}
			_dialogueRichTextLabel.Text += letter;
			await Task.Delay((int)(delayTime * 1000));
		}
	}

	string PAUSE()
	{
		return "PAUSE!";
	}
	
	string PLAYER_NAME()
	{
		return playerName;
	}
}
