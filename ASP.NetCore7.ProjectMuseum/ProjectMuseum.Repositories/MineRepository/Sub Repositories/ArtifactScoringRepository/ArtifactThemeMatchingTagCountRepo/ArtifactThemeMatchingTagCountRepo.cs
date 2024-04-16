using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactThemeMatchingTagCountRepository;

public class ArtifactThemeMatchingTagCountRepo : IArtifactThemeMatchingTagCountRepo
{
    private readonly JsonFileDatabase<ArtifactThemeMatchingTagCount> _artifactThemeMatchingTagCountDatabase;

    public ArtifactThemeMatchingTagCountRepo(JsonFileDatabase<ArtifactThemeMatchingTagCount> artifactThemeMatchingTagCountDatabase)
    {
       _artifactThemeMatchingTagCountDatabase = artifactThemeMatchingTagCountDatabase;
    }

    public Task<int> GetArtifactThemeMatchingTagCountByTheme(string theme)
    {
        throw new NotImplementedException();
    }
}