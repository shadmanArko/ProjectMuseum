using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ResourceRepository;

public interface IResourceRepository
{
    Task<Resource> AddResourceToMine(string variant);
    Task<Resource?> RemoveResourceFromMine(string resourceId);
}