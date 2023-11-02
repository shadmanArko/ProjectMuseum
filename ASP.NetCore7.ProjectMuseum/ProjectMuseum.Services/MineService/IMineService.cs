using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService;

public interface IMineService
{
    Task<Mine> GetMineData();
}