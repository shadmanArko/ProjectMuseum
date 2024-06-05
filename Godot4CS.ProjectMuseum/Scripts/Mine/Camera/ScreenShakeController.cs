using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Camera;

public partial class ScreenShakeController : Node2D
{
    private RandomNumberGenerator _random;
    private Tween _tween;
    private Camera2D _camera;
    [Export] private float _magnitude = 10;

    // public override void _Ready()
    // {
    //     _random = new RandomNumberGenerator();
    //     _tween = CreateTween();
    //     _camera = ReferenceStorage.Instance.CameraController.GetCamera();
    // }
    //
    // public void VerticalCameraShake()
    // {
    //     var offset = new Vector2(0, _random.RandfRange(-1f, 1f) * _magnitude);
    //     // _tween.TweenProperty(_camera,)
    //     GD.PrintErr("SHAKING SCREEN");
    // }
    //
    // public void Shake(float amount)
    // {
    //     var defaultPos = _camera.Position;
    //     var offsetPosition = new Vector2(0, _random.RandfRange(-1f,1f)) * _magnitude + defaultPos;
    //     _camera.Position
    // }
}