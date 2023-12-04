using Godot;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;

public partial class NewGameSetupUi : Control
{
	[Export] public Button StartButton;

	[Export] public LineEdit LineEdit;
	[Export] public OptionButton OptionButton;

	[Export] public CheckButton CheckButton;
	[Export] private Control _warningPanel;

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
		GD.Print("wil change scene now");
		GetTree().ChangeSceneToFile("res://Scenes/Museum/Main Scene/Museum.tscn");
	}

	private void StartButtonOnPressed()
	{
		if (LineEdit.Text == "")
		{
			GD.Print("No Name");
			_warningPanel.Visible = true;
			return;
		}
		
		GD.Print($"Name: {LineEdit.Text}, Gender: {OptionButton.Text}, Tutorial: { CheckButton.ButtonPressed}" );
		string body = Json.Stringify(new Godot.Collections.Dictionary
		{
			{ "Id", "string" },
			{ "Name", LineEdit.Text },
			{ "Gender", OptionButton.Text },
			{ "Tutorial", CheckButton.ButtonPressed }
			

		});
		string[] headers = { "Content-Type: application/json"};
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
		_httpRequestForClearingPreviousDataAndStartingNewGame.Request(ApiAddress.MuseumApiPath +
		                                                              "StartNewGame");
		GD.Print("new game set Up request done");
	}
}
