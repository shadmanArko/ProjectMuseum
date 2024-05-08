using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;

public partial class MouseFollowingSprite : Control
{
    private InventoryItem _inventoryItem; 
    
    [Export] private TextureRect _textureRect;
    [Export] private Label _counterLabel;

    [Export] private Vector2 _offset;

    public override void _Ready()
    {
        _inventoryItem = null;
    }
    
    public override void _Process(double delta)
    {
        var mousePos = GetViewport().GetMousePosition() + _offset;
        Position = mousePos;
    }

    public void SetCurrentCursorItem(InventoryItem item)
    {
        _inventoryItem = item;
    }

    public InventoryItem GetCurrentCursorInventoryItem()
    {
        return _inventoryItem;
    }

    public void ShowMouseFollowSprite(InventoryItem inventoryItem)
    {
        _inventoryItem = inventoryItem;
        if (inventoryItem == null)
        {
            Visible = false;
            _textureRect.Texture = null;
            _counterLabel.Text = "";
            return;
        }
        var texture2D = GD.Load<Texture2D>(_inventoryItem.PngPath);
        Visible = true;
        _textureRect.Texture = texture2D;
        _counterLabel.Text = _inventoryItem.Stack > 1 ? _inventoryItem.Stack.ToString() : "";
        SetProcess(true);
    }

    public void HideFollowSpriteAndSetInventoryItemToNull()
    {
        Visible = false;
        _inventoryItem = null;
        SetProcess(false);
    }
}