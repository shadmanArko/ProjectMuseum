using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class PauseMenuButton : Node
{
	[Export] private TextureRect _pauseMenu;
	[Export] private Button _pauseButton;

	private void OnPauseButtonPressed()
	{
		if (GetTree().GetCurrentScene().GetName() == "MineScene")
		{
			var pauseManager = ReferenceStorage.Instance.MinePauseManager;
			if(pauseManager.IsPaused) return;
			MineActions.OnGamePaused?.Invoke();
		}
		
		_pauseMenu.Visible = true;
		_pauseButton.Visible = false;
		
	}

	private void OnBackButtonPressed()
	{
		_pauseMenu.Visible = false;
		_pauseButton.Visible = true;
		MineActions.OnGameUnpaused?.Invoke();
	}
}
