using Godot;
using Godot4CS.ProjectMuseum.Scripts.Test;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;

public partial class InventoryViewController : CanvasLayer
{
    [Export] public InventorySlot[] InventoryItems;
}