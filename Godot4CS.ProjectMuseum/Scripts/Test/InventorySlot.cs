using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Test;

public partial class InventorySlot : TextureRect
{
	private InventoryManager _inventoryManager;
	private InventoryItem _inventoryItem;

	[Export] private TextureRect _textureRect;
	[Export] private Label _itemCounter;
	[Export] private bool _isSlotEmpty;
	[Export] private int _slotNumber;
    
	// public static Tuple<int, Texture2D> Data;

	#region Initializers
    
	public override void _Ready()
	{
		_inventoryManager = ReferenceStorage.Instance.InventoryManager;
	}
    
	public override void _EnterTree()
	{
		
	}
    
	#endregion

	#region Set Remove Modify Inventory Item

	public void SetInventoryItemToSlot(InventoryItem item)
	{
		if (item == null)
		{
			_isSlotEmpty = true;
			return;
		}
		
		_inventoryItem = item;
		_isSlotEmpty = false;
		
		_textureRect.Texture = GetTexture(item.PngPath);
		_itemCounter.Text = item.IsStackable && item.Stack > 1 ? item.Stack.ToString() : "";
	}

	public void SetSlotNumber(int slotNo) => _slotNumber = slotNo;

	public InventoryItem RemoveInventoryItemSlot()
	{
		var tempItem = _inventoryItem;
		_inventoryItem = null;
		_isSlotEmpty = true;
		
		_textureRect.Texture = null;
		_textureRect.Visible = false;
		_itemCounter.Text = "";
		return tempItem;
	}

    
	#endregion
	
	// [Export] private bool _isDragging;

	private void OnInputEvent(Node viewport, InputEvent inputEvent, int shape)
	{
		if (inputEvent.IsActionReleased("ui_left_click"))
		{
			_inventoryManager.MakeDecision(_slotNumber, _isSlotEmpty, MouseButton.Left, out var stackNumber, out var pngSlotPath, out var slotEmpty);
			_isSlotEmpty = slotEmpty;
			_textureRect.Visible = !_isSlotEmpty;
			_itemCounter.Text = stackNumber.ToString();
			if (!string.IsNullOrEmpty(pngSlotPath))
				_textureRect.Texture = GetTexture(pngSlotPath);
			
			//todo: call a method in inventory manager
			// if(_isSlotEmpty) return;
			// if (_isSlotEmpty)
			// {
			// 	
			// }
			// else
			// {
			// 	_textureRect.Visible = false;
			// 	_itemCounter.Text = "";
			// 	var item = _inventoryManager.SetItemFromInventorySlotToMouseCursor(_inventoryItem);
			// 	if (item is not null)
			// 	{
			// 		SetInventoryItemToSlot(item);
			// 	}
			// }

		}
		else if (inputEvent.IsActionReleased("ui_right_click"))
		{
			//todo: 
		}
	}
	

	#region Essentials

	private Texture2D GetTexture(string pngPath)
	{
		var texture2D = GD.Load<Texture2D>(pngPath);
		return texture2D;
	}

	#endregion
	

	#region Previous Code

	private struct ItemData
	{
		public int SlotNo;
		public Texture2D Texture;
		public ItemData(int slot, Texture2D tex)
		{
			SlotNo = slot;
			Texture = tex;
		}
	}

	private void MouseEnter()
	{
		GD.Print("Mouse enter");
	}
	
	private void MouseExit()
	{
		GD.Print("Mouse exit");
	}
	
	// public override Variant _GetDragData(Vector2 atPosition)
	// {
	// 	//retrieve info about the slot we are dragging 
	// 	if (!_isDragging)
	// 	{
	// 		var dragTexture = new TextureRect();
	// 		dragTexture.ExpandMode = ExpandModeEnum.FitWidthProportional;
	// 		dragTexture.Texture = Texture;
	// 		dragTexture.Size = new Vector2(100, 100);
	// 		return dragTexture;
	// 	}
	// 	else
	// 	{
	// 		var dragTexture = new TextureRect();
	// 		dragTexture.ExpandMode = ExpandModeEnum.FitWidthProportional;
	// 		dragTexture.Texture = Texture;
	// 		dragTexture.Size = new Vector2(100, 100);
	//
	// 		var control = new Control();
	// 		control.AddChild(dragTexture);
	// 		GD.Print($"drag texture size: {dragTexture.Texture.GetSize()}");
	// 		SetDragPreview(control);
	// 		GD.Print("is dragging");
	// 		Data = new Tuple<int, Texture2D>(_slotNumber, Texture);
	// 		
	// 		return Texture;
	// 	}
	// }

	// public override bool _CanDropData(Vector2 atPosition, Variant data)
	// {
	// 	if (!_isSlotEmpty)
	// 	{
	// 		GD.Print("slot not empty");
	// 		return false;
	// 	}
	// 	GD.Print("slot is empty");
	// 	return true;
	// 	//check if we can drop an item in this slot
	// }

	// public override void _DropData(Vector2 atPosition, Variant data)
	// {
	// 	if (Input.IsActionJustReleased("ui_left_click"))
	// 	{
	// 		GD.Print("inside drop data method");
	// 		GD.Print($"{Data.Item1} and {Data.Item2}");
	// 		var texture = (Texture2D) data;
	// 		Texture = texture;
	// 	}
	// }

	#endregion
}