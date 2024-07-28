using Godot;
using Godot4CS.ProjectMuseum.Service.MuseumServices;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Managers;

public partial class MuseumReferenceManager: Node
{
    public static MuseumReferenceManager Instance;
    [Export] public ItemPlacementConditionService ItemPlacementConditionService;
    public override void _Ready()
    {
        base._Ready();
        Instance ??= this;
    }
}