using Godot;
using System;

public partial class Tooltip4 : Control
{
	private Label label;
    private VBoxContainer _vBoxContainer;
    private Rect2 _rect;
    

    public override void _Ready()
    {
        //label = GetNode<Label>("Label");
        _vBoxContainer = GetNode<VBoxContainer>("VBoxContainer");
       // Hide();
        _rect = GetRect();
        _rect.Size = new Vector2(_vBoxContainer.GetRect().Size.X + 10, _vBoxContainer.GetRect().Size.Y + 5);
        GD.Print(_rect.Size);
    }

    public void SetText(string text)
    {
        label.Text = text;
       _rect.Size = new Vector2(label.GetRect().Size.X + 10, label.GetRect().Size.Y + 5);
       GD.Print(_rect.Size);
    }

    public override void _Process(double delta)
    {
        var centerPositionOfScreen = GetViewportRect().Size / 2;
        var mousePosition = GetViewport().GetMousePosition();

        if (mousePosition.X < centerPositionOfScreen.X && mousePosition.Y > centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(50, -_rect.Size.Y) ; // todo done
        }
        else if (mousePosition.X < centerPositionOfScreen.X && mousePosition.Y < centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(50, 50) ; //todo done
        }
        else if (mousePosition.X > centerPositionOfScreen.X && mousePosition.Y < centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(-_rect.Size.X, 50) ;
        }
        else
        {
            Position = GetViewport().GetMousePosition() + new Vector2(-_rect.Size.X, -_rect.Size.Y) ; // todo done
        }
        
        // Follow the mouse position
         GD.Print(GetViewportRect().Size/2);
    }
}
