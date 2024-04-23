using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactThemeMatchingTagCountRepository;

public class ArtifactThemeMatchingTagCountRepo : IArtifactThemeMatchingTagCountRepo
{
    private readonly JsonFileDatabase<ArtifactThemeMatchingTagCount> _artifactThemeMatchingTagCountDatabase;

    public ArtifactThemeMatchingTagCountRepo(JsonFileDatabase<ArtifactThemeMatchingTagCount> artifactThemeMatchingTagCountDatabase)
    {
       _artifactThemeMatchingTagCountDatabase = artifactThemeMatchingTagCountDatabase;
    }

    public async Task<float> GetArtifactThemeMatchingMultiplierByThemeCount(int themeMatchCount)
    {
        var artifactThemeMatchingTagCounts = await _artifactThemeMatchingTagCountDatabase.ReadDataAsync();
        var artifactThemeMatchingTagCount =
            artifactThemeMatchingTagCounts.FirstOrDefault(count => count.TagMatchedTheme == themeMatchCount);
        return artifactThemeMatchingTagCount.ScoreMultiplier;

    }
}