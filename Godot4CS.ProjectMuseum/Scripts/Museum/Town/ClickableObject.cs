using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class ClickableObject : Sprite2D
{
    private bool _mouseOnObject;
    private Color _startColor;
    protected int HoverColorCode = 0xD3D3D3;
    

    public override void _Ready()
    {
        _startColor = Modulate;
        Initialize();
    }

    public override void _Process(double delta)
    {
        ProcessObject(delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_left_click") && _mouseOnObject)
        {
            HandleClick();
        }
    }

    private void OnMouseEntered()
    {
        _mouseOnObject = true;
        AssignColor(HoverColorCode);
    }

    private void OnMouseExit()
    {
        _mouseOnObject = false;
        Modulate = _startColor;
    }

    private void AssignColor(int hexCode)
    {
        int red = (hexCode >> 16) & 0xFF;
        int green = (hexCode >> 8) & 0xFF;
        int blue = hexCode & 0xFF;
        Color modulateColor = new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
        Modulate = modulateColor;
    }

    protected virtual void HandleClick()
    {
        
    }

    protected virtual void Initialize() { }

    protected virtual void ProcessObject(double delta) { }
}