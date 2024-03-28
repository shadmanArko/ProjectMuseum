using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.ProceduralMineGenerationService.MineOrdinaryCellGeneratorService;

public interface IMineOrdinaryCellGeneratorService
{
    Task<Mine> GenerateMineCellData(int xSize, int ySize, int cellSize);
}