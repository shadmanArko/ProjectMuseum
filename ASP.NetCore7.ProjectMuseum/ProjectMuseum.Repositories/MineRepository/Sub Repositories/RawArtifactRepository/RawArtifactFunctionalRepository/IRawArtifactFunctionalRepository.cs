using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactFunctionalRepository;

public interface IRawArtifactFunctionalRepository
{
    Task<List<RawArtifactFunctional>?> GetAllRawArtifactFunctional();
}