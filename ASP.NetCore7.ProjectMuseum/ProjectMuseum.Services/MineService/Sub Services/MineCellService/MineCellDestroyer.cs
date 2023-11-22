using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellService;

public class MineCellDestroyer
{
    public int XSize = 20;
    public int YSize = 20;
    public Mine Mine;

    private readonly IMineRepository _mineRepository;

    public MineCellDestroyer(IMineRepository mineRepository)
    {
        _mineRepository = mineRepository;
        Mine = _mineRepository.Get().Result;
    }

    public void DestroyCellById(string id)
    {
        Mine = _mineRepository.Get().Result;
        
        var cellToDestroy = Mine.Cells.FirstOrDefault(cell => cell.Id == id);
        if (cellToDestroy == null) return;
        
        // For Top Neighbour
        if (cellToDestroy.PositionY != YSize-1)
        {
            ChangeBottomNeighbourTopBrokenSide(cellToDestroy.PositionX, cellToDestroy.PositionY);
        }

        // For Right Neighbour
        if (cellToDestroy.PositionX != XSize-1)
        {
            ChangeRightNeighbourLeftBrokenSide(cellToDestroy.PositionX, cellToDestroy.PositionY);
        }

        // For Bottom Neighbour
        if (cellToDestroy.PositionY != 0)
        {
            ChangeTopNeighbourBottomBrokenSide(cellToDestroy.PositionX, cellToDestroy.PositionY);
        }
        
        // For Left Neighbour
        if (cellToDestroy.PositionX != 0)
        {
            ChangeLeftNeighbourRightBrokenSide(cellToDestroy.PositionX, cellToDestroy.PositionY);
        }
        
        //todo change the all data of CellToDestroy

        _mineRepository.Update(Mine);
    }

    private void ChangeTopNeighbourBottomBrokenSide(int positionX, int positionY)
    {
        var cell = Mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 0) && cell1.PositionY == (positionY + 1));
        if (cell != null) cell.BottomBrokenSide = true;
    }
    
    private void ChangeRightNeighbourLeftBrokenSide(int positionX, int positionY)
    {
        var cell = Mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 1) && cell1.PositionY == (positionY + 0));
        if (cell != null) cell.LeftBrokenSide = true;
    }
    
    private void ChangeBottomNeighbourTopBrokenSide(int positionX, int positionY)
    {
        var cell = Mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 0) && cell1.PositionY == (positionY - 1));
        if (cell != null) cell.TopBrokenSide = true;
    }
    
    private void ChangeLeftNeighbourRightBrokenSide(int positionX, int positionY)
    {
        var cell = Mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX - 1) && cell1.PositionY == (positionY + 0));
        if (cell != null) cell.RightBrokenSide = true;
    }
}