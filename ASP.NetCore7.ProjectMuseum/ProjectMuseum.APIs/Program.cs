using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.DisplayArtifactRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.TradingArtifactRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;
using ProjectMuseum.Repositories.PlayerInfoRepository;
using ProjectMuseum.Repositories.StorySceneRepository;
using ProjectMuseum.Services;
using ProjectMuseum.Services.ExhibitService;
using ProjectMuseum.Services.InventorySevice;
using ProjectMuseum.Services.LoadAndSaveService;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MineService.Sub_Services;
using ProjectMuseum.Services.MuseumService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactStorageService;
using ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;
using ProjectMuseum.Services.MuseumService.Sub_Services.TradingArtifactsService;
using ProjectMuseum.Services.MuseumTileService;
using ProjectMuseum.Services.PlayerInfoService;
using ProjectMuseum.Services.StorySceneService;

var builder = WebApplication.CreateBuilder(args);
string museumTileDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museumTile.json");
string museumDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museum.json");
string exhibitDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "exhibit.json");
string exhibitVariationDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "exhibitVariations.json");
string storySceneDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "StoryScene.json");
string mineDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "mine.json");
string playerInfoDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "PlayerInfo.json");
string saveDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "save.json");
string inventoryDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "inventory.json");
string mineArtifactDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "mineArtifact.json");
string displayArtifactDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "displayArtifact.json");
string artifactStorageDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "artifactStorage.json");
string tradingArtifactsDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "tradingArtifacts.json");



//string dataFolderPath = Path.Combine(AppContext.BaseDirectory, "Dummy Data Folder", "museumTile.json"); //todo for deployment


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new JsonFileDatabase<MuseumTile>(museumTileDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Exhibit>(exhibitDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ExhibitVariation>(exhibitVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<StoryScene>(storySceneDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<PlayerInfo>(playerInfoDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Museum>(museumDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Mine>(mineDataFolderPath));
builder.Services.AddSingleton(new SaveDataJsonFileDatabase(museumTileDataFolderPath, exhibitDataFolderPath, saveDataFolderPath, museumDataFolderPath, playerInfoDataFolderPath, storySceneDataFolderPath, exhibitVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Inventory>(inventoryDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<MineArtifacts>(mineArtifactDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DisplayArtifacts>(displayArtifactDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactStorage>(artifactStorageDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<TradingArtifacts>(tradingArtifactsDataFolderPath));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IMuseumTileRepository, MuseumTileRepository>();
builder.Services.AddScoped<IMineRepository, MineRepository>();
builder.Services.AddScoped<IMuseumRepository, MuseumRepository>();
builder.Services.AddScoped<IExhibitRepository, ExhibitRepository>();
builder.Services.AddScoped<IPlayerInfoRepository, PlayerInfoRepository>();
builder.Services.AddScoped<IStorySceneRepository, StorySceneRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IMineArtifactRepository, MineArtifactRepository>();
builder.Services.AddScoped<IDisplayArtifactRepository, DisplayArtifactRepository>();
builder.Services.AddScoped<IArtifactStorageRepository, ArtifactStorageRepository>();
builder.Services.AddScoped<ITradingArtifactsRepository, TradingArtifactsRepository>();
builder.Services.AddScoped<IMineService, MineService>();
builder.Services.AddScoped<IMuseumTileService, MuseumTileService>();
builder.Services.AddScoped<IMuseumService, MuseumService>();
builder.Services.AddScoped<IExhibitService, ExhibitService>();
builder.Services.AddScoped<ILoadService, LoadService>();
builder.Services.AddScoped<ISaveService, SaveService>();
builder.Services.AddScoped<IPlayerInfoService, PlayerInfoService>();
builder.Services.AddScoped<IStorySceneService, StorySceneService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IMineArtifactService, MineArtifactService>();
builder.Services.AddScoped<IDisplayArtifactService, DisplayArtifactService>();
builder.Services.AddScoped<IArtifactStorageService, ArtifactStorageService>();
builder.Services.AddScoped<ITradingArtifactsService, TradingArtifactsService>();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
