using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.ExhibitRepository;
using ProjectMuseum.Repositories.MuseumRepository;
using ProjectMuseum.Repositories.MuseumTileRepository;
using ProjectMuseum.Services;
using ProjectMuseum.Services.MuseumTileService;

var builder = WebApplication.CreateBuilder(args);
string museumTileDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museumTile.json");
string museumDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "museum.json");
string exhibitDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "exhibit.json");
string saveDataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "save.json");

//string dataFolderPath = Path.Combine(AppContext.BaseDirectory, "Dummy Data Folder", "museumTile.json"); //todo for deployment


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new JsonFileDatabase<MuseumTile>(museumTileDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Exhibit>(exhibitDataFolderPath));
builder.Services.AddSingleton(new JsonFileDatabase<Museum>(exhibitDataFolderPath));
builder.Services.AddSingleton(new SaveDataJsonFileDatabase(museumTileDataFolderPath,exhibitDataFolderPath, saveDataFolderPath));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IMuseumTileRepository, MuseumTileRepository>();
builder.Services.AddScoped<IMuseumRepository, MuseumRepository>();
builder.Services.AddScoped<IExhibitRepository, ExhibitRepository>();
builder.Services.AddScoped<IMuseumService, MuseumService>();
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
