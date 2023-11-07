using Godot;
using System;

public partial class CameraPanAndZoom : Camera2D
{
	// Zoom parameters
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
                Position -= ((InputEventMouseMotion)@event).Relative * (dragSensitivity / Zoom);
            }
        }

        if (@event is InputEventMouseButton) // for zooming
        {
            var mouseButtonEvent = (InputEventMouseButton)@event;
            if (Input.IsActionPressed("ui_wheel_up"))
            {
                Zoom += new Vector2(zoomSpd, zoomSpd);
            }
            else if (Input.IsActionPressed("ui_wheel_down"))
            {
                Zoom -= new Vector2(zoomSpd, zoomSpd);
            }
            Zoom = new Vector2(Mathf.Clamp(Zoom.X, Minzoom, Maxzoom), Mathf.Clamp(Zoom.Y, Minzoom, Maxzoom));
        }
    }
}
