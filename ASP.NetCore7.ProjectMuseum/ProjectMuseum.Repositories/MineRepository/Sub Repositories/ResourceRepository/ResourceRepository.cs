using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ResourceRepository;

public class ResourceRepository : IResourceRepository
{
    private readonly JsonFileDatabase<Mine> _mineDatabase;
    private readonly JsonFileDatabase<Resource> _resourceDatabase;

    public ResourceRepository(JsonFileDatabase<Mine> mineDatabase, JsonFileDatabase<Resource> resourceDatabase)
    {
        _mineDatabase = mineDatabase;
        _resourceDatabase = resourceDatabase;
    }

    public async Task<Resource> AddResourceToMine(string variant, int posX, int posY)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        var resources = await _resourceDatabase.ReadDataAsync();
        var resource = resources?.FirstOrDefault(resource1 => resource1.Variant == variant);
        resource!.Id = Guid.NewGuid().ToString();
        resource.PositionX = posX;
        resource.PositionY = posY;
        mine?.Resources.Add(resource);
        if (mine != null) await _mineDatabase.WriteDataAsync(mines!);
        return resource;
    }

    public async Task<Resource?> RemoveResourceFromMine(string resourceId)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        var resources = mine?.Resources;
        var resourceToRemove = resources?.FirstOrDefault(resource1 => resource1.Id == resourceId);
        if (resourceToRemove != null) resources?.Remove(resourceToRemove);
        var cell = mine?.Cells.FirstOrDefault(cell => resourceToRemove != null && cell.PositionX == resourceToRemove.PositionX && cell.PositionY == resourceToRemove.PositionY);
        if (cell != null) cell.HasResource = false;
        if (mine != null) await _mineDatabase.WriteDataAsync(mines!);

        return resourceToRemove;
    }

    public async Task<List<Resource>?> GetAllResources()
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        var resources = mine?.Resources;
        return resources;
    }
}