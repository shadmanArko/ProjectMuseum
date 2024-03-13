using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.CaveService;

public interface ICaveGeneratorService
{
    Task<Cave> GenerateCave(int xMin, int xMax, int yMin, int yMax, int stalagmiteCount, int stalactiteCount);
}