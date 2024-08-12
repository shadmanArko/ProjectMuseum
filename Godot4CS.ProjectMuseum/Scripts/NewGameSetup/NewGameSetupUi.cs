using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

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
		MainMenuReferanceManager.Instance.PlayerInfoServices.LoadDataForNewGame(playerInfo);
		LoadMuseumScene();
		// MainMenuReferanceManager.Instance.PlayerInfoServices.
		GD.Print("new game set Up request done");
		
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
