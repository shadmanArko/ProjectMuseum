using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class BuilderCardServices: Node
{
    private List<TileVariation> _tileVariationDatabase;
    private List<WallVariation> _wallVariationDatabase;
    private List<WallpaperVariation> _wallpaperVariationDatabase;

    public override void _Ready()
    {
        base._Ready();
        var tileVariationDatabaseJson = File.ReadAllText(
            "res://Game Data/BuilderCardsData/tileVariations.json");
        _tileVariationDatabase = JsonSerializer.Deserialize<List<TileVariation>>(tileVariationDatabaseJson);
        
        var wallVariationDatabaseJson = File.ReadAllText(
            "res://Game Data/BuilderCardsData/wallVariations.json");
        _wallVariationDatabase = JsonSerializer.Deserialize<List<WallVariation>>(wallVariationDatabaseJson);
        
        var wallpaperVariationDatabaseJson = File.ReadAllText(
            "res://Game Data/BuilderCardsData/wallpaperVariations.json");
        _wallpaperVariationDatabase = JsonSerializer.Deserialize<List<WallpaperVariation>>(wallpaperVariationDatabaseJson);
        GD.Print($"Tile Count {_tileVariationDatabase.Count}");
    }

    public List<TileVariation> GetAllTileVariations()
    {
        var tileVariations = _tileVariationDatabase;
        return tileVariations;
    }

    public List<WallVariation> GetAllWallVariations()
    {
        var wallVariations = _wallVariationDatabase;
        return wallVariations;
    }
    public List<WallpaperVariation> GetAllWallpaperVariations()
    {
        var wallpaperVariations = _wallpaperVariationDatabase;
        return wallpaperVariations;
    }
}