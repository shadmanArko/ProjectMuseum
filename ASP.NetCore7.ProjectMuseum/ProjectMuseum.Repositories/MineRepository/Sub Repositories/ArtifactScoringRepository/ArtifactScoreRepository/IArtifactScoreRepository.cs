using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactScoreRepository;

public interface IArtifactScoreRepository
{
    Task<List<ArtifactScore>?> GetAllArtifactScore();
    Task<List<ArtifactScore>?> UpdateArtifactScore(List<ArtifactScore> artifactScores);
}