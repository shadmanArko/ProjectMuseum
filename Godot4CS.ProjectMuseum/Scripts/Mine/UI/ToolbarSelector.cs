using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enum;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class ToolbarSelector : Node
{
	[Export] private Array<ToolbarSlot> _toolbarSlots;
	public override void _Ready()
	{
		SubscribeToActions();
	}

	private void SubscribeToActions()
	{
		MineActions.OnToolbarSlotChanged += SelectItem;
	}

	private void SelectItem(Equipables equipable)
	{
		GD.Print($"eqipable: {(int) equipable}");
		DeselectAllItems();
		_toolbarSlots[(int) equipable].SetItemAsSelected();
	}


	private void DeselectAllItems()
	{
		foreach (var slot in _toolbarSlots)
			slot.SetItemAsDeselected();
	}
}