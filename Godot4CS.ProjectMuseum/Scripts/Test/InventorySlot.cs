using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Test;

public partial class InventorySlot : PanelContainer
{
	private InventoryManager _inventoryManager;

	[Export] private TextureRect _textureRect;
	[Export] private Label _itemCounter;
	[Export] private bool _isSlotEmpty;
	[Export] private int _slotNumber;

	#region Initializers
    
	public override void _Ready()
	{
		_inventoryManager = ReferenceStorage.Instance.InventoryManager;
	}
    
	#endregion

	#region Set Inventory Item

	public void SetInventoryItemToSlot(InventoryItem item)
	{
		if (item == null)
		{
			_isSlotEmpty = true;
			return;
		}
        
		_isSlotEmpty = false;
		_textureRect.Texture = GetTexture(item.PngPath);
		_itemCounter.Text = item.IsStackable && item.Stack > 1 ? item.Stack.ToString() : "";
	}

	public void SetSlotNumber(int slotNo) => _slotNumber = slotNo;
    
	#endregion

	private void OnInputEvent(Node viewport, InputEvent inputEvent, int shape)
	{
		if (inputEvent.IsActionReleased("ui_left_click"))
		{
			_inventoryManager.MakeDecision(_slotNumber, _isSlotEmpty, MouseButton.Left, out var stackNumber, out var pngSlotPath, out var slotEmpty);
			_isSlotEmpty = slotEmpty;
			_textureRect.Visible = !_isSlotEmpty;
			_itemCounter.Text = stackNumber <= 1 ? "" : stackNumber.ToString();
			if (!string.IsNullOrEmpty(pngSlotPath))
				_textureRect.Texture = GetTexture(pngSlotPath);
		}
		else if (inputEvent.IsActionReleased("ui_right_click"))
		{
			_inventoryManager.MakeDecision(_slotNumber, _isSlotEmpty, MouseButton.Right, out var stackNumber, out var pngSlotPath, out var slotEmpty);
			_isSlotEmpty = slotEmpty;
			_textureRect.Visible = !_isSlotEmpty;
			_itemCounter.Text = stackNumber <= 1 ? "" : stackNumber.ToString();
			if (!string.IsNullOrEmpty(pngSlotPath))
				_textureRect.Texture = GetTexture(pngSlotPath);
		}
	}

	#region Essentials

	private Texture2D GetTexture(string pngPath)
	{
		var texture2D = GD.Load<Texture2D>(pngPath);
		return texture2D;
	}

	#endregion
}