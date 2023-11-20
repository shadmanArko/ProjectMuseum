using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public class SaveDataJsonFileDatabase
{
    private readonly string _museumTileDataFolderPath;
    private readonly string _museumDataFolderPath;

    private readonly string _exhibitDataFolderPath;
    private readonly string _exhibitVariationDataFolderPath;
    private readonly string _playerInfoDataFolderPath;
    private readonly string _storySceneDataFolderPath;

    private readonly string _saveDataFolderPath;

    public SaveDataJsonFileDatabase(string museumTileDataFolderPath, string exhibitDataFolderPath, string saveDataFolderPath, string museumDataFolderPath, string playerInfoDataFolderPath, string storySceneDataFolderPath, string exhibitVariationDataFolderPath)
    {
        _museumTileDataFolderPath = museumTileDataFolderPath;
        _exhibitDataFolderPath = exhibitDataFolderPath;
        _saveDataFolderPath = saveDataFolderPath;
        _museumDataFolderPath = museumDataFolderPath;
        _playerInfoDataFolderPath = playerInfoDataFolderPath;
        _storySceneDataFolderPath = storySceneDataFolderPath;
        _exhibitVariationDataFolderPath = exhibitVariationDataFolderPath;
    }

    public async Task MergeJsonFiles()
    {
        var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(await File.ReadAllTextAsync(_museumTileDataFolderPath));
        var exhibits = JsonSerializer.Deserialize<List<Exhibit>>(await File.ReadAllTextAsync(_exhibitDataFolderPath));
        var exhibitVariations = JsonSerializer.Deserialize<List<ExhibitVariation>>(await File.ReadAllTextAsync(_exhibitVariationDataFolderPath));
        var playerInfos = JsonSerializer.Deserialize<List<PlayerInfo>>(await File.ReadAllTextAsync(_playerInfoDataFolderPath));
        var museums = JsonSerializer.Deserialize<List<Museum>>(await File.ReadAllTextAsync(_museumDataFolderPath));
        var storyScenes = JsonSerializer.Deserialize<List<StoryScene>>(await File.ReadAllTextAsync(_storySceneDataFolderPath));


        var mergedData = new MergedData
        {
            MuseumTiles = museumTiles,
            Exhibits = exhibits,
            ExhibitVariations = exhibitVariations,
            PlayerInfos = playerInfos,
            Museums = museums,
            StoryScenes = storyScenes
        };

        var mergedJson = JsonSerializer.Serialize(mergedData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(_saveDataFolderPath, mergedJson);
    }
    
    public async Task SplitJsonFile()
    {
        var mergedJson = await File.ReadAllTextAsync(_saveDataFolderPath);
        var mergedData = JsonSerializer.Deserialize<MergedData>(mergedJson);

        await File.WriteAllTextAsync(_museumTileDataFolderPath, JsonSerializer.Serialize(mergedData?.MuseumTiles, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    
        await File.WriteAllTextAsync(_exhibitDataFolderPath, JsonSerializer.Serialize(mergedData?.Exhibits, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
        await File.WriteAllTextAsync(_exhibitVariationDataFolderPath, JsonSerializer.Serialize(mergedData?.ExhibitVariations, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
        await File.WriteAllTextAsync(_playerInfoDataFolderPath, JsonSerializer.Serialize(mergedData?.PlayerInfos, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
        
        await File.WriteAllTextAsync(_museumDataFolderPath, JsonSerializer.Serialize(mergedData?.Museums, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
        await File.WriteAllTextAsync(_storySceneDataFolderPath, JsonSerializer.Serialize(mergedData?.StoryScenes, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
    
}
public class MergedData
{
    [JsonPropertyName("MuseumTiles")]
    public List<MuseumTile>? MuseumTiles { get; set; }
    
    [JsonPropertyName("Exhibits")]
    public List<Exhibit>? Exhibits { get; set; }
    [JsonPropertyName("ExhibitVariations")]
    public List<ExhibitVariation>? ExhibitVariations { get; set; }
    [JsonPropertyName("PlayerInfo")]
    public List<PlayerInfo>? PlayerInfos { get; set; }
    
    [JsonPropertyName("Museums")]
    public List<Museum>? Museums { get; set; }
    [JsonPropertyName("StoryScenes")]
    public List<StoryScene>? StoryScenes { get; set; }
}