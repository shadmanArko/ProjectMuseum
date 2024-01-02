using Godot;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class StoryController : Node2D
{
	private HttpRequest _httpRequestForGettingPlayerInfo;

	private int _totalStoryNumber = 15;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpRequestForGettingPlayerInfo = new HttpRequest();
		AddChild(_httpRequestForGettingPlayerInfo);
		_httpRequestForGettingPlayerInfo.RequestCompleted += HttpRequestForGettingPlayerInfoOnRequestCompleted;
		_httpRequestForGettingPlayerInfo.Request(ApiAddress.PlayerApiPath + "GetPlayerInfo");
		
		MuseumActions.StorySceneEnded += StorySceneEnded;
	}

	private void HttpRequestForGettingPlayerInfoOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print( "Player info " +jsonStr);
		var playerInfo = JsonSerializer.Deserialize<PlayerInfo>(jsonStr);
		var nextStoryNumber = playerInfo.CompletedStoryScene + 1;
		if (nextStoryNumber<= _totalStoryNumber)
		{
			MuseumActions.PlayStoryScene?.Invoke(nextStoryNumber);
		}
		
	}

	private async void StorySceneEnded(int sceneNumber)
	{
		if (sceneNumber <= _totalStoryNumber)
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
