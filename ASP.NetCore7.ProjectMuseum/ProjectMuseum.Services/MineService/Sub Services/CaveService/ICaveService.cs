using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.CaveService;

public interface ICaveService
{
    Task<Cave> GenerateCave(int xMin, int xMax, int yMin, int yMax);
}