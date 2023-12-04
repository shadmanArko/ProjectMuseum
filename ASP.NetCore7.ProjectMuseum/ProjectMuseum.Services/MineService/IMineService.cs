using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService;

public interface IMineService
{
    Task<Mine> UpdateMine(Mine mine);
    Task<Mine> GetMineData();
    Task<Mine> AssignArtifactsToMine();
}