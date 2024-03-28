using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SiteArtifactRepository;

public interface ISiteArtifactChanceRepository
{
    Task<SiteArtifactChanceData> GetBySite(string siteName);
}