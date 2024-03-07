using System;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Array = System.Array;

public partial class ManualSorting : Node2D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnItemUpdated += SortItemsManually;
	}

	// public override void _PhysicsProcess(double delta)
	// {
	// 	SortItemsManually();
	// }

	public void SortItemsManually()
	{
		// Assuming you have an array of ItemScript instances to sort
		// GD.Print("Sorting Manually");
		Array<Node> nodesInGroup = GetTree().GetNodesInGroup("ManualSortGroup");
		Item[] itemsToSort = new Item[nodesInGroup.Count];
		for (int i = 0; i < nodesInGroup.Count; i++)
		{
			itemsToSort[i] = (Item)nodesInGroup[i];
		}
		foreach (var item in itemsToSort)
		{
			item.Position = new Vector2(item.Position.X, Mathf.Round(item.Position.Y));
			foreach (var item1 in itemsToSort)
			{
				if (item != item1 && Math.Abs(item.Position.Y - item1.Position.Y) < 0.1f)
				{
					var item1TilePosition = GameManager.tileMap.LocalToMap(item1.Position);
					var itemTilePosition = GameManager.tileMap.LocalToMap(item.Position);
					if (Math.Abs(item.Position.X - item1.Position.X) < 40f && item.Position.X > item1.Position.X)
					{
						if (itemTilePosition.X > item1TilePosition.X && item.numberOfTilesItTakes < 2)
						{
							item.Position = new Vector2(item.Position.X, item1.Position.Y + 0.1f);
						}
						else
						{
							item.Position = new Vector2(item.Position.X, item1.Position.Y - 0.1f);
						}
					}
				}
			}
		}
		// Sort the array using the custom comparer
		// Array.Sort(itemsToSort);
		//
		// // Reposition the nodes in the scene based on the sorted array
		// for (int i = 0; i < itemsToSort.Length; i++)
		// {
		// 	itemsToSort[i].ZIndex = itemsToSort.Length - i;
		// }
	}

	public override void _ExitTree()
	{
		MuseumActions.OnItemUpdated -= SortItemsManually;

	}
}
