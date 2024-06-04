using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ItemCollectionLogSystem;

public partial class LogMessageController : PanelContainer
{
    [Export] private VBoxContainer _vBoxContainer;
    public List<LogMessageCard> ItemCards;

    private string _itemCardScenePath;

    public override void _Ready()
    {
        SubscribeToActions();
        ItemCards = new List<LogMessageCard>();
        _itemCardScenePath = ReferenceStorage.Instance.ItemCardScenePath;
    }

    private void SubscribeToActions()
    {
        MineActions.OnCollectItemDrop += OnItemCollectedByPlayer;
    }

    private void OnItemCollectedByPlayer(InventoryItem item)
    {
        var collectedItem = ItemCards.FirstOrDefault(tempItem => tempItem.InventoryItem.Variant == item.Variant);
        if (collectedItem != null)
        {
            collectedItem.ShowCollectedItem(item);
            return;
        }
        
        var itemCard = ResourceLoader.Load<PackedScene>(_itemCardScenePath).Instantiate() as LogMessageCard;
        if (itemCard is null)
        {
            GD.PrintErr("FATAL ERROR: Could not instantiate Item Card");
            return;
        }
        _vBoxContainer.AddChild(itemCard);
        
        itemCard.ShowCollectedItem(item);
        ItemCards.Add(itemCard);
    }

    public void ShowLogMessage(string message)
    {
        var collectedItem = ItemCards.FirstOrDefault(temp => temp.GetMessage() == message);

        if (collectedItem == null)
        {
            var itemCard = ResourceLoader.Load<PackedScene>(_itemCardScenePath).Instantiate() as LogMessageCard;
            if (itemCard is null)
            {
                GD.PrintErr("FATAL ERROR: Could not instantiate Item Card");
                return;
            }
            
            _vBoxContainer.AddChild(itemCard);
            itemCard.DisplayMessage(message);
            ItemCards.Add(itemCard);
            return;
        }
        
        collectedItem.DisplayMessage(message);
    }
}