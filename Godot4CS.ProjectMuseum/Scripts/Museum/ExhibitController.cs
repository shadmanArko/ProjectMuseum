using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class ExhibitController : Node2D
{
    [Export] private ExhibitEditorUi _exhibitEditorUi;
    public override void _Ready()
    {
        MuseumActions.OnClickItem += OnClickItem;
    }

    private void OnClickItem(Item item)
    {
        GD.Print($"Clicked {item.itemType} {item.Name}");
        _exhibitEditorUi.Visible = true;
    }


    private void OpenUiForNode(Node node)
    {
        // Perform actions specific to the clicked node
        GD.Print("Clicked on: " + node.Name);
        // Add your UI opening logic here
    }
}