using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class PauseMenuButton : Node
{
	[Export] private TextureRect _pauseMenu;
	[Export] private Button _pauseButton;

	private void OnPauseButtonPressed()
	{
		var pauseManager = ReferenceStorage.Instance.MinePauseManager;
		
		if(pauseManager.IsPaused) return;
		_pauseMenu.Visible = true;
		_pauseButton.Visible = false;
		MineActions.OnGamePaused?.Invoke();
	}

	private void OnBackButtonPressed()
	{
		_pauseMenu.Visible = false;
		_pauseButton.Visible = true;
		MineActions.OnGameUnpaused?.Invoke();
	}
}
