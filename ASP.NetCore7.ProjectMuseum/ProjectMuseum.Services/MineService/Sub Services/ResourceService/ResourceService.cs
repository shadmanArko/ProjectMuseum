using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ResourceRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.ResourceService;

public class ResourceService : IResourceService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly IMineRepository _mineRepository;

    private Random _rand = new();

    public ResourceService(IResourceRepository resourceRepository, IInventoryRepository inventoryRepository, IMineRepository mineRepository)
    {
        _resourceRepository = resourceRepository;
        _inventoryRepository = inventoryRepository;
        _mineRepository = mineRepository;
    }

    public async Task<InventoryItem?> SendResourceFromMineToInventory(string resourceId)
    {
        var resource = await _resourceRepository.RemoveResourceFromMine(resourceId);
        var inventoryItem = await _inventoryRepository.AddInventoryItem("Resource", resource!.Variant, resource.PNGPath);
        return inventoryItem;
    }

    public async Task<Mine> AssignResourcesToMine()
    {
        var mine = await _mineRepository.Get();
        var resources = mine.Resources;
        foreach (Cell cell in mine.Cells)
        {
            cell.HasResource = false;
        }

        foreach (var resource in resources)
        {
            var cell = mine.Cells.FirstOrDefault(cell1 =>
                cell1.PositionX == resource.PositionX && cell1.PositionY == resource.PositionY);
            if (cell != null) cell.HasResource = true;
        }

        await _mineRepository.Update(mine);
        return mine;
    }

    public async Task<List<Resource>> GenerateResources(List<string> variants)
    {
        var mine = await _mineRepository.Get();
        var cells = new List<Cell>();
        foreach (var mineCell in mine.Cells)
        {
            mineCell.HasResource = false;
            if(mineCell.HasCave || mineCell.HasArtifact || !mineCell.IsInstantiated || mineCell.IsBroken || !mineCell.IsBreakable) continue;
            cells.Add(mineCell);
        }
        mine.Resources.Clear();
        
        
        var numberOfRootNodes = _rand.Next(20,30);

        for (var i = 0; i < numberOfRootNodes; i++)
        {
            var resourceCells = new List<Cell>();
            var cell = GetRandomCell(cells);
            resourceCells.Add(cell);
            cells.Remove(cell);
            
            var rootNodeVariant = variants[_rand.Next(0, variants.Count)];
            await _resourceRepository.AddResourceToMine(rootNodeVariant, cell.PositionX, cell.PositionY);

            var resourceBranches = rootNodeVariant.Equals("Iron") ? _rand.Next(3, 5) : _rand.Next(5, 8);
            var currentBranchCell = cell;
            for (var j = 0; j <= resourceBranches; j++)
            {
                currentBranchCell = GetRandomAdjacentCell(cells, currentBranchCell);
                if(resourceCells.Contains(currentBranchCell)) continue;
                resourceCells.Add(currentBranchCell);
                Console.WriteLine($"Added {rootNodeVariant} Resource Cell {currentBranchCell.PositionX}, {currentBranchCell.PositionY}");
            }
            
            foreach (var resourceCell in resourceCells)
            {
                var resource = await _resourceRepository.AddResourceToMine(rootNodeVariant, resourceCell.PositionX,
                    resourceCell.PositionY);
                mine.Resources.Add(resource);
                FormResourceDistanceOfFourTiles(cells, resourceCell);
            }
        }

        await _mineRepository.Update(mine);
        return mine.Resources;
    }

    private Cell GetRandomCell(List<Cell> cells)
    {
        return cells[_rand.Next(0,cells.Count)];
    }

    private Cell GetRandomAdjacentCell(List<Cell> cells, Cell currentCell)
    {
        var xMin = currentCell.PositionX - 1;
        var xMax = currentCell.PositionX + 1;
        var yMin = currentCell.PositionY - 1;
        var yMax = currentCell.PositionY + 1;

        var adjacentCells = new List<Cell>();
        
        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var cell = cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                if(cell == null) continue;
                if(cell == currentCell) continue;
                if (cell.HasResource || cell.HasCave || cell.HasArtifact
                    || !cell.IsInstantiated || cell.IsBroken || !cell.IsBreakable) continue;
                adjacentCells.Add(cell);
            }
        }

        return adjacentCells[_rand.Next(0, adjacentCells.Count - 1)];
    }

    private void FormResourceDistanceOfFourTiles(List<Cell> cells, Cell currentCell)
    {
        var xMin = currentCell.PositionX - 4;
        var xMax = currentCell.PositionX + 4;
        var yMin = currentCell.PositionY - 4;
        var yMax = currentCell.PositionY + 4;
        
        for (var i = xMin; i <= xMax; i++)
        {
            for (var j = yMin; j <= yMax; j++)
            {
                var cell = cells.FirstOrDefault(tempCell => tempCell.PositionX == i && tempCell.PositionY == j);
                if(cell == null) continue;
                if(cell != currentCell) continue;
                cells.Remove(cell);
            }
        }

    }
}