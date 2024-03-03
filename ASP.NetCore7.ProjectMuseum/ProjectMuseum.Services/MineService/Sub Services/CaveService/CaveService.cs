using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.CaveRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.CaveService;

public class CaveService : ICaveService
{
    private readonly ICaveRepository _caveRepository;

    public CaveService(ICaveRepository caveRepository)
    {
        _caveRepository = caveRepository;
    }


    public async Task<Cave> GenerateCave(int xMin, int xMax, int yMin, int yMax)
    {
        var cave = await _caveRepository.GenerateCave(xMin, xMax, yMin, yMax);
        return cave;
    }
}