using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class ItemPlacementConditionService: Node
{
    private MuseumTileContainer _museumTileContainer;
    public  override async void _Ready()
    {
        base._Ready();
        await Task.Delay(1000);
        _museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();

    }

    public TilesWithExhibitDto PlaceExhibitOnTiles(string originTileId, List<string> tileIds, string exhibitVariationName, int rotationFrame)
    {
        TilesWithExhibitDto tilesWithExhibitDto = new TilesWithExhibitDto();
        Exhibit exhibit = new Exhibit();
        var museumTiles = _museumTileContainer.MuseumTiles;
        tilesWithExhibitDto.Exhibit = exhibit;
        tilesWithExhibitDto.MuseumTiles = museumTiles;
        foreach (var tileId in tileIds)
        {
            if (tileId == originTileId)
            {
                var museumTile = museumTiles.FirstOrDefault(tile => tile.Id == tileId);
                if (museumTile != null && !museumTile.Walkable) return tilesWithExhibitDto;
                if (museumTile == null) return tilesWithExhibitDto;
                exhibit = new Exhibit
                {
                    Id = Guid.NewGuid().ToString(),
                    ExhibitVariationName = exhibitVariationName,
                    XPosition = museumTile.XPosition,
                    YPosition = museumTile.YPosition,
                    OccupiedTileIds = tileIds,
                    ArtifactIds = new List<string>(),
                    RotationFrame = rotationFrame,
                    ExhibitDecoration = "string",
                    ExhibitArtifactSlot1 = "string",
                    ExhibitArtifactSlot2 = "string",
                    ExhibitArtifactSlot3 = "string",
                    ExhibitArtifactSlot4 = "string",
                    ExhibitArtifactSlot5 = "string"
                };
                _museumTileContainer.Exhibits.Add(exhibit);
                
            }
        }
        foreach (var tile in museumTiles)
        {
            if (tileIds.Contains(tile.Id))
            {
                tile.ItemId = exhibit.Id;
                tile.Walkable = false;
                tile.HasExhibit = true;
                tile.ItemType = ItemTypeEnum.Exhibit;
            }
        }
        
        tilesWithExhibitDto.Exhibit = exhibit;
        var exhibits = _museumTileContainer.Exhibits;
        tilesWithExhibitDto.Exhibits = exhibits;
        if (museumTiles != null) tilesWithExhibitDto.MuseumTiles = museumTiles;
        return tilesWithExhibitDto;
    }

}