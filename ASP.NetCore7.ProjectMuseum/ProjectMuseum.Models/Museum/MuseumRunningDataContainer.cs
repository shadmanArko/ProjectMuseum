using Godot4CS.ProjectMuseum.Scripts.Museum.GameData;
using ProjectMuseum.Models.Artifact_and_Inventory;
using Time = ProjectMuseum.Models.Time;
namespace ProjectMuseum.Models;
public class MuseumRunningDataContainer
{
    private List<MuseumTile> museumTiles; // Use a private field to store the list
    public List<Exhibit> Exhibits { get; set; }
    public List<Sanitation> Sanitations { get; set; }
    public List<Shop> Shops { get; set; }
    public List<Product> Products { get; set; }
    public List<DecorationOther> DecorationOthers;
    public Time Time { get; set; }
    public PlayerInfo PlayerInfo { get; set; }
    public ArtifactStorage ArtifactStorage { get; set; }

    public List<MuseumTile> MuseumTiles
    {
        get => museumTiles;
        set
        {
            museumTiles = value;
            GenerateAStarNodes();
        }
    }

    public List<AStarNode> AStarNodes { get; private set; }

    private void GenerateAStarNodes()
    {
        AStarNodes = new List<AStarNode>();

        foreach (var museumTile in museumTiles)
        {
            bool isWalkable = museumTile.Walkable;
            AStarNode aStarNode = new AStarNode(museumTile.XPosition * -1, museumTile.YPosition *-1, null, 0f, 0f, isWalkable);

            // Assign the node to the grid
            AStarNodes.Add(aStarNode); //
        }
    }
} 