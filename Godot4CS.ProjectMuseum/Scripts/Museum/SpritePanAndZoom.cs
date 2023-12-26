using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class SpritePanAndZoom : Control
{
    // Zoom parameters
    [Export] private Sprite2D _sprite2D;
    [Export] private float zoomSpd = 0.05f;
    [Export] private float Minzoom = 0.001f;
    [Export] private float Maxzoom = 3.0f;
    [Export] private Vector2 dragSensitivity = new Vector2(1f, 1f);

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            if (Input.IsActionPressed("ui_right_click"))
            {
                //previous code
                // _sprite2D.Position += ((InputEventMouseMotion)@event).Relative * (dragSensitivity / _sprite2D.Scale);
                // Calculate the new position
                Vector2 newPosition = _sprite2D.Position + ((InputEventMouseMotion)@event).Relative * (dragSensitivity / _sprite2D.Scale);
                ClampPosition(newPosition);
            }
        }

        if (@event is InputEventMouseButton) // for zooming
        {
            var mouseButtonEvent = (InputEventMouseButton)@event;
            if (Input.IsActionPressed("ui_wheel_up"))
            {
                _sprite2D.Scale += new Vector2(zoomSpd, zoomSpd);
            }
            else if (Input.IsActionPressed("ui_wheel_down"))
            {
                _sprite2D.Scale -= new Vector2(zoomSpd, zoomSpd);
            }
            _sprite2D.Scale = new Vector2(Mathf.Clamp(_sprite2D.Scale.X, Minzoom, Maxzoom), Mathf.Clamp(_sprite2D.Scale.Y, Minzoom, Maxzoom));
            ClampPosition(_sprite2D.Position);
        }
    }

    private void ClampPosition(Vector2 newPosition)
    {
        float multiplier = 4;
        // Clamp the position to stay within control boundaries
        // newPosition.X = Mathf.Clamp(newPosition.X, -((Size.X + _sprite2D.GetRect().Size.X * _sprite2D.Scale.X) /
        //                                              multiplier)+ Size.X,
        //     (Size.X + _sprite2D.GetRect().Size.X * _sprite2D.Scale.X)/multiplier);
        // newPosition.Y = Mathf.Clamp(newPosition.Y, -((Size.Y + _sprite2D.GetRect().Size.Y * _sprite2D.Scale.Y) /
        //                                              multiplier)+ Size.Y,
        //     (Size.Y + _sprite2D.GetRect().Size.Y * _sprite2D.Scale.Y)/multiplier);

        Vector2 maxPosition = Size + _sprite2D.GetRect().Size * _sprite2D.Scale;
        // multiplier = multiplier ;
        // Clamp the position to stay within control boundaries
        newPosition.X = Mathf.Clamp(newPosition.X, -(maxPosition.X / multiplier) + Size.X, maxPosition.X / multiplier);
        newPosition.Y = Mathf.Clamp(newPosition.Y, -(maxPosition.Y / multiplier) + Size.Y, maxPosition.Y / multiplier);
        // Set the new position
        _sprite2D.Position = newPosition;
    }
}