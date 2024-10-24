using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public class SaveDataJsonFileDatabase
{
    private readonly string _artifactStorageDataFolderPath;
    private readonly string _displayArtifactDataFolderPath;
    private readonly string _exhibitDataFolderPath;
    private readonly string _exhibitVariationDataFolderPath;
    private readonly string _decorationShopDataFolderPath;
    private readonly string _decorationShopVariationDataFolderPath;
    private readonly string _decorationOtherDataFolderPath;
    private readonly string _decorationOtherVariationDataFolderPath;
    private readonly string _inventoryDataFolderPath;
    private readonly string _mineArtifactsDataFolderPath;
    private readonly string _mineDataFolderPath;
    private readonly string _museumDataFolderPath;
    private readonly string _museumTileDataFolderPath;
    private readonly string _playerInfoDataFolderPath;
    private readonly string _saveDataFolderPath;
    private readonly string _storySceneDataFolderPath;
    private readonly string _tileVariationDataFolderPath;
    private readonly string _timeDataFolderPath;
    private readonly string _tradingArtifactsDataFolderPath;
    private readonly string _tutorialDataFolderPath;
    private readonly string _wallpaperVariationDataFolderPath;
    private readonly string _wallVariationDataFolderPath;
    private readonly string _sanitationDataFolderPath;


    public SaveDataJsonFileDatabase(
        string artifactStorageDataFolderPath,
        string displayArtifactDataFolderPath,
        string exhibitDataFolderPath,
        string exhibitVariationDataFolderPath,
        string decorationShopDataFolderPath,
        string decorationShopVariationDataFolderPath,
        string decorationOtherDataFolderPath,
        string decorationOtherVariationDataFolderPath,
        string inventoryDataFolderPath,
        string mineDataFolderPath,
        string mineArtifactsDataFolderPath,
        string museumDataFolderPath,
        string museumTileDataFolderPath,
        string playerInfoDataFolderPath,
        string saveDataFolderPath,
        string storySceneDataFolderPath,
        string tradingArtifactsDataFolderPath, 
        string timeDataFolderPath,
        string tutorialDataFolderPath, 
        string wallVariationDataFolderPath, 
        string tileVariationDataFolderPath,
        string wallpaperVariationDataFolderPath, 
        string sanitationDataFolderPath)
    {
        _artifactStorageDataFolderPath = artifactStorageDataFolderPath;
        _displayArtifactDataFolderPath = displayArtifactDataFolderPath;
        _exhibitDataFolderPath = exhibitDataFolderPath;
        _exhibitVariationDataFolderPath = exhibitVariationDataFolderPath;
        _decorationShopDataFolderPath = decorationShopDataFolderPath;
        _decorationShopVariationDataFolderPath = decorationShopVariationDataFolderPath;
        _decorationOtherDataFolderPath = decorationOtherDataFolderPath;
        _decorationOtherVariationDataFolderPath = decorationOtherVariationDataFolderPath;
        _inventoryDataFolderPath = inventoryDataFolderPath;
        _mineDataFolderPath = mineDataFolderPath;
        _mineArtifactsDataFolderPath = mineArtifactsDataFolderPath;
        _museumDataFolderPath = museumDataFolderPath;
        _museumTileDataFolderPath = museumTileDataFolderPath;
        _playerInfoDataFolderPath = playerInfoDataFolderPath;
        _saveDataFolderPath = saveDataFolderPath;
        _storySceneDataFolderPath = storySceneDataFolderPath;
        _tradingArtifactsDataFolderPath = tradingArtifactsDataFolderPath;
        _timeDataFolderPath = timeDataFolderPath;
        _tutorialDataFolderPath = tutorialDataFolderPath;
        _wallVariationDataFolderPath = wallVariationDataFolderPath;
        _tileVariationDataFolderPath = tileVariationDataFolderPath;
        _wallpaperVariationDataFolderPath = wallpaperVariationDataFolderPath;
        _sanitationDataFolderPath = sanitationDataFolderPath;
    }

    public async Task MergeJsonFiles()
    {
        var artifactStorage = JsonSerializer.Deserialize<List<ArtifactStorage>>(await File.ReadAllTextAsync(_artifactStorageDataFolderPath));
        var displayArtifact = JsonSerializer.Deserialize<List<DisplayArtifacts>>(await File.ReadAllTextAsync(_displayArtifactDataFolderPath));
        var exhibits = JsonSerializer.Deserialize<List<Exhibit>>(await File.ReadAllTextAsync(_exhibitDataFolderPath));
        var exhibitVariations = JsonSerializer.Deserialize<List<ExhibitVariation>>(await File.ReadAllTextAsync(_exhibitVariationDataFolderPath));
        var decorationShops = JsonSerializer.Deserialize<List<DecorationShop>>(await File.ReadAllTextAsync(_decorationShopDataFolderPath));
        var decorationShopVariations = JsonSerializer.Deserialize<List<DecorationShopVariation>>(await File.ReadAllTextAsync(_decorationShopVariationDataFolderPath));
        var decorationOthers = JsonSerializer.Deserialize<List<DecorationOther>>(await File.ReadAllTextAsync(_decorationOtherDataFolderPath));
        var decorationOtherVariations = JsonSerializer.Deserialize<List<DecorationOtherVariation>>(await File.ReadAllTextAsync(_decorationOtherVariationDataFolderPath));
        var wallVariations = JsonSerializer.Deserialize<List<WallVariation>>(await File.ReadAllTextAsync(_wallVariationDataFolderPath));
        var wallpaperVariations = JsonSerializer.Deserialize<List<WallpaperVariation>>(await File.ReadAllTextAsync(_wallpaperVariationDataFolderPath));
        var sanitations = JsonSerializer.Deserialize<List<Sanitation>>(await File.ReadAllTextAsync(_sanitationDataFolderPath));

        var tileVariations = JsonSerializer.Deserialize<List<TileVariation>>(await File.ReadAllTextAsync(_tileVariationDataFolderPath));
        var inventory = JsonSerializer.Deserialize<List<Inventory>>(await File.ReadAllTextAsync(_inventoryDataFolderPath));
        var mine = JsonSerializer.Deserialize<List<Mine>>(await File.ReadAllTextAsync(_mineDataFolderPath));
        var mineArtifact = JsonSerializer.Deserialize<List<MineArtifacts>>(await File.ReadAllTextAsync(_mineArtifactsDataFolderPath));
        var museums = JsonSerializer.Deserialize<List<Museum>>(await File.ReadAllTextAsync(_museumDataFolderPath));
        var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(await File.ReadAllTextAsync(_museumTileDataFolderPath));
        var playerInfos = JsonSerializer.Deserialize<List<PlayerInfo>>(await File.ReadAllTextAsync(_playerInfoDataFolderPath));
        var storyScenes = JsonSerializer.Deserialize<List<StoryScene>>(await File.ReadAllTextAsync(_storySceneDataFolderPath));
        var tutorials = JsonSerializer.Deserialize<List<Tutorial>>(await File.ReadAllTextAsync(_tutorialDataFolderPath));
        var tradingArtifacts = JsonSerializer.Deserialize<List<TradingArtifacts>>(await File.ReadAllTextAsync(_tradingArtifactsDataFolderPath));
        var times = JsonSerializer.Deserialize<List<Time>>(await File.ReadAllTextAsync(_timeDataFolderPath));


        var mergedData = new MergedData
        {
            ArtifactStorages = artifactStorage,
            DisplayArtifacts = displayArtifact,
            Exhibits = exhibits,
            ExhibitVariations = exhibitVariations,
            DecorationShops = decorationShops,
            DecorationShopVariations = decorationShopVariations,
            DecorationOthers = decorationOthers,
            DecorationOtherVariations = decorationOtherVariations,
            Inventories = inventory,
            Mines = mine,
            MineArtifacts = mineArtifact,
            Museums = museums,
            MuseumTiles = museumTiles,
            PlayerInfos = playerInfos,
            StoryScenes = storyScenes,
            Tutorials = tutorials,
            TradingArtifacts = tradingArtifacts,
            Times = times,
            WallVariations = wallVariations,
            TileVariations = tileVariations,
            WallpaperVariations = wallpaperVariations,
            Sanitations = sanitations
        };

        var mergedJson = JsonSerializer.Serialize(mergedData, new JsonSerializerOptions{ WriteIndented = true });

        await File.WriteAllTextAsync(_saveDataFolderPath, mergedJson);
    }

    public async Task SplitJsonFile()
    {
        var mergedJson = await File.ReadAllTextAsync(_saveDataFolderPath);
        var mergedData = JsonSerializer.Deserialize<MergedData>(mergedJson);

        await File.WriteAllTextAsync(_artifactStorageDataFolderPath, JsonSerializer.Serialize(mergedData?.ArtifactStorages, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_displayArtifactDataFolderPath, JsonSerializer.Serialize(mergedData?.DisplayArtifacts, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_exhibitDataFolderPath, JsonSerializer.Serialize(mergedData?.Exhibits, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_exhibitVariationDataFolderPath, JsonSerializer.Serialize(mergedData?.ExhibitVariations, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_decorationShopDataFolderPath, JsonSerializer.Serialize(mergedData?.DecorationShops, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_decorationShopVariationDataFolderPath, JsonSerializer.Serialize(mergedData?.DecorationShopVariations, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_decorationOtherDataFolderPath, JsonSerializer.Serialize(mergedData?.DecorationOthers, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_decorationOtherVariationDataFolderPath, JsonSerializer.Serialize(mergedData?.DecorationOtherVariations, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_sanitationDataFolderPath, JsonSerializer.Serialize(mergedData?.Sanitations, new JsonSerializerOptions{ WriteIndented = true }));

        await File.WriteAllTextAsync(_wallVariationDataFolderPath, JsonSerializer.Serialize(mergedData?.WallVariations, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_wallpaperVariationDataFolderPath, JsonSerializer.Serialize(mergedData?.WallpaperVariations, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_tileVariationDataFolderPath, JsonSerializer.Serialize(mergedData?.TileVariations, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_inventoryDataFolderPath, JsonSerializer.Serialize(mergedData?.Inventories, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_mineDataFolderPath, JsonSerializer.Serialize(mergedData?.Mines, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_mineArtifactsDataFolderPath, JsonSerializer.Serialize(mergedData?.MineArtifacts, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_museumDataFolderPath, JsonSerializer.Serialize(mergedData?.Museums, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_museumTileDataFolderPath, JsonSerializer.Serialize(mergedData?.MuseumTiles, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_playerInfoDataFolderPath, JsonSerializer.Serialize(mergedData?.PlayerInfos, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_storySceneDataFolderPath, JsonSerializer.Serialize(mergedData?.StoryScenes, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_tutorialDataFolderPath, JsonSerializer.Serialize(mergedData?.Tutorials, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_tradingArtifactsDataFolderPath, JsonSerializer.Serialize(mergedData?.TradingArtifacts, new JsonSerializerOptions{ WriteIndented = true }));
        await File.WriteAllTextAsync(_timeDataFolderPath, JsonSerializer.Serialize(mergedData?.Times, new JsonSerializerOptions{ WriteIndented = true }));
    }
}
public class MergedData
{
    [JsonPropertyName("ArtifactStorages")] public List<ArtifactStorage>? ArtifactStorages { get; set; }
    [JsonPropertyName("DisplayArtifacts")] public List<DisplayArtifacts>? DisplayArtifacts { get; set; }
    [JsonPropertyName("Exhibits")] public List<Exhibit>? Exhibits { get; set; }
    [JsonPropertyName("ExhibitVariations")] public List<ExhibitVariation>? ExhibitVariations { get; set; }
    [JsonPropertyName("DecorationShops")] public List<DecorationShop>? DecorationShops { get; set; }
    [JsonPropertyName("DecorationShopVariations")] public List<DecorationShopVariation>? DecorationShopVariations { get; set; }
    [JsonPropertyName("DecorationOthers")] public List<DecorationOther>? DecorationOthers { get; set; }
    [JsonPropertyName("DecorationOtherVariations")] public List<DecorationOtherVariation>? DecorationOtherVariations { get; set; }
    [JsonPropertyName("Inventories")] public List<Inventory>? Inventories { get; set; }
    [JsonPropertyName("Mines")] public List<Mine>? Mines { get; set; }
    [JsonPropertyName("MineArtifacts")] public List<MineArtifacts>? MineArtifacts { get; set; }
    [JsonPropertyName("Museums")] public List<Museum>? Museums { get; set; }
    [JsonPropertyName("MuseumTiles")] public List<MuseumTile>? MuseumTiles { get; set; }
    [JsonPropertyName("PlayerInfo")] public List<PlayerInfo>? PlayerInfos { get; set; }
    [JsonPropertyName("StoryScenes")] public List<StoryScene>? StoryScenes { get; set; }
    [JsonPropertyName("Tutorials")] public List<Tutorial>? Tutorials { get; set; }
    [JsonPropertyName("TradingArtifacts")] public List<TradingArtifacts>? TradingArtifacts { get; set; }
    [JsonPropertyName("Time")] public List<Time>? Times { get; set; }
    [JsonPropertyName("WallVariations")] public List<WallVariation>? WallVariations { get; set; }
    [JsonPropertyName("TileVariations")] public List<TileVariation>? TileVariations { get; set; }
    [JsonPropertyName("WallpaperVariations")] public List<WallpaperVariation>? WallpaperVariations { get; set; }
    [JsonPropertyName("Sanitations")] public List<Sanitation>? Sanitations { get; set; }
}