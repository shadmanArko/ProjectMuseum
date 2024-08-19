using ProjectMuseum.Models;
using ProjectMuseum.Models.Artifact_and_Inventory;
using ProjectMuseum.Models.CoreShop;
using ProjectMuseum.Models.MIne;
using ProjectMuseum.Models.MIne.Equipables;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.BuilderCardsRepository;
using ProjectMuseum.Repositories.DecorationOtherRepository;
using ProjectMuseum.Repositories.DecorationRepository;
using ProjectMuseum.Repositories.DecorationShopRepository;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactEraRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactRarityRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactScoreRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactThemeMatchingTagCountRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CellPlaceableRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ConsumableRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.EquipableRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineArtifactRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.MineCellCrackMaterialRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ProceduralMineGenerationRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactDescriptiveRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactFunctionalRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ResourceRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SiteArtifactRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SpecialBackdropRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VineInformationRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.WallPlaceableRepository;
using ProjectMuseum.Repositories.MiscellaneousDataRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.DisplayArtifactRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.GuestBuildingParameterRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.MuseumZoneRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.ProductRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.SanitationRepository;
using ProjectMuseum.Repositories.MuseumRepository.Sub_Repositories.ShopRepository;
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
using ProjectMuseum.Services.LoadAndSaveService;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MineService.Sub_Services.CaveService;
using ProjectMuseum.Services.MineService.Sub_Services.CellPlaceableService;
using ProjectMuseum.Services.MineService.Sub_Services.ConsumableService;
using ProjectMuseum.Services.MineService.Sub_Services.EquipableService;
using ProjectMuseum.Services.MineService.Sub_Services.MineArtifactService;
using ProjectMuseum.Services.MineService.Sub_Services.MineCellCrackService;
using ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService;
using ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService.MineOrdinaryCellGeneratorService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService.RawArtifactDescriptiveService;
using ProjectMuseum.Services.MineService.Sub_Services.ResourceService;
using ProjectMuseum.Services.MineService.Sub_Services.SiteArtifactChanceService;
using ProjectMuseum.Services.MineService.Sub_Services.SpecialBackdropService;
using ProjectMuseum.Services.MineService.Sub_Services.VineInformationService;
using ProjectMuseum.Services.MineService.Sub_Services.WallPlaceableService;
using ProjectMuseum.Services.MiscellaneousDataService;
using ProjectMuseum.Services.MuseumService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService.ArtifactConditionService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService.ArtifactRarityService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactStorageService;
using ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;
using ProjectMuseum.Services.MuseumService.Sub_Services.GuestBuilderParameterService;
using ProjectMuseum.Services.MuseumService.Sub_Services.MuseumZoneService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ProductService;
using ProjectMuseum.Services.MuseumService.Sub_Services.SanaitationService;
using ProjectMuseum.Services.MuseumService.Sub_Services.ShopService;
using ProjectMuseum.Services.MuseumService.Sub_Services.TradingArtifactsService;
using ProjectMuseum.Services.MuseumTileService;
using ProjectMuseum.Services.PlayerInfoService;
using ProjectMuseum.Services.PlayerService.Sub_Services.InventoryService;
using ProjectMuseum.Services.PlayerService.Sub_Services.TimeService;
using ProjectMuseum.Services.StorySceneService;

var builder = WebApplication.CreateBuilder(args);


//NO Need to change these paths
string cellCrackMaterialDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "CellCrackMaterial", "CellCrackMaterial.json");
string rawArtifactFunctionalDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "RawArtifactData", "RawArtifactFunctionalData", "RawArtifactFunctionalData.json");
string rawArtifactDescriptiveDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "RawArtifactData", "RawArtifactDescriptiveData", "RawArtifactDescriptiveDataEnglish.json");
string resourceDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "Resource", "Resource.json");
string mainMenuMiscellaneousDataFolderPath = MiscellaneousDataFolderPath.MainMenuMiscellaneousDataFolderPath(LanguageSelector.GetCurrentLanguage());
string settingsMiscellaneousDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "MiscellaneousData", "SettingsMiscellaneousData", "SettingsMiscellaneousData.json");
string museumMiscellaneousDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "MiscellaneousData", "MuseumMiscellaneousData", "MuseumMiscellaneousData.json");
string mineMiscellaneousDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "MiscellaneousData", "MineMiscellaneousData", "MineMiscellaneousData.json");
string wallPlaceableDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "WallPlaceableData", "WallPlaceable.json");
string caveDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "CaveData.json");
string specialBackdropDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "SpecialBackdropData", "SpecialBackdropPngInformation.json");
string proceduralMineGenerationDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "ProceduralGenerationData", "ProceduralMineGenerationData.json");
string siteArtifactChanceFunctionalDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "ProceduralGenerationData", "SiteArtifactChanceData", "SiteArtifactChanceFunctionalData", "SiteArtifactChanceFunctionalData.json");
string consumableDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "Consumable", "Consumable.json");
string artifactConditionDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "ArtifactScore", "ArtifactCondition.json");
string artifactEraDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "ArtifactScore", "ArtifactEra.json");
string artifactRarityDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "ArtifactScore", "ArtifactRarity.json");
string artifactThemeMatchingTagCountDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "ArtifactScore", "ArtifactThemeMatchingTagCount.json");
string equipableMeleeDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "Equipable", "EquipableMelee.json");
string equipableRangeDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "Equipable", "EquipableRange.json");
string equipablePickaxeDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "Equipable", "EquipablePickaxe.json");
string cellPlaceableDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "CellPlaceableData", "CellPlaceable.json");

string coreShopFunctionalDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "CoreShopData", "coreShopFunctionalData.json");
string coreShopDescriptiveDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder", "CoreShopData", "coreShopDescriptiveData.json");
string productDataFolderPath = Path.Combine(AppContext.BaseDirectory, "Game Data Folder","CoreProductData",  "productData.json");
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

builder.Services.AddSingleton(new JsonFileDatabase<Shop>(Const.decorationShopDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DecorationShopVariation>(Const.decorationShopVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DecorationOther>(Const.decorationOtherDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DecorationOtherVariation>(Const.decorationOtherVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Sanitation>(Const.sanitationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<SanitationVariation>(Const.sanitationVariationDataFolderPath));

builder.Services.AddSingleton(new JsonFileDatabase<TileVariation>(Const.tileVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<WallVariation>(Const.wallVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<WallpaperVariation>(Const.wallpaperVariationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<StoryScene>(Const.storySceneDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Tutorial>(Const.tutorialDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<PlayerInfo>(Const.playerInfoDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Museum>(Const.museumDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<GuestBuildingParameter>(Const.guestBuilderParameterDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactScore>(Const.artifactScoreDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Time>(Const.timeDataFolderPath));

builder.Services.AddSingleton(new JsonFileDatabase<Mine>(Const.mineDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Inventory>(Const.inventoryDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<MineArtifacts>(Const.mineArtifactsDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<DisplayArtifacts>(Const.displayArtifactDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactStorage>(Const.artifactStorageDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<TradingArtifacts>(Const.tradingArtifactsDataFolderPath));

builder.Services.AddSingleton(new JsonFileDatabase<CellCrackMaterial>(cellCrackMaterialDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<RawArtifactFunctional>(rawArtifactFunctionalDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<RawArtifactDescriptive>(rawArtifactDescriptiveDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Resource>(resourceDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<WallPlaceable>(wallPlaceableDataFolderPath));

builder.Services.AddSingleton(new JsonFileDatabase<MainMenuMiscellaneousData>(mainMenuMiscellaneousDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<MuseumMiscellaneousData>(museumMiscellaneousDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<SettingsMiscellaneousData>(settingsMiscellaneousDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<MineMiscellaneousData>(mineMiscellaneousDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Cave>(caveDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<SpecialBackdropPngInformation>(specialBackdropDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ProceduralMineGenerationData>(proceduralMineGenerationDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<SiteArtifactChanceData>(siteArtifactChanceFunctionalDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Consumable>(consumableDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactCondition>(artifactConditionDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactEra>(artifactEraDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactRarity>(artifactRarityDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<ArtifactThemeMatchingTagCount>(artifactThemeMatchingTagCountDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<EquipableMelee>(equipableMeleeDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<EquipableRange>(equipableRangeDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<EquipablePickaxe>(equipablePickaxeDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<CellPlaceable>(cellPlaceableDataFolderPath));

builder.Services.AddSingleton(new JsonFileDatabase<CoreShopFunctional>(coreShopFunctionalDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<CoreShopDescriptive>(coreShopDescriptiveDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Product>(productDataFolderPath));



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
    Const.wallpaperVariationDataFolderPath,
    Const.sanitationDataFolderPath
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
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IWallPlaceableRepository, WallPlaceableRepository>();
builder.Services.AddScoped<ITimeRepository, TimeRepository>();
builder.Services.AddScoped<IMiscellaneousDataRepository, MiscellaneousDataRepository>();
builder.Services.AddScoped<IMuseumZoneRepository, MuseumZoneRepository>();
builder.Services.AddScoped<ICaveGeneratorRepository, CaveGeneratorRepository>();
builder.Services.AddScoped<ISpecialBackdropRepository, SpecialBackdropRepository>();
builder.Services.AddScoped<IProceduralMineGenerationRepository, ProceduralMineGenerationRepository>();
builder.Services.AddScoped<ISiteArtifactChanceRepository, SiteArtifactChanceRepository>();
builder.Services.AddScoped<IGuestBuildingParameterRepository, GuestBuildingParameterRepository>();
builder.Services.AddScoped<ISanitationRepository, SanitationRepository>();
builder.Services.AddScoped<IConsumableRepository, ConsumableRepository>();
builder.Services.AddScoped<IArtifactConditionRepository, ArtifactConditionRepository>();
builder.Services.AddScoped<IArtifactEraRepository, ArtifactEraRepository>();
builder.Services.AddScoped<IArtifactRarityRepository, ArtifactRarityRepository>();
builder.Services.AddScoped<IArtifactScoreRepository, ArtifactScoreRepository>();
builder.Services.AddScoped<IArtifactThemeMatchingTagCountRepo, ArtifactThemeMatchingTagCountRepo>();
builder.Services.AddScoped<IEquipableRepository, EquipableRepository>();
builder.Services.AddScoped<ICellPlaceableRepository, CellPlaceableRepository>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IVineInformationRepository, VineInformationRepository>();



builder.Services.AddScoped<IMineService, MineService>();
builder.Services.AddScoped<IMineOrdinaryCellGeneratorService, MineOrdinaryCellGeneratorService>();
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
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IWallPlaceableService, WallPlaceableService>();
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IMiscellaneousDataService, MiscellaneousDataService>();
builder.Services.AddScoped<IMuseumZoneService, MuseumZoneService>();
builder.Services.AddScoped<ICaveGeneratorService, CaveGeneratorService>();
builder.Services.AddScoped<ISpecialBackdropService, SpecialBackdropService>();
builder.Services.AddScoped<IProceduralMineGenerationService, ProceduralMineGenerationService>();
builder.Services.AddScoped<ISiteArtifactChanceService, SiteArtifactChanceService>();
builder.Services.AddScoped<IGuestBuilderParameterService, GuestBuilderParameterService>();
builder.Services.AddScoped<ISanitationService, SanitationService>();
builder.Services.AddScoped<IConsumableService, ConsumableService>();
builder.Services.AddScoped<IArtifactScoringService, ArtifactScoringService>();
builder.Services.AddScoped<IArtifactConditionService, ArtifactConditionService>();
builder.Services.AddScoped<IArtifactRarityService, ArtifactRarityService>();
builder.Services.AddScoped<IEquipableService, EquipableService>();
builder.Services.AddScoped<ICellPlaceableService, CellPlaceableService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IVineInformationService, VineInformationService>();


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
Console.WriteLine("Base directory " + AppContext.BaseDirectory);
// Console.WriteLine("Current directory " + AppContext.BaseDirectory);
// Console.WriteLine("Doc folder directory " + Environment.SpecialFolder.MyDocuments);
// Console.WriteLine("persistent directory " +  Environment.SpecialFolder.ApplicationData);

var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),/*"Documents",*/ "Red Thorn Interactive", "Museum Keeper");
if (!Directory.Exists(savePath))
{
    Directory.CreateDirectory(savePath);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();