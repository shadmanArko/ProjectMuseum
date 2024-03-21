using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SiteArtifactRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.SiteArtifactChanceService;

public class SiteArtifactChanceService : ISiteArtifactChanceService
{
    private readonly ISiteArtifactChanceRepository _siteArtifactChanceRepository;

    public SiteArtifactChanceService(ISiteArtifactChanceRepository siteArtifactChanceRepository)
    {
        _siteArtifactChanceRepository = siteArtifactChanceRepository;
    }

    public async Task<SiteArtifactChanceData> GetSiteArtifactChanceDataBySite(string siteName)
    {
        var siteArtifactData = await _siteArtifactChanceRepository.GetBySite(siteName);
        return siteArtifactData;
    }
}