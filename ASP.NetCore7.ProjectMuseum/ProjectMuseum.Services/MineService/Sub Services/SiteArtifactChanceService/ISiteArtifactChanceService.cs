using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.SiteArtifactChanceService;

public interface ISiteArtifactChanceService
{
    Task<SiteArtifactChanceData> GetSiteArtifactChanceDataBySite(string siteName);
}