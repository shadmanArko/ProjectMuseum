using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.MineCellService;

public class MineCellDestroyer : IMineCellDestroyer
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

    public List<string?> DestroyCellById(string id)
    {
        Mine = _mineRepository.Get().Result;
        var listOfMineIds = new List<string?>();

        var cellToDestroy = Mine.Cells.FirstOrDefault(cell => cell.Id == id);
        // For Top Neighbour
        if (cellToDestroy.PositionY != YSize-1)
        {
            listOfMineIds.Add(ChangeBottomNeighbourTopBrokenSide(cellToDestroy.PositionX, cellToDestroy.PositionY)); 
        }

        // For Right Neighbour
        if (cellToDestroy.PositionX != XSize-1)
        {
            listOfMineIds.Add(ChangeRightNeighbourLeftBrokenSide(cellToDestroy.PositionX, cellToDestroy.PositionY));
        }

        // For Bottom Neighbour
        if (cellToDestroy.PositionY != 0)
        {
            listOfMineIds.Add(ChangeTopNeighbourBottomBrokenSide(cellToDestroy.PositionX, cellToDestroy.PositionY));
        }
        
        // For Left Neighbour
        if (cellToDestroy.PositionX != 0)
        {
            listOfMineIds.Add(ChangeLeftNeighbourRightBrokenSide(cellToDestroy.PositionX, cellToDestroy.PositionY));
        }
        
        //todo change the all data of CellToDestroy

        _mineRepository.Update(Mine);
        return listOfMineIds;
    }

    private string? ChangeTopNeighbourBottomBrokenSide(int positionX, int positionY)
    {
        var cell = Mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 0) && cell1.PositionY == (positionY + 1));
        if (cell != null) cell.BottomBrokenSide = true;
        return cell?.Id;
    }
    
    private string? ChangeRightNeighbourLeftBrokenSide(int positionX, int positionY)
    {
        var cell = Mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 1) && cell1.PositionY == (positionY + 0));
        if (cell != null) cell.LeftBrokenSide = true;
        return cell?.Id;
    }
    
    private string? ChangeBottomNeighbourTopBrokenSide(int positionX, int positionY)
    {
        var cell = Mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX + 0) && cell1.PositionY == (positionY - 1));
        if (cell != null) cell.TopBrokenSide = true;
        return cell?.Id;
    }
    
    private string? ChangeLeftNeighbourRightBrokenSide(int positionX, int positionY)
    {
        var cell = Mine.Cells.FirstOrDefault(cell1 =>
            cell1.PositionX == (positionX - 1) && cell1.PositionY == (positionY + 0));
        if (cell != null) cell.RightBrokenSide = true;
        return cell?.Id;
    }
}

public interface IMineCellDestroyer
{
    
}