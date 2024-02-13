using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.WallPlaceableRepository;

public class WallPlaceableRepository : IWallPlaceableRepository
{
    private readonly JsonFileDatabase<Mine> _mineDatabase;

    public WallPlaceableRepository(JsonFileDatabase<Mine> mineDatabase)
    {
        _mineDatabase = mineDatabase;
    }

    public async Task<WallPlaceable> AddWallPlaceable(WallPlaceable wallPlaceable)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        
        var cell = mine?.Cells.FirstOrDefault(cell1 => cell1.Id == wallPlaceable.OccupiedCellIds[0]);
        wallPlaceable.PositionX = cell!.PositionX;
        wallPlaceable.PositionY = cell.PositionY;

        foreach (var occupiedCellId in wallPlaceable.OccupiedCellIds)
        {
            var tempCell = mine?.Cells.FirstOrDefault(cell1 => cell1.Id == occupiedCellId);
            tempCell!.HasWallPlaceable = true;
        }
        
        mine?.WallPlaceables.Add(wallPlaceable);
        if (mine != null) await _mineDatabase.WriteDataAsync(mines!);
        
        return wallPlaceable;
    }

    public async Task<WallPlaceable?> RemoveWallPlaceable(string wallPlaceableId)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        
        var wallPlaceables = mine?.WallPlaceables;
        var wallPlaceableToRemove = wallPlaceables?.FirstOrDefault(wallPlaceable1 => wallPlaceable1.Id == wallPlaceableId);

        foreach (var occupiedCellId in wallPlaceableToRemove!.OccupiedCellIds)
        {
            var tempCell = mine?.Cells.FirstOrDefault(cell1 => cell1.Id == occupiedCellId);
            tempCell!.HasWallPlaceable = false;
        }

        wallPlaceables?.Remove(wallPlaceableToRemove);
        if (mine != null) await _mineDatabase.WriteDataAsync(mines!);

        return wallPlaceableToRemove;
    }
}