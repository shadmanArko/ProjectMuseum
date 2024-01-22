using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.BuilderCardsRepository;
using ProjectMuseum.Repositories.DecorationOtherRepository;
using ProjectMuseum.Repositories.DecorationRepository;
using ProjectMuseum.Repositories.DecorationShopRepository;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineCellCrackMaterialRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactDescriptiveRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactFunctionalRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.DisplayArtifactRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.TradingArtifactRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;
using ProjectMuseum.Repositories.PlayerInfoRepository;
using ProjectMuseum.Repositories.PlayerRepository.Sub_Repositories.TimeRepository;
using ProjectMuseum.Repositories.StorySceneRepository;
using ProjectMuseum.Services.BuilderCardService;
using ProjectMuseum.Services.DecorationOtherService;
using ProjectMuseum.Services.DecorationOtherServices;
using ProjectMuseum.Services.DecorationShopServices;
using ProjectMuseum.Services.ExhibitService;
using ProjectMuseum.Services.InventorySevice;
using ProjectMuseum.Services.LoadAndSaveService;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MineService.Sub_Services;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellCrackService;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService.RawArtifactDescriptiveService;
using ProjectMuseum.Services.MineService.Sub_Services.VehicleService;
using ProjectMuseum.Services.MuseumService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactStorageService;
using ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;
using ProjectMuseum.Services.MuseumService.Sub_Services.TradingArtifactsService;
using ProjectMuseum.Services.MuseumTileService;
using ProjectMuseum.Services.PlayerInfoService;
using ProjectMuseum.Services.PlayerService.Sub_Services.TimeService;
using ProjectMuseum.Services.StorySceneService;

var builder = WebApplication.CreateBuilder(args);


//NO Need to change these paths
string cellCrackMaterialDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Game Data Folder", "CellCrackMaterial", "CellCrackMaterial.json");
string rawArtifactFunctionalDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Game Data Folder", "RawArtifactData", "RawArtifactFunctionalData", "RawArtifactFunctionalData.json");
string rawArtifactDescriptiveDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Game Data Folder", "RawArtifactData", "RawArtifactDescriptiveData", "RawArtifactDescriptiveDataEnglish.json");
string vehicleDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Game Data Folder", "Vehicle", "Vehicle.json");




//string museumTileDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museumTile.json"); //todo for dev
//string dataFolderPath = Path.Combine(AppContext.BaseDirectory, "Dummy Data Folder", "museumTile.json"); //todo for deployment


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddSingleton(new JsonFileDatabase<MuseumTile>(Const.museumTileDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Exhibit>(Const.exhibitDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ExhibitVariation>(Const.exhibitVariationDataFolderPath));

builder.Services.AddSingleton(new JsonFileDatabase<DecorationShop>(Const.decorationShopDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DecorationShopVariation>(Const.decorationShopVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DecorationOther>(Const.decorationOtherDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DecorationOtherVariation>(Const.decorationOtherVariationDataFolderPath));

builder.Services.AddSingleton(new JsonFileDatabase<TileVariation>(Const.tileVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<WallVariation>(Const.wallVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<WallpaperVariation>(Const.wallpaperVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<StoryScene>(Const.storySceneDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Tutorial>(Const.tutorialDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<PlayerInfo>(Const.playerInfoDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Museum>(Const.museumDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Mine>(Const.mineDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Inventory>(Const.inventoryDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<MineArtifacts>(Const.mineArtifactsDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DisplayArtifacts>(Const.displayArtifactDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactStorage>(Const.artifactStorageDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<TradingArtifacts>(Const.tradingArtifactsDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<CellCrackMaterial>(cellCrackMaterialDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<RawArtifactFunctional>(rawArtifactFunctionalDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<RawArtifactDescriptive>(rawArtifactDescriptiveDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Vehicle>(vehicleDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Time>(Const.timeDataFolderPath));


builder.Services.AddSingleton(new SaveDataJsonFileDatabase(
    Const.artifactStorageDataFolderPath,
    Const.displayArtifactDataFolderPath,
    Const.exhibitDataFolderPath,
    Const.exhibitVariationDataFolderPath,
    Const.decorationShopDataFolderPath,
    Const.decorationShopVariationDataFolderPath,
    Const.decorationOtherDataFolderPath,
    Const.decorationOtherVariationDataFolderPath,
    Const.inventoryDataFolderPath,
    Const.mineDataFolderPath,
    Const.mineArtifactsDataFolderPath,
    Const.museumDataFolderPath,
    Const.museumTileDataFolderPath,
    Const.playerInfoDataFolderPath,
    Const.saveDataFolderPath,
    Const.storySceneDataFolderPath,
    Const.tradingArtifactsDataFolderPath,
    Const.timeDataFolderPath,
    Const.tutorialDataFolderPath,
    Const.wallVariationDataFolderPath,
    Const.tileVariationDataFolderPath,
    Const.wallpaperVariationDataFolderPath
    ));


builder.Services.AddScoped<IMuseumTileRepository, MuseumTileRepository>();
builder.Services.AddScoped<IMineRepository, MineRepository>();
builder.Services.AddScoped<IMuseumRepository, MuseumRepository>();
builder.Services.AddScoped<IExhibitRepository, ExhibitRepository>();
builder.Services.AddScoped<IDecorationOtherRepository, DecorationOtherRepository>();
builder.Services.AddScoped<IDecorationShopRepository, DecorationShopRepository>();
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
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITimeRepository, TimeRepository>();


builder.Services.AddScoped<IMineService, MineService>();
builder.Services.AddScoped<IMineCellGeneratorService, MineCellGeneratorService>();
builder.Services.AddScoped<IMuseumTileService, MuseumTileService>();
builder.Services.AddScoped<IMuseumService, MuseumService>();
builder.Services.AddScoped<IExhibitService, ExhibitService>();
builder.Services.AddScoped<IDecorationOtherService, DecorationOtherService>();
builder.Services.AddScoped<IDecorationShopService, DecorationShopService>();
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
builder.Services.AddScoped<IVehicleService, VehicleService>();
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
Console.WriteLine("Base directory " + Directory.GetCurrentDirectory());
// Console.WriteLine("Current directory " + Directory.GetCurrentDirectory());
// Console.WriteLine("Doc folder directory " + Environment.SpecialFolder.MyDocuments);
// Console.WriteLine("persistent directory " +  Environment.SpecialFolder.ApplicationData);

var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),/*"Documents",*/ "Red Thorn Interactive","Museum Keeper");
if (!Directory.Exists(savePath))
{
    Directory.CreateDirectory(savePath);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();