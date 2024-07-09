using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Misc.GradientSettings;

public partial class MineGradientController : Node2D
{
    [Export] private Sprite2D _topGradSprite;
    [Export] private Sprite2D _bottomGradSprite;

    [Export] private int _topGradientVisibleStartPosition;
    [Export] private int _topGradientVisibleEndPosition;

    [Export] private int _bottomGradientVisibleStartPosition;
    [Export] private int _bottomGradientVisibleEndPosition;
    
    [Export] private Vector2 _topGradOffset;
    [Export] private Vector2 _bottomGradOffset;

    private int _topLimit;
    private int _bottomLimit;
    private int _rightLimit;
    private int _leftLimit;

    private Camera2D _camera;

    public override void _Ready()
    {
        InitializeDiInstaller();
        SubscribeToActions();
        _camera = ReferenceStorage.Instance.CameraController.GetCamera();
        _topLimit = _camera.LimitTop;
        _bottomLimit = _camera.LimitBottom;
        _leftLimit = _camera.LimitLeft;
        _rightLimit = _camera.LimitRight;
        GD.Print($"Camera viewport Size: {_camera.GetViewportRect().Size}");
        var viewportSize = _camera.GetViewportRect().Size;
        _topGradOffset = new Vector2(viewportSize.X / 16, viewportSize.Y / 9);
        _topGradOffset = new Vector2(viewportSize.X / 16, viewportSize.Y / 9);
    }

    private void InitializeDiInstaller()
    {
        
    }

    private void SubscribeToActions()
    {
        MineActions.OnCameraPositionChanged += ChangeUpperGradientPosition;
        MineActions.OnCameraPositionChanged += ChangeLowerGradientPosition;
    }

    private void ChangeUpperGradientPosition()
    {
        if (_camera.Position.Y >= _topGradientVisibleStartPosition)
        {
            var difference = _topGradientVisibleEndPosition - _camera.Position.Y;

            if (difference > 0)
            {
                var valueChange = difference / 10f;
                var valuePerUnit = 1 - valueChange * 0.1f; //per unit
                var alphaVal = Mathf.Clamp(valuePerUnit, 0, 1f);
                var whiteColor = Colors.White;
                whiteColor.A = alphaVal;
                _topGradSprite.Modulate = whiteColor;
            }
            else
            {
                _topGradSprite.Modulate = Colors.White;
            }
        }
        else
        {
            var zeroAlphaColor = new Color(0, 0, 0, 0);
            _topGradSprite.Modulate = zeroAlphaColor;
        }
        
        var viewportSize = _camera.GetViewportRect().Size;
        _topGradOffset = new Vector2(viewportSize.X / 16, viewportSize.Y / 9);
        _topGradSprite.Position = _camera.GlobalPosition - _topGradOffset;
    }

    private void ChangeLowerGradientPosition()
    {
        if (_camera.Position.Y >= _bottomGradientVisibleStartPosition)
        {
            var difference = _bottomGradientVisibleEndPosition - _camera.Position.Y;

            if (difference > 0)
            {
                var valueChange = difference / 10f;
                var valuePerUnit = 1 - valueChange * 0.1f; //per unit
                var alphaVal = Mathf.Clamp(valuePerUnit, 0, 1f);
                var whiteColor = Colors.White;
                whiteColor.A = alphaVal;
                _bottomGradSprite.Modulate = whiteColor;
            }
            else
            {
                _bottomGradSprite.Modulate = Colors.White;
            }
        }
        else
        {
            var zeroAlphaColor = new Color(0, 0, 0, 0);
            _bottomGradSprite.Modulate = zeroAlphaColor;
        }
        
        var viewportSize = _camera.GetViewportRect().Size;
        _bottomGradOffset = new Vector2(viewportSize.X / 16, viewportSize.Y / 9);
        _bottomGradSprite.GlobalPosition = _camera.GlobalPosition + _bottomGradOffset;

    }
    
    private void UnsubscribeToActions()
    {
        MineActions.OnCameraPositionChanged -= ChangeUpperGradientPosition;
        MineActions.OnCameraPositionChanged -= ChangeLowerGradientPosition;
    }

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}