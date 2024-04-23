using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames.LeftRightMouseMiniGame;

public partial class LeftRightMouseMiniGame : Node2D
{
    [Export] private Label _timerLabel;
    [Export] private Label _progressLabel;
    [Export] private TextureProgressBar _textureProgressBar;

    [Export] private int _finalValue;
    [Export] private int _progressValue;

    [Export] private int _successPoints;
    [Export] private int _failPoints;

    [Export] private double _initialCountDownValue;
    [Export] private double _countDownTimer;
    [Export] private double _timer;
    [Export] private double _timeReduceInterval;

    [Export] private Vector2 _lastRegisteredMousePos;
    [Export] private bool _isMovingLeft = true;

    [Export] private bool _miniGameWon;

    public override void _Ready()
    {
        _timer = 2;
        _miniGameWon = false;
        SetProcess(true);
        _lastRegisteredMousePos = GetGlobalMousePosition();
    }

    public override void _PhysicsProcess(double delta)
    {
        _timerLabel.Text =
            $"Time Remaining:   00:{Mathf.Ceil(Mathf.Clamp(_countDownTimer, 0, _initialCountDownValue)):g00}";
        if (_countDownTimer <= 0)
        {
            if (_progressValue >= _finalValue)
            {
                _progressValue = _finalValue;
                _progressLabel.Text = $"{_progressValue}%";
                _miniGameWon = true;
            }
            else
            {
                _progressLabel.Text = $"{_progressValue}%";
                _miniGameWon = false;
            }

            DestroyScene();
        }
        else
        {
            _countDownTimer -= delta;

            if (_progressValue >= _finalValue)
            {
                _miniGameWon = true;
                GD.Print("Alt Tap INSIDE ELSE");
                DestroyScene();
            }
        }

        if (_progressValue <= 0)
            _progressValue = 0;

        CheckMouseUpDownMovement();

        if (_timer > 0)
        {
            _timer -= delta;
        }
        else
        {
            _progressValue -= _failPoints;
            _timer = _timeReduceInterval;
        }

        _progressLabel.Text = $"{Mathf.Clamp(Mathf.CeilToInt(_progressValue), 0, _finalValue)}%";
        _textureProgressBar.Value = Mathf.Clamp(Mathf.CeilToInt(_progressValue), 0, _finalValue);
    }

    private void CheckMouseUpDownMovement()
    {
        if (_isMovingLeft)
        {
            if (GetGlobalMousePosition().X > _lastRegisteredMousePos.X)
            {
                GD.Print("moving right");
                _isMovingLeft = false;
                _progressValue += _successPoints;
            }
            else if (GetGlobalMousePosition().X < _lastRegisteredMousePos.X)
            {
                GD.Print("moving left");
            }
        }
        else
        {
            if (GetGlobalMousePosition().X < _lastRegisteredMousePos.X)
            {
                GD.Print("moving left");
                _isMovingLeft = true;
                _progressValue += _successPoints;
            }
            else
            {
                GD.Print("moving right");
            }
        }

        _lastRegisteredMousePos = GetGlobalMousePosition();
    }

    private void DestroyScene()
    {
        SetProcess(false);
        Visible = false;
        if (_miniGameWon)
            MineActions.OnMiniGameWon?.Invoke();
        else
            MineActions.OnMiniGameLost?.Invoke();

        GD.Print("Process Stopped");
        QueueFree();
    }
}