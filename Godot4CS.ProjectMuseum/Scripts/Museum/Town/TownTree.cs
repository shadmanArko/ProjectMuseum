using Godot;
using System;
namespace Godot4CS.ProjectMuseum.Scripts.Museum.Town;

public partial class TownTree : ClickableObject
{
	public override void _Ready()
	{
		base._Ready();
		HoverColorCode = 0xFFFFFF;
	}

	protected override void HandleClick()
	{
		GD.Print("TownTree Clicked: " + Name);
		// Add specific logic for TownBuilding
		base.HandleClick();
	} 
}
