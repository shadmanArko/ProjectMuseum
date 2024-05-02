using System;
using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine;

namespace Godot4CS.ProjectMuseum.Scripts.Test;

public partial class InventoryItemDraggable : TextureRect
{
	[Export] private bool _isSlotEmpty;
	[Export] private int _slotNumber;

	public static Tuple<int, Texture2D> Data;
	
	
	public override void _Ready()
	{
		
	}

	
	public override void _Process(double delta)
	{
		
	}

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

	public override Variant _GetDragData(Vector2 atPosition)
	{
		//retrieve info about the slot we are dragging 
		var dragTexture = new TextureRect();
		dragTexture.ExpandMode = ExpandModeEnum.FitWidthProportional;
		dragTexture.Texture = Texture;
		dragTexture.Size = new Vector2(100, 100);

		var control = new Control();
		control.AddChild(dragTexture);
        GD.Print($"drag texture size: {dragTexture.Texture.GetSize()}");
		SetDragPreview(control);
		GD.Print("is dragging");
		Data = new Tuple<int, Texture2D>(_slotNumber, Texture);
			
		return Texture;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		if (!_isSlotEmpty)
		{
			GD.Print("slot not empty");
			return false;
		}
		GD.Print("slot is empty");
		return true;
		//check if we can drop an item in this slot
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		if (Input.IsActionJustReleased("ui_left_click"))
		{
			GD.Print("inside drop data method");
			GD.Print($"{Data.Item1} and {Data.Item2}");
			var texture = (Texture2D) data;
			Texture = texture;
		}
	}
}