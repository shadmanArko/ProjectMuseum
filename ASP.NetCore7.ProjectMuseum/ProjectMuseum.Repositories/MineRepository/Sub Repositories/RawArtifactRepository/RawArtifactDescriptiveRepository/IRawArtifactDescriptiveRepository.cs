using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactDescriptiveRepository;

public interface IRawArtifactDescriptiveRepository
{
    Task<List<RawArtifactDescriptive>?> GetAllRawArtifactDescriptive();
}