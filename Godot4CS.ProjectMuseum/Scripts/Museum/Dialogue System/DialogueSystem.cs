using Godot;
using System;

public partial class DialogueSystem : Control
{
	public string fullDialogue = "";
	public string playerName = $"Life";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fullDialogue = $"My name is {PLAYER_NAME()} {PAUSE()}";
		GD.Print(fullDialogue);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
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
