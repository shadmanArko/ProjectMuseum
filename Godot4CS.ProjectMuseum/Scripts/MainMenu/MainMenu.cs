using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;

public partial class MainMenu : Control
{
	[Export] public Button ContinueButton;
	[Export] public Button NewGameButton;
	[Export] public Button OptionsButton;
	[Export] public Button ExitButton;
	[Export] public Button SettingsExitButton;
	[Export] public Control SettingsPanel;

	private HttpRequest _httpRequestForLoadingGame;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ExitButton.Pressed += ExitButtonOnPressed;
		NewGameButton.Pressed += NewGameButtonOnPressed;
		ContinueButton.Pressed += ContinueButtonOnPressed;
		OptionsButton.Pressed += OptionsButtonOnPressed;
		SettingsExitButton.Pressed += SettingsExitButtonOnPressed;
		_httpRequestForLoadingGame = new HttpRequest();
		AddChild(_httpRequestForLoadingGame);
		_httpRequestForLoadingGame.RequestCompleted += HttpRequestForLoadingGameOnRequestCompleted;
	}

	private void SettingsExitButtonOnPressed()
	{
		SettingsPanel.Visible = false;
	}

	private void OptionsButtonOnPressed()
	{
		SettingsPanel.Visible = true;
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
		MuseumActions.OnClinkStartNewGameButton?.Invoke();
		Visible = false;
	}

	private void ExitButtonOnPressed()
	{
		GetTree().Quit();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		ExitButton.Pressed -= ExitButtonOnPressed;
		NewGameButton.Pressed -= NewGameButtonOnPressed;
		ContinueButton.Pressed -= ContinueButtonOnPressed;
		OptionsButton.Pressed -= OptionsButtonOnPressed;
		SettingsExitButton.Pressed -= SettingsExitButtonOnPressed;
	}
}
