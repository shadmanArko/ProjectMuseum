using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellService;

public interface IMineCellGenerator
{
    Task<Mine> GenerateMineCellData();
}