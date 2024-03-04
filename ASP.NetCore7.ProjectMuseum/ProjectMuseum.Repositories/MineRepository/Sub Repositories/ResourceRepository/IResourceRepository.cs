using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ResourceRepository;

public interface IResourceRepository
{
    Task<Resource> AddResourceToMine(string variant, int posX, int posY);
    Task<Resource?> RemoveResourceFromMine(string resourceId);
    Task<List<Resource>?> GetAllResources();
}