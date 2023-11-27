using Godot;

public partial class MainMenu : Control
{
	[Export] public Button ContinueButton;
	[Export] public Button NewGameButton;
	[Export] public Button OptionsButton;
	[Export] public Button ExitButton;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ExitButton.Pressed += ExitButtonOnPressed;
		NewGameButton.Pressed += NewGameButtonOnPressed;
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
