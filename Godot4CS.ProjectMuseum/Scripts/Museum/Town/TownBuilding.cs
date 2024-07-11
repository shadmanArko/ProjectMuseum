using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Town;

public partial class TownBuilding: ClickableObject
{
    [Export] private bool _hasDiggingBuddy;
    [Export] private bool _livingHouse;
    protected override void HandleClick()
    {
        GD.Print("TownBuilding Clicked: " + Name);
        // Add specific logic for TownBuilding
        base.HandleClick();
        if (_hasDiggingBuddy)
        {
            MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("FoundDiggingBuddy");
        }
        else if (_livingHouse)
        {
            MuseumActions.OnPlayerClickedAnEmptyHouse?.Invoke();
        }
    } 
}