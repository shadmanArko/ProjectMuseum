using Godot;
using Godot4CS.ProjectMuseum.Service.MuseumServices;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Managers;

public partial class MuseumReferenceManager: Node
{
    public static MuseumReferenceManager Instance;
    [Export] public ItemPlacementConditionService ItemPlacementConditionService;
    [Export] public TileServices TileServices;

    [Export] public ExhibitServices ExhibitServices;
    [Export] public StoryAndTutorialServices StoryAndTutorialServices;
    [Export] public ArtifactStoreServices ArtifactStoreServices;
    [Export] public DisplayArtifactServices DisplayArtifactServices;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance ??= this;
    }

    
}