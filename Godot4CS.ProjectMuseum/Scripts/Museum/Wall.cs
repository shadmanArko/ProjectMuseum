using Godot;
using System;
using System.Drawing;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class Wall : Sprite2D
{
    [Export] private Sprite2D _wallPreview;
    [Export] private Texture2D _wallPreviewSprite;

    private bool _showPreview = false;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Add your code here. For example:
        // GD.Print("Hello, World!");
    }

    // Called every time the node is added to the scene.
    // Initialization here
    public override void _Ready()
    {
        MuseumActions.OnPreviewWallpaperUpdated += OnPreviewWallpaperUpdated;
        MuseumActions.OnClickBuilderCard += OnClickBuilderCard;
    }

    private void OnClickBuilderCard(BuilderCardType arg1, string arg2)
    {
        if (arg1 != BuilderCardType.Wallpaper)
        {
            _showPreview = false;
        }
    }

    private void OnPreviewWallpaperUpdated(Texture2D obj)
    {
        _showPreview = true;
        _wallPreviewSprite = obj;
    }

    // Function called when the mouse enters the object
    private void _on_Hover()
    {
        if (!_showPreview) return;
        
        // GD.Print("Mouse entered!");
        _wallPreview.Texture = _wallPreviewSprite;
        _wallPreview.Visible = true;
        // Add your hover effect code here
    }

    // Function called when the mouse exits the object
    private void _on_Unhover()
    {
        _wallPreview.Visible = false;
        // GD.Print("Mouse exited!");
        // Add your unhover effect code here
    }
    private void OnClickWall()
    {
        
        Texture = _wallPreviewSprite;
        _wallPreview.Visible = false;
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Input(InputEvent @event)
    {
        // Check if the mouse is over the object
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            if (GetRect().HasPoint(GetLocalMousePosition()) )
            {
                // Emit the mouse_entered signal
                _on_Hover();
                
            }
            else
            {
                // Emit the mouse_exited signal
                _on_Unhover();
            }
        }

        if (Input.IsActionPressed("ui_left_click"))
        {
            if (GetRect().HasPoint(GetLocalMousePosition()))
            {
                OnClickWall();
            }
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.OnPreviewWallpaperUpdated -= OnPreviewWallpaperUpdated;

    }
}
