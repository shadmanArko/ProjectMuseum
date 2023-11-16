using Godot;
using System;

public partial class DropTarget : ColorRect
{
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		var canDrop =  ((Node)data).IsInGroup("Draggable");
		GD.Print($"Can drop data has run {Name}");
		return canDrop;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		GD.Print($"drop data has run");
	}
}
