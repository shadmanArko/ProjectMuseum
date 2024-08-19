using Godot;
using Godot4CS.ProjectMuseum.Service.MuseumServices;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Managers;

public partial class MuseumReferenceManager: Node
{
    public static MuseumReferenceManager Instance;
    [Export] public ItemPlacementConditionService ItemPlacementConditionService;
    [Export] public TileServices TileServices;
    [Export] public TimeServices TimeServices;

    [Export] public ExhibitServices ExhibitServices;
    [Export] public StoryAndTutorialServices StoryAndTutorialServices;
    [Export] public ArtifactStoreServices ArtifactStoreServices;
    [Export] public DisplayArtifactServices DisplayArtifactServices;
    [Export] public PlayerInfoServices PlayerInfoServices;
    [Export] public BuilderCardServices BuilderCardServices;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance ??= this;
    }

    
}