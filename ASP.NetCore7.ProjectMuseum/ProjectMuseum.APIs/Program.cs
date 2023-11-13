using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;
using ProjectMuseum.Repositories.PlayerInfoRepository;
using ProjectMuseum.Services;
using ProjectMuseum.Services.LoadAndSaveService;
using ProjectMuseum.Services.MineService;
using ProjectMuseum.Services.MuseumService;
using ProjectMuseum.Services.MuseumTileService;
using ProjectMuseum.Services.PlayerInfoService;

var builder = WebApplication.CreateBuilder(args);
string museumTileDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museumTile.json");
string museumDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museum.json");
string exhibitDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "exhibit.json");
string mineDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "mine.json");
string playerInfoDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "PlayerInfo.json");
string saveDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "save.json");


//string dataFolderPath = Path.Combine(AppContext.BaseDirectory, "Dummy Data Folder", "museumTile.json"); //todo for deployment


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new JsonFileDatabase<MuseumTile>(museumTileDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Exhibit>(exhibitDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<PlayerInfo>(playerInfoDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Museum>(museumDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Mine>(mineDataFolderPath));
builder.Services.AddSingleton(new SaveDataJsonFileDatabase(museumTileDataFolderPath, exhibitDataFolderPath, saveDataFolderPath, museumDataFolderPath, playerInfoDataFolderPath));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IMuseumTileRepository, MuseumTileRepository>();
builder.Services.AddScoped<IMineRepository, MineRepository>();
builder.Services.AddScoped<IMuseumRepository, MuseumRepository>();
builder.Services.AddScoped<IExhibitRepository, ExhibitRepository>();
builder.Services.AddScoped<IPlayerInfoRepository, PlayerInfoRepository>();
builder.Services.AddScoped<IMineService, MineService>();
builder.Services.AddScoped<IMuseumTileService, MuseumTileTileService>();
builder.Services.AddScoped<IMuseumService, MuseumService>();
builder.Services.AddScoped<ILoadService, LoadService>();
builder.Services.AddScoped<ISaveService, SaveService>();
builder.Services.AddScoped<IPlayerInfoService, PlayerInfoService>();
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
