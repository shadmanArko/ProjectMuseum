using Godot;
using System;

public partial class Tooltip4 : Control
{
	private Label label;
    private VBoxContainer _vBoxContainer;
    private MarginContainer _marginContainer;
    private Panel _panel;
    private Rect2 _rect;
    

    public override void _Ready()
    {
        //label = GetNode<Label>("Label");
        _vBoxContainer = GetNode<VBoxContainer>("MarginContainer/VBoxContainer");
        _marginContainer = GetNode<MarginContainer>("MarginContainer");
        _panel = GetNode<Panel>("Panel");
       // label = GetNode<Label>("MarginContainer/VBoxContainer/Label");
         Hide();
        _rect = GetRect();
        _rect.Size = new Vector2(_vBoxContainer.GetRect().Size.X + 10, _vBoxContainer.GetRect().Size.Y + 5);

        _panel.Size = _marginContainer.Size;
        //GD.Print(_rect.Size);
    }

    public void SetText(string text)
    {
        label.Text = text;
       _rect.Size = new Vector2(_vBoxContainer.GetRect().Size.X + 10, _vBoxContainer.GetRect().Size.Y + 5);
       _panel.Size = _marginContainer.Size;
       Show();
       //GD.Print(_rect.Size);
    }

    public void ShowTooltip()
    {
        Show();
    }

    public void HideTooltip()
    {
        Hide();
    }

    public override void _Process(double delta)
    {
        var centerPositionOfScreen = GetViewportRect().Size / 2;
        var mousePosition = GetViewport().GetMousePosition();

        // Bottom Left
        if (mousePosition.X < centerPositionOfScreen.X && mousePosition.Y > centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(50, -_rect.Size.Y) ; 
        }
        
        // Top Left
        else if (mousePosition.X < centerPositionOfScreen.X && mousePosition.Y < centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(50, 50) ; 
        }
        
        // Top Right
        else if (mousePosition.X > centerPositionOfScreen.X && mousePosition.Y < centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(-_rect.Size.X-50, 50) ;
        }
        
        // Bottom Right
        else
        {
            Position = GetViewport().GetMousePosition() + new Vector2(-_rect.Size.X-50, -_rect.Size.Y-75) ; 
        }
        
        // Follow the mouse position
        //GD.Print(GetViewportRect().Size/2);
    }
}
