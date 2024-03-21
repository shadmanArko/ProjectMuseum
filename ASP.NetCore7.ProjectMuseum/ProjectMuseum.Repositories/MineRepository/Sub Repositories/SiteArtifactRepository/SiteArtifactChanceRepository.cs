using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SiteArtifactRepository;

public class SiteArtifactChanceRepository : ISiteArtifactChanceRepository
{
    private readonly JsonFileDatabase<SiteArtifactChanceData> _siteArtFileDatabase;

    public SiteArtifactChanceRepository(JsonFileDatabase<SiteArtifactChanceData> siteArtFileDatabase)
    {
        _siteArtFileDatabase = siteArtFileDatabase;
    }

    public async Task<SiteArtifactChanceData> GetBySite(string siteName)
    {
        var siteArtifactDatas = await _siteArtFileDatabase.ReadDataAsync();
        var siteArtifactData = siteArtifactDatas!.FirstOrDefault(tempSite => tempSite.Site == siteName);
        return siteArtifactData!;
    }
}