using System.Net.Mime;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.BuilderCardsRepository;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineCellCrackMaterialRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactDescriptiveRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactFunctionalRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.DisplayArtifactRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.TradingArtifactRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;
using ProjectMuseum.Repositories.PlayerInfoRepository;
using ProjectMuseum.Repositories.PlayerRepository.Sub_Repositories.TimeRepository;
using ProjectMuseum.Repositories.StorySceneRepository;
using ProjectMuseum.Services.BuilderCardService;
using ProjectMuseum.Services.ExhibitService;
using ProjectMuseum.Services.InventorySevice;
using ProjectMuseum.Services.LoadAndSaveService;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MineService.Sub_Services;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellCrackService;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService.RawArtifactDescriptiveService;
using ProjectMuseum.Services.MuseumService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactStorageService;
using ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;
using ProjectMuseum.Services.MuseumService.Sub_Services.TradingArtifactsService;
using ProjectMuseum.Services.MuseumTileService;
using ProjectMuseum.Services.PlayerInfoService;
using ProjectMuseum.Services.PlayerService.Sub_Services.TimeService;
using ProjectMuseum.Services.StorySceneService;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
string museumTileDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museumTile.json");
string museumDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museum.json");
string exhibitDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "exhibit.json");
string exhibitVariationDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "exhibitVariations.json");
string wallVariationDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "wallVariations.json");
string wallpaperVariationDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "wallpaperVariations.json");
string tileVariationDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "tileVariations.json");
string storySceneDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "StoryScene.json");
string tutorialDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "Tutorials.json");
string mineDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "mine.json");
string playerInfoDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "PlayerInfo.json");
string saveDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "save.json");
string inventoryDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "inventory.json");
string mineArtifactsDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "mineArtifact.json");
string displayArtifactDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "displayArtifact.json");
string artifactStorageDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "artifactStorage.json");
string tradingArtifactsDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "tradingArtifacts.json");
string timeDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "time.json");



//NO Need to change these paths
string cellCrackMaterialDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Game Data Folder", "CellCrackMaterial", "CellCrackMaterial.json");
string rawArtifactFunctionalDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Game Data Folder", "RawArtifactData", "RawArtifactFunctionalData", "RawArtifactFunctionalData.json");
string rawArtifactDescriptiveDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Game Data Folder", "RawArtifactData", "RawArtifactDescriptiveData", "RawArtifactDescriptiveDataEnglish.json");




//string museumTileDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museumTile.json"); //todo for dev
//string dataFolderPath = Path.Combine(AppContext.BaseDirectory, "Dummy Data Folder", "museumTile.json"); //todo for deployment


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddSingleton(new JsonFileDatabase<MuseumTile>(museumTileDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Exhibit>(exhibitDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ExhibitVariation>(exhibitVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<TileVariation>(tileVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<WallVariation>(wallVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<WallpaperVariation>(wallpaperVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<StoryScene>(storySceneDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Tutorial>(tutorialDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<PlayerInfo>(playerInfoDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Museum>(museumDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Mine>(mineDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Inventory>(inventoryDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<MineArtifacts>(mineArtifactsDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DisplayArtifacts>(displayArtifactDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactStorage>(artifactStorageDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<TradingArtifacts>(tradingArtifactsDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<CellCrackMaterial>(cellCrackMaterialDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<RawArtifactFunctional>(rawArtifactFunctionalDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<RawArtifactDescriptive>(rawArtifactDescriptiveDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Time>(timeDataFolderPath));


builder.Services.AddSingleton(new SaveDataJsonFileDatabase(
    artifactStorageDataFolderPath,
    displayArtifactDataFolderPath,
    exhibitDataFolderPath,
    exhibitVariationDataFolderPath,
    inventoryDataFolderPath,
    mineDataFolderPath,
    mineArtifactsDataFolderPath,
    museumDataFolderPath,
    museumTileDataFolderPath,
    playerInfoDataFolderPath,
    saveDataFolderPath,
    storySceneDataFolderPath,
    tradingArtifactsDataFolderPath,
    timeDataFolderPath,
    tutorialDataFolderPath,
    wallVariationDataFolderPath,
    tileVariationDataFolderPath,
    wallpaperVariationDataFolderPath
    ));


builder.Services.AddScoped<IMuseumTileRepository, MuseumTileRepository>();
builder.Services.AddScoped<IMineRepository, MineRepository>();
builder.Services.AddScoped<IMuseumRepository, MuseumRepository>();
builder.Services.AddScoped<IExhibitRepository, ExhibitRepository>();
builder.Services.AddScoped<IBuilderCardRepository, BuilderCardRepository>();
builder.Services.AddScoped<IPlayerInfoRepository, PlayerInfoRepository>();
builder.Services.AddScoped<IStorySceneRepository, StorySceneRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IMineArtifactRepository, MineArtifactRepository>();
builder.Services.AddScoped<IDisplayArtifactRepository, DisplayArtifactRepository>();
builder.Services.AddScoped<IArtifactStorageRepository, ArtifactStorageRepository>();
builder.Services.AddScoped<ITradingArtifactsRepository, TradingArtifactsRepository>();
builder.Services.AddScoped<IMineCellCrackMaterialRepository, MineCellCrackMaterialRepository>();
builder.Services.AddScoped<IRawArtifactFunctionalRepository, RawArtifactFunctionalRepository>();
builder.Services.AddScoped<IRawArtifactDescriptiveRepository, RawArtifactDescriptiveRepository>();
builder.Services.AddScoped<ITimeRepository, TimeRepository>();


builder.Services.AddScoped<IMineService, MineService>();
builder.Services.AddScoped<IMineCellGeneratorService, MineCellGeneratorService>();
builder.Services.AddScoped<IMuseumTileService, MuseumTileService>();
builder.Services.AddScoped<IMuseumService, MuseumService>();
builder.Services.AddScoped<IExhibitService, ExhibitService>();
builder.Services.AddScoped<IBuilderCardService, BuilderCardService>();
builder.Services.AddScoped<ILoadService, LoadService>();
builder.Services.AddScoped<ISaveService, SaveService>();
builder.Services.AddScoped<IPlayerInfoService, PlayerInfoService>();
builder.Services.AddScoped<IStorySceneService, StorySceneService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IMineArtifactService, MineArtifactService>();
builder.Services.AddScoped<IDisplayArtifactService, DisplayArtifactService>();
builder.Services.AddScoped<IArtifactStorageService, ArtifactStorageService>();
builder.Services.AddScoped<ITradingArtifactsService, TradingArtifactsService>();
builder.Services.AddScoped<IMineCellCrackMaterialService, MineCellCrackMaterialService>();
builder.Services.AddScoped<IRawArtifactFunctionalService, RawArtifactFunctionalService>();
builder.Services.AddScoped<IRawArtifactDescriptiveService, RawArtifactDescriptiveService>();
builder.Services.AddScoped<ITimeService, TimeService>();


// builder.Services.AddScoped<ISaveService>(provider => new SaveService(provider.GetRequiredService<SaveDataJsonFileDatabase>()));
// builder.Services.AddScoped<ILoadService>(provider => new LoadService(provider.GetRequiredService<SaveDataJsonFileDatabase>()));


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new PascalCaseNamingPolicy();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
Console.WriteLine("Base directory " +AppContext.BaseDirectory);
Console.WriteLine("Current directory " +Directory.GetCurrentDirectory());
Console.WriteLine("persistent directory " +  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Museum Keeper"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
