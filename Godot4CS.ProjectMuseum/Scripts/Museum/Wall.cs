using Godot;
using System;
using System.Drawing;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class Wall : Node2D
{
    [Export] private Sprite2D _wallSprite;
    [Export] private Sprite2D _wallPreview;
    [Export] private Texture2D _wallPreviewSprite;
    public string WallId;
    public string TileId;
    private bool _showPreview = false;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private int _maxXPosition = -17;
    private int _maxYPosition = -19;
    public void SetUpWall(MuseumTile museumTile, WallSide wallSide)
    {
        switch (wallSide)
        {
            case WallSide.BackLeft:
                WallId = museumTile.BackLeftWallId;
                break;
            case WallSide.BackRight:
                WallId = museumTile.BackRightWallId;
                break;
            case WallSide.FrontLeft:
                WallId = museumTile.FrontLeftWallId;
                break;
            case WallSide.FrontRight:
                WallId = museumTile.FrontRightWallId;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(wallSide), wallSide, null);
        }
        
        TileId = museumTile.Id;
        Texture2D texture2D = GD.Load<Texture2D>($"res://Assets/2D/Sprites/Wallpapers/{WallId}.png");
        _wallSprite.Texture = texture2D;
        if (museumTile.XPosition == 0 || museumTile.YPosition == 0)
        {
            _wallSprite.Frame = 1;
            _wallPreview.Frame = 1;
        }else if (museumTile.BackLeftWallId!= "" && museumTile.BackRightWallId != "")
        {
            _wallSprite.Frame = 0;
            _wallPreview.Frame = 0;
        }
        else
        {
            _wallSprite.Frame = 2;
            _wallPreview.Frame = 2;
        }
    }
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
        MuseumActions.OnWallpaperSuccessfullyUpdated += OnWallpaperSuccessfullyUpdated;
    }

    private void OnWallpaperSuccessfullyUpdated()
    {
        if (_showedPreviewOnce)
        {
            _wallSprite.Texture = _wallPreviewSprite;
        }
        DisablePreview();
    }

    private void DisablePreview()
    {
        _showPreview = false;
        _showedPreviewOnce = false;
        _wallPreview.Visible = false;
    }

    private void OnClickBuilderCard(BuilderCardType arg1, string arg2)
    {
        if (arg1 != BuilderCardType.Wallpaper)
        {
            _showPreview = false;
            _showedPreviewOnce = false;
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
        ShowPreview();
        // Add your hover effect code here
    }

    private bool _showedPreviewOnce = false;
    private void ShowPreview()
    {
        _wallPreview.Texture = _wallPreviewSprite;
        _wallPreview.Visible = true;
        _showedPreviewOnce = true;
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
        if (_showPreview && !_showedPreviewOnce)
        {
            ShowPreview();
            MuseumActions.OnClickWallForUpdatingWallPaper?.Invoke(TileId);
            // Texture = _wallPreviewSprite;
            // _wallPreview.Visible = false;
        }
        
        
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Input(InputEvent @event)
    {
        // Check if the mouse is over the object
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            if (_wallSprite.GetRect().HasPoint(GetLocalMousePosition()) )
            {
                // Emit the mouse_entered signal
                // _on_Hover();
                
            }
            else
            {
                // Emit the mouse_exited signal
                // _on_Unhover();
            }
        }

        if (Input.IsActionPressed("ui_left_click"))
        {
            if (_wallSprite.GetRect().HasPoint(GetLocalMousePosition()))
            {
                OnClickWall();
            }
        }
        
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.OnPreviewWallpaperUpdated -= OnPreviewWallpaperUpdated;
        MuseumActions.OnClickBuilderCard -= OnClickBuilderCard;
        MuseumActions.OnWallpaperSuccessfullyUpdated -= OnWallpaperSuccessfullyUpdated;

    }
}
