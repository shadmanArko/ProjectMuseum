namespace ProjectMuseum.Models;

public class MuseumTileContainer
{
    private List<MuseumTile> museumTiles; // Use a private field to store the list
    public List<Exhibit> Exhibits { get; set; }
    public List<Sanitation> Sanitations { get; set; }
    public List<DecorationShop> DecorationShops { get; set; }
    public List<MuseumTile> MuseumTiles
    {
        get => museumTiles;
        set
        {
            museumTiles = value;
            GenerateAStarNodes();
        }
    }

    public AStarNode[,] AStarNodes { get; private set; }

    private void GenerateAStarNodes()
    {
        var width = 18;
        var height = 20;
        AStarNodes = new AStarNode[width, height];

        foreach (var museumTile in museumTiles)
        {
            bool isWalkable = (museumTile.ExhibitId == "string" || museumTile.ExhibitId == "") && museumTile.Walkable;
            AStarNode aStarNode = new AStarNode(museumTile.XPosition * -1, museumTile.YPosition *-1, null, 0f, 0f, isWalkable);

            // Assign the node to the grid
            AStarNodes[museumTile.XPosition *-1, museumTile.YPosition * -1] = aStarNode;
        }
    }
} 