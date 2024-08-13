using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;
using ProjectMuseum.Models.Artifact_and_Inventory;

public partial class NewGameSetupUi : Control
{
	[Export] public Button StartButton;

	[Export] public LineEdit LineEdit;
	[Export] public OptionButton OptionButton;

	[Export] public CheckButton CheckButton;
	[Export] private Control _warningPanel;
	[Export] private Control LoadingPanel;

	private HttpRequest _httpRequestForNewGameSetUpData;
	private HttpRequest _httpRequestForClearingPreviousDataAndStartingNewGame;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StartButton.Pressed += StartButtonOnPressed;
		
		
		_httpRequestForNewGameSetUpData = new HttpRequest();
		_httpRequestForClearingPreviousDataAndStartingNewGame = new HttpRequest();
		AddChild(_httpRequestForNewGameSetUpData);
		AddChild(_httpRequestForClearingPreviousDataAndStartingNewGame);
		_httpRequestForNewGameSetUpData.RequestCompleted += OnNewGameSetupRequestForNewGameSetUpDataComplete;
		_httpRequestForClearingPreviousDataAndStartingNewGame.RequestCompleted += HttpRequestForClearingPreviousDataAndStartingNewGameOnRequestCompleted;
	}

	private void HttpRequestForClearingPreviousDataAndStartingNewGameOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		SavePlayerInfo();
	}

	private void StartButtonOnPressed()
	{
		if (LineEdit.Text == "")
		{
			GD.Print("No Name");
			_warningPanel.Visible = true;
			return;
		}
		LoadingPanel.SetProcess(true);
		LoadingPanel.Visible = true;
		// _httpRequestForClearingPreviousDataAndStartingNewGame.Request(ApiAddress.PlayerApiPath +
		//                                                               "LoadDataForNewGame");
		var playerInfo = new PlayerInfo();
		playerInfo.Id = "string";
		playerInfo.Name = LineEdit.Text;
		playerInfo.Gender = OptionButton.Text;
		playerInfo.Tutorial = CheckButton.ButtonPressed;
		playerInfo.WakeUpHour = 7;
		playerInfo.ForceSleepHour = 00;
		LoadDataForNewGame(playerInfo);
		LoadMuseumScene();
		// MainMenuReferanceManager.Instance.PlayerInfoServices.
		GD.Print("new game set Up request done");
		
	}
	public void LoadDataForNewGame(PlayerInfo playerInfo)
	{
		SaveData saveData = new SaveData();
		var exhibitsJson = Godot.FileAccess.Open("res://Game Data/Starting Data/exhibit.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
		var museumTileJson = Godot.FileAccess.Open("res://Game Data/Starting Data/museumTile.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
		var displayArtifactsJson = Godot.FileAccess.Open("res://Game Data/Starting Data/displayArtifact.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
		var artifactsStorageJson = Godot.FileAccess.Open("res://Game Data/Starting Data/artifactStorage.json", Godot.FileAccess.ModeFlags.Read).GetAsText();

        
		saveData.Exhibits = JsonSerializer.Deserialize<List<Exhibit>>(exhibitsJson);
		saveData.MuseumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(museumTileJson);
		saveData.DisplayArtifacts = JsonSerializer.Deserialize<DisplayArtifacts>(displayArtifactsJson);
		saveData.ArtifactStorage = JsonSerializer.Deserialize<ArtifactStorage>(artifactsStorageJson);
		saveData.PlayerInfo = playerInfo;
        
		SaveLoadService.Save(saveData);
	}
	private void SavePlayerInfo()
	{
		GD.Print($"Name: {LineEdit.Text}, Gender: {OptionButton.Text}, Tutorial: {CheckButton.ButtonPressed}");
		string body = Json.Stringify(new Godot.Collections.Dictionary
		{
			{ "Id", "string" },
			{ "Name", LineEdit.Text },
			{ "Gender", OptionButton.Text },
			{ "Tutorial", CheckButton.ButtonPressed },
			{"WakeUpHour", 7},
			{"ForceSleepHour", 00}
		});
		string[] headers = { "Content-Type: application/json" };
		Error error = _httpRequestForNewGameSetUpData.Request($"{ApiAddress.UrlPrefix}Player/PostPlayerInfo", headers,
			HttpClient.Method.Post, body);
		if (error != Error.Ok)
		{
			GD.Print("Error not ok");
		}
		else
		{
			GD.Print("Error ok");
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void OnNewGameSetupRequestForNewGameSetUpDataComplete(long result, long responsecode, string[] headers, byte[] body)
	{
		LoadMuseumScene();
	}

	private void LoadMuseumScene()
	{
		GD.Print("wil change scene now");
		GetTree().ChangeSceneToFile("res://Scenes/Museum/Main Scene/Museum.tscn");
	}

	private void OnClinkStartNewGameButton()
	{
		Visible = true;
	}
	public override void _EnterTree()
	{
		base._EnterTree();
		MuseumActions.OnClinkStartNewGameButton += OnClinkStartNewGameButton;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClinkStartNewGameButton -= OnClinkStartNewGameButton;
	}
}
