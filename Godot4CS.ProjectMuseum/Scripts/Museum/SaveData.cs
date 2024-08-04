using System.Collections.Generic;
using Godot;

namespace ProjectMuseum.Models;

public class SaveData
{
    public PlayerInfo PlayerInfo { get; set; }
    public List<MuseumTile> MuseumTiles { get; set; }

    public ArtifactStorage ArtifactStorage { get; set; }
    
}