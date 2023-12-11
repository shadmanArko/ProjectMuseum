using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;

public partial class SaveGame : Node2D
{
	private HttpRequest _httpRequestForSavingGame;
	private bool _savingGame = false;
	public override void _Ready()
	{
		base._Ready();
		_httpRequestForSavingGame = new HttpRequest();
		AddChild(_httpRequestForSavingGame);
		_httpRequestForSavingGame.RequestCompleted += HttpRequestForSavingGameOnRequestCompleted;
	}

	private void HttpRequestForSavingGameOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		_savingGame = false;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (Input.IsActionJustPressed("save"))
		{
			if (!_savingGame)
			{
				_httpRequestForSavingGame.Request(ApiAddress.PlayerApiPath + "SaveData");
				GD.Print("Save Game");
			}
		}
	}
}
