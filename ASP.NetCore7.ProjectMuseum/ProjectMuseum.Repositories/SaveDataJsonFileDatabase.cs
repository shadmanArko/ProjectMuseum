using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories;

public class SaveDataJsonFileDatabase
{
    private readonly string _museumTileDataFolderPath;
    private readonly string _exhibitDataFolderPath;

    private readonly string _saveDataFolderPath;

    public SaveDataJsonFileDatabase(string museumTileDataFolderPath, string exhibitDataFolderPath, string saveDataFolderPath)
    {
        _museumTileDataFolderPath = museumTileDataFolderPath;
        _exhibitDataFolderPath = exhibitDataFolderPath;
        _saveDataFolderPath = saveDataFolderPath;
    }

    public async Task MergeJsonFiles()
    {
        var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(await File.ReadAllTextAsync(_museumTileDataFolderPath));
        var exhibits = JsonSerializer.Deserialize<List<Exhibit>>(await File.ReadAllTextAsync(_exhibitDataFolderPath));

        var mergedData = new MergedData
        {
            MuseumTiles = museumTiles,
            Exhibits = exhibits
        };

        var mergedJson = JsonSerializer.Serialize(mergedData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(_saveDataFolderPath, mergedJson);
    }
    
    public void SplitJsonFile()
    {
        var mergedJson = File.ReadAllText(_saveDataFolderPath);
        var mergedData = JsonSerializer.Deserialize<MergedData>(mergedJson);

        File.WriteAllText(_museumTileDataFolderPath, JsonSerializer.Serialize(mergedData?.MuseumTiles, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    
        File.WriteAllText(_exhibitDataFolderPath, JsonSerializer.Serialize(mergedData?.Exhibits, new JsonSerializerOptions
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
}