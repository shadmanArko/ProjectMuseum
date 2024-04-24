using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactRarityRepository;

public interface IArtifactRarityRepository
{
    Task<float> GetRarityValueByRarity(string rarity);
    Task<List<ArtifactRarity>> GetAllArtifactRarity();
}