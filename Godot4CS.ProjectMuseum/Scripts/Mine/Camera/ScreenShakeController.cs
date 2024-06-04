using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Camera;

public partial class ScreenShakeController : Node2D
{
    private Random _random = new();
    private Tween _tween;
    private Camera2D _camera;
    [Export] private float _magnitude = 10;

    public override void _Ready()
    {
        _random = new Random();
        _tween = CreateTween();
        _camera = ReferenceStorage.Instance.CameraController.GetCamera();
    }

    public void VerticalCameraShake()
    {
        var offset = new Vector2(0, _random.Next(-1, 1) * _magnitude);
        // _tween.TweenProperty(_camera, _camera.SceneFilePath, ) 
            Tween.InterpolateValue(_camera.Position, offset, 1, 1, Tween.TransitionType.Linear, Tween.EaseType.In);
        GD.PrintErr("SHAKING SCREEN");
    }
}