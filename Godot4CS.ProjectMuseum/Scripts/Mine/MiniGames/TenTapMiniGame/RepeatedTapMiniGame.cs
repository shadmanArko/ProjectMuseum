using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames.TenTapMiniGame;

public partial class RepeatedTapMiniGame : Node2D
{
    [Export] private Label _titleLabel;
    [Export] private Label _timerLabel;
    [Export] private Label _progressLabel;
    [Export] private Label _tapCounterLabel;
    [Export] private TextureProgressBar _textureProgressBar;

    [Export] private int _finalValue;
    [Export] private int _progressValue;

    [Export] private int _successPoints;
    [Export] private int _failPoints;

    [Export] private double _initialCountDownValue;
    [Export] private double _countDownTimer;
    [Export] private double _timer;
    [Export] private double _timeReduceInterval;

    private List<string> keys = new() { "move_down", "move_left", "move_right", "move_up" };
    [Export] private int _tapCounter;
    [Export] private int _totalNoOfTaps;

    private Random _random;

    [Export] private bool _miniGameWon;
    
    public override void _Ready()
    {
        _random = new Random();
        _timer = 2;
        _miniGameWon = false;
        _currentKeyToTap = PickRandomKey();
        SetProcess(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        _titleLabel.Text = $"Repeatedly tap the key that appears {_currentKeyToTap}";
        _tapCounterLabel.Text = _tapCounter.ToString();
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
                DestroyScene();
            }
        }

        if (_progressValue <= 0)
            _progressValue = 0;

        CheckRepeatedTapButton();

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

    [Export] private string _currentKeyToTap;
    private async void CheckRepeatedTapButton()
    {
        if (_tapCounter >= _totalNoOfTaps)
        {
            _tapCounter = 0;
            for (var i = 0; i < _successPoints; i++)
            {
                await Task.Delay(1);
                _progressValue += 1;
            }
            _currentKeyToTap = PickRandomKey();
        }
        else if(Input.IsActionJustReleased(_currentKeyToTap))
            _tapCounter++;
    }

    private string PickRandomKey()
    {
        var newKeyToTap = keys[_random.Next(0, keys.Count)];
        return newKeyToTap;
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