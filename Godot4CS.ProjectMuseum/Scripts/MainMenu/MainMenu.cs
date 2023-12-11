using Godot;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;

public partial class MainMenu : Control
{
	[Export] public Button ContinueButton;
	[Export] public Button NewGameButton;
	[Export] public Button OptionsButton;
	[Export] public Button ExitButton;

	private HttpRequest _httpRequestForLoadingGame;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ExitButton.Pressed += ExitButtonOnPressed;
		NewGameButton.Pressed += NewGameButtonOnPressed;
		ContinueButton.Pressed += ContinueButtonOnPressed;
		_httpRequestForLoadingGame = new HttpRequest();
		AddChild(_httpRequestForLoadingGame);
		_httpRequestForLoadingGame.RequestCompleted += HttpRequestForLoadingGameOnRequestCompleted;
	}

	private void HttpRequestForLoadingGameOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		GetTree().ChangeSceneToFile("res://Scenes/Museum/Main Scene/Museum.tscn");
	}

	private void ContinueButtonOnPressed()
	{
		_httpRequestForLoadingGame.Request(ApiAddress.PlayerApiPath + "LoadData");
	}

	private void NewGameButtonOnPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/NewGameSetUp/NewGameSetupUi.tscn");
	}

	private void ExitButtonOnPressed()
	{
		GetTree().Quit();
	}

	
}
