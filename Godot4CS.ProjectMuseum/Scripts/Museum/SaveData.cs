using System.Collections.Generic;
using Godot;
using ProjectMuseum.Models.Artifact_and_Inventory;

namespace ProjectMuseum.Models;

public class SaveData
{
    public PlayerInfo PlayerInfo { get; set; }
    public Museum Museum { get; set; }
    public List<MuseumTile> MuseumTiles { get; set; }

    public ArtifactStorage ArtifactStorage { get; set; }
    public Inventory Inventory { get; set; }
    public Time Time { get; set; }
    public DisplayArtifacts DisplayArtifacts { get; set; }
    public List<Exhibit> Exhibits { get; set; }
    public List<Sanitation> Sanitations { get; set; }
    public List<Shop> Shops { get; set; }
    public List<Product> Products { get; set; }
    public List<DecorationOther> DecorationOthers { get; set; }
    

   
    
}