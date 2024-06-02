using System.Threading.Tasks;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ItemCollectionLogSystem;

public partial class LogMessageCard : PanelContainer
{
    [Export] private Label _label;

    public InventoryItem InventoryItem;
    [Export] private int _itemCount;
    
    [Export] private double _finalTimer;
    
    private double _timer;

    public override void _Ready()
    {
        SetProcess(false);
    }

    public void ShowCollectedItem(InventoryItem inventoryItem)
    {
        Visible = true;
        InventoryItem = inventoryItem;
        _label.Text = $"{++_itemCount}x {inventoryItem.Variant}";
        // var panelSize = Size;
        // panelSize.Y = _label.Size.Y + 20;
        // Size = panelSize;
        _timer = 0;
        SetProcess(true);
    }

    public string GetMessage() => _label.Text;

    public void DisplayMessage(string message)
    {
        Visible = true;
        _label.Text = message;
        // var panelSize = Size;
        // panelSize.Y = _label.Size.Y + 20;
        // Size = panelSize;
        _timer = 0;
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        if (_timer >= _finalTimer)
        {
            SetProcess(false);
            Visible = false;
            ReferenceStorage.Instance.LogMessageController.ItemCards.Remove(this);
            QueueFree();
        }
        else
            _timer += delta;
    }
    
}