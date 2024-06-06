using ProjectMuseum.Models;

namespace ProjectMuseum.Services.LoadAndSaveService;

public class NewGameDataLoaderService
{
     string _artifactStorageStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "artifactStorage.json");
    string _displayArtifactStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "displayArtifact.json");
    string _decorationShopStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "decorationShop.json");
    string _decorationShopVariationStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "decorationShopVariations.json");
    string _decorationOtherStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "decorationOther.json");
    string _decorationOtherVariationStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "decorationOtherVariations.json");
    string _exhibitStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "exhibit.json");
    string _exhibitVariationStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "exhibitVariations.json");
    string _inventoryStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "inventory.json");
    string _mineArtifactsStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "mineArtifact.json");
    string _mineStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "mine.json");
    string _museumStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "museum.json");
    string _museumTileStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "museumTile.json");
    string _playerInfoStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "PlayerInfo.json");
    string _saveStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "save.json");
    string _storySceneStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "StoryScene.json");
    string _tileVariationStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "tileVariations.json");
    string _timeStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "time.json");
    string _tradingArtifactsStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "tradingArtifacts.json");
    string _tutorialStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "Tutorials.json");
    string _wallpaperVariationStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "wallpaperVariations.json");
    string _wallVariationStartingDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "wallVariations.json");
    string _sanitationDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "sanitation.json");
    string _sanitationVariationDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Starting Data Folder", "sanitationVariations.json");
    
    //string museumTileDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Dummy Data Folder", "museumTile.json"); //todo for dev
    //string dataFolderPath = Path.Combine(AppContext.BaseDirectory, "Dummy Data Folder", "museumTile.json"); //todo for deployment

    public async Task LoadDataForNewGame()
    {
        await File.WriteAllTextAsync(Const.artifactStorageDataFolderPath, await File.ReadAllTextAsync(_artifactStorageStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.displayArtifactDataFolderPath, await File.ReadAllTextAsync(_displayArtifactStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.decorationShopDataFolderPath, await File.ReadAllTextAsync(_decorationShopStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.decorationShopVariationDataFolderPath, await File.ReadAllTextAsync(_decorationShopVariationStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.decorationOtherDataFolderPath, await File.ReadAllTextAsync(_decorationOtherStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.decorationOtherVariationDataFolderPath, await File.ReadAllTextAsync(_decorationOtherVariationStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.exhibitDataFolderPath, await File.ReadAllTextAsync(_exhibitStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.exhibitVariationDataFolderPath, await File.ReadAllTextAsync(_exhibitVariationStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.inventoryDataFolderPath, await File.ReadAllTextAsync(_inventoryStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.mineArtifactsDataFolderPath, await File.ReadAllTextAsync(_mineArtifactsStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.mineDataFolderPath, await File.ReadAllTextAsync(_mineStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.museumDataFolderPath, await File.ReadAllTextAsync(_museumStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.museumTileDataFolderPath, await File.ReadAllTextAsync(_museumTileStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.playerInfoDataFolderPath, await File.ReadAllTextAsync(_playerInfoStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.saveDataFolderPath, await File.ReadAllTextAsync(_saveStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.storySceneDataFolderPath, await File.ReadAllTextAsync(_storySceneStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.tileVariationDataFolderPath, await File.ReadAllTextAsync(_tileVariationStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.timeDataFolderPath, await File.ReadAllTextAsync(_timeStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.tradingArtifactsDataFolderPath, await File.ReadAllTextAsync(_tradingArtifactsStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.tutorialDataFolderPath, await File.ReadAllTextAsync(_tutorialStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.wallpaperVariationDataFolderPath, await File.ReadAllTextAsync(_wallpaperVariationStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.wallVariationDataFolderPath, await File.ReadAllTextAsync(_wallVariationStartingDataFolderPath));
        await File.WriteAllTextAsync(Const.sanitationDataFolderPath, await File.ReadAllTextAsync(_sanitationDataFolderPath));
        await File.WriteAllTextAsync(Const.sanitationVariationDataFolderPath, await File.ReadAllTextAsync(_sanitationVariationDataFolderPath));}
}