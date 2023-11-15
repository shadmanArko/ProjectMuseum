using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;

public partial class AlternateTapMiniGame : CanvasLayer
{
	[Export] private double _countDownTimer;
	[Export] private double _timer;
	[Export] private double _timeReduceInterval;

	[Export] private Label _timerLabel;
	[Export] private Label _pressButtonLabel;
	[Export] private Label _progressLabel;

	[Export] private TextureProgressBar _textureProgressBar;
	
	[Export] private int _finalValue;
	[Export] private int _progressValue;

	[Export] private int _successPoints;
	[Export] private int _failPoints;
	
	[Export] private bool _isAlternateTapOption;

	[Export] private bool _miniGameWon;

	public override void _Ready()
	{
		_timer = 2;
		_miniGameWon = false;
		SetProcess(true);
	}
    
	public override void _Process(double delta)
	{
		_timerLabel.Text = Mathf.Ceil(Mathf.Clamp(_countDownTimer,0,10)).ToString();
		if (_countDownTimer <= 0)
		{
			if (_progressValue >= _finalValue)
			{
				
				_progressValue = _finalValue;
				_progressLabel.Text = _progressValue.ToString();
				_pressButtonLabel.Text = "Successfully Extracted Artifact";
				_miniGameWon = true;
			}
			else
			{
				_progressLabel.Text = _progressValue.ToString();
				_pressButtonLabel.Text = "Failed to Extract Artifact";
				_miniGameWon = false;
			}
			_ExitTree();
		}
		else
		{
			_countDownTimer -= delta;
		}

		if (_progressValue >= _finalValue)
		{
			_miniGameWon = true;
			_ExitTree();
		}

		if (_progressValue <= 0)
			_progressValue = 0;
		
		CheckAlternateButtonsPressed();
		
		if (_timer > 0)
		{
			_timer -= delta;
		}
		else
		{
			_progressValue -= _failPoints;
			_timer = _timeReduceInterval;
		}

		//_timerLabel.Text = Mathf.CeilToInt( _timer).ToString();
		_pressButtonLabel.Text = _isAlternateTapOption ? "Q" : "E";
		_progressLabel.Text = $"Progress: {Mathf.Clamp(Mathf.CeilToInt(_progressValue), 0,_finalValue)}";
		_textureProgressBar.Value = Mathf.Clamp(Mathf.CeilToInt(_progressValue), 0, _finalValue);
	}

	private void CheckAlternateButtonsPressed()
	{
		if(_isAlternateTapOption)
			CheckQPressed();
		else
			CheckEPressed();
	}

	private void CheckQPressed()
	{
		var isPressed = Input.IsActionJustReleased("PressQ");
		if (!isPressed) return; 
		_progressValue += _successPoints;
		_isAlternateTapOption = !_isAlternateTapOption;
	}

	private void CheckEPressed()
	{
		var isPressed = Input.IsActionJustReleased("PressE");
		if (!isPressed) return; 
		_progressValue += _successPoints;
		_isAlternateTapOption = !_isAlternateTapOption;
	}

	public override void _ExitTree()
	{
		SetProcess(false);
		Visible = false;
		if(_miniGameWon)
			MineActions.OnMiniGameWon?.Invoke();
		else
			MineActions.OnMiniGameLost?.Invoke();
		
		GD.Print("Process Stopped");
	}
}