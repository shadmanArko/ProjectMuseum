using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;

public partial class MouseFollowingSprite : Control
{
    public InventoryItem InventoryItem; 
    
    [Export] private TextureRect _textureRect;
    [Export] private Label _counterLabel;

    [Export] private Vector2 _offset;

    public override void _Ready()
    {
        
    }
    
    public override void _Process(double delta)
    {
        var mousePos = GetViewport().GetMousePosition() + _offset;
        Position = mousePos;
    }

    public void ShowMouseFollowSprite(InventoryItem inventoryItem)
    {
        InventoryItem = inventoryItem;
        var texture2D = GD.Load<Texture2D>(InventoryItem.PngPath);
        Visible = true;
        _textureRect.Texture = texture2D;
        _counterLabel.Text = InventoryItem.Stack > 1 ? InventoryItem.Stack.ToString() : "";
        SetProcess(true);
    }

    public void HideFollowSprite()
    {
        Visible = false;
        SetProcess(false);
    }
}