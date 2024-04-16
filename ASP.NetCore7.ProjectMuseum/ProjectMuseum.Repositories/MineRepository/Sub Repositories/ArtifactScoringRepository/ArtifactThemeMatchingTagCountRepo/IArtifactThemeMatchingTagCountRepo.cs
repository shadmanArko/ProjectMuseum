namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactThemeMatchingTagCountRepository;

public interface IArtifactThemeMatchingTagCountRepo
{
    Task<int> GetArtifactThemeMatchingTagCountByTheme(string theme);
}