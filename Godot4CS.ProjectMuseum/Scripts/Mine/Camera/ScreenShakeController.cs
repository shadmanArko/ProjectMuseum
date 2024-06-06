using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Camera;

public partial class ScreenShakeController : Node2D
{
    private Camera2D _camera2D;
    
    [Export] private float _shakeAmount;
    private Vector2 _shakeDirection;
    
    [Export] private float _shakeDuration = 0.1f;
    [Export] private float _timer = 0.1f;
    
    private const float MildIntensity = 0.5f;
    private const float ModerateIntensity = 1f;
    private const float HeavyIntensity = 1.5f;

    

    public override void _Ready()
    {
        _camera2D = ReferenceStorage.Instance.CameraController.GetCamera();
        SetProcess(false);
    }
    
    public void ShakeScreen(ShakeIntensity intensity, ShakeDirection direction)
    {
        _shakeAmount = intensity switch
        {
            ShakeIntensity.Mild => MildIntensity,
            ShakeIntensity.Moderate => ModerateIntensity,
            ShakeIntensity.Heavy => HeavyIntensity,
            _ => MildIntensity
        };

        _shakeDirection = direction switch
        {
            ShakeDirection.Vertical => new Vector2(0, (float)GD.RandRange(-_shakeAmount, _shakeAmount)),
            ShakeDirection.Horizontal => new Vector2((float)GD.RandRange(-_shakeAmount, _shakeAmount), 0),
            ShakeDirection.OmniDirectional => new Vector2((float)GD.RandRange(-_shakeAmount, _shakeAmount),
                (float)GD.RandRange(-_shakeAmount, _shakeAmount)),
            _ => new Vector2((float)GD.RandRange(-_shakeAmount, _shakeAmount),
                (float)GD.RandRange(-_shakeAmount, _shakeAmount))
        };
        
        _timer = _shakeDuration;
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        if (_timer > 0.0f)
        {
            _timer -= (float) delta;
            _camera2D.Offset = _shakeDirection;
        }
        else
        {
            _camera2D.Offset = Vector2.Zero;
            SetProcess(false);
        }
    }
}