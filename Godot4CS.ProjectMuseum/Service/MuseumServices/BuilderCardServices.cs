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
    private List<ExhibitVariation> _exhibitVariations;
    private List<DecorationShopVariation> _decorationShopVariations;
    private List<SanitationVariation> _sanitationVariations;
    private List<DecorationOtherVariation> _decorationOthers;
    private bool Loaded = false;
    public override void _Ready()
    {
        base._Ready();
        LoadData();

        GD.Print($"Tile Count {_tileVariationDatabase.Count}");

    }

    private void LoadData()
    {
        if (Loaded) return;
        
        var tileVariationDatabaseJson = Godot.FileAccess.Open(
            "res://Game Data/BuilderCardsData/tileVariations.json",
            Godot.FileAccess.ModeFlags.Read).GetAsText();
        _tileVariationDatabase = JsonSerializer.Deserialize<List<TileVariation>>(tileVariationDatabaseJson);

        var wallVariationDatabaseJson = Godot.FileAccess.Open(
            "res://Game Data/BuilderCardsData/wallVariations.json",
            Godot.FileAccess.ModeFlags.Read).GetAsText();
        _wallVariationDatabase = JsonSerializer.Deserialize<List<WallVariation>>(wallVariationDatabaseJson);

        var wallpaperVariationDatabaseJson = Godot.FileAccess.Open(
            "res://Game Data/BuilderCardsData/wallpaperVariations.json",
            Godot.FileAccess.ModeFlags.Read).GetAsText();
        _wallpaperVariationDatabase = JsonSerializer.Deserialize<List<WallpaperVariation>>(wallpaperVariationDatabaseJson);
        
        var exhibitVariationDatabaseJson = Godot.FileAccess.Open(
            "res://Game Data/Starting Data/exhibitVariations.json",
            Godot.FileAccess.ModeFlags.Read).GetAsText();
        _exhibitVariations = JsonSerializer.Deserialize<List<ExhibitVariation>>(exhibitVariationDatabaseJson);
        var decorationShopVariationDatabaseJson = Godot.FileAccess.Open(
            "res://Game Data/Starting Data/decorationShopVariations.json",
            Godot.FileAccess.ModeFlags.Read).GetAsText();
        _decorationShopVariations = JsonSerializer.Deserialize<List<DecorationShopVariation>>(decorationShopVariationDatabaseJson);
        var sanitationDatabaseJson = Godot.FileAccess.Open(
            "res://Game Data/Starting Data/sanitationVariations.json",
            Godot.FileAccess.ModeFlags.Read).GetAsText();
        _sanitationVariations = JsonSerializer.Deserialize<List<SanitationVariation>>(sanitationDatabaseJson);
        
        var decorationOtherDatabaseJson = Godot.FileAccess.Open(
            "res://Game Data/Starting Data/decorationOtherVariations.json",
            Godot.FileAccess.ModeFlags.Read).GetAsText();
        _decorationOthers = JsonSerializer.Deserialize<List<DecorationOtherVariation>>(decorationOtherDatabaseJson);
        Loaded = true;
    }

    public List<TileVariation> GetAllTileVariations()
    {
        LoadData();
        var tileVariations = _tileVariationDatabase;
        return tileVariations;
    }
    public List<ExhibitVariation> GetAllExhibitVariations()
    {
        LoadData();
        return _exhibitVariations;
    }
    public List<SanitationVariation> GetAllSanitationVariations()
    {
        LoadData();
        return _sanitationVariations;
    }
    public List<DecorationShopVariation> GetAllShopVariations()
    {
        LoadData();
        return _decorationShopVariations;
    }
     public List<DecorationOtherVariation> GetAllDecorationOtherVariations()
    {
        LoadData();
        return _decorationOthers;
    }
    
    
    public List<WallVariation> GetAllWallVariations()
    {
        LoadData();
        var wallVariations = _wallVariationDatabase;
        return wallVariations;
    }
    public List<WallpaperVariation> GetAllWallpaperVariations()
    {
        LoadData();
        var wallpaperVariations = _wallpaperVariationDatabase;
        return wallpaperVariations;
    }
}