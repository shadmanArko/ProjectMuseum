using ProjectMuseum.Models;
using ProjectMuseum.Models.TransportChainBlocks;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.TransportChainBlockRepository;

public class MineTransportChainBlockRepository : IMineTransportChainBlockRepository
{
    private readonly JsonFileDatabase<Mine> _mineDatabase;
    private readonly JsonFileDatabase<TransportChainBlock> _transportChainBlockDatabase;

    public MineTransportChainBlockRepository(JsonFileDatabase<TransportChainBlock> transportChainBlockDatabase, JsonFileDatabase<Mine> mineDatabase)
    {
        _transportChainBlockDatabase = transportChainBlockDatabase;
        _mineDatabase = mineDatabase;
    }

    public async Task<TransportChainBlock> AddTransportChainBlockToMine(string subCategory)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        var chainBlocks = await _transportChainBlockDatabase.ReadDataAsync();
        var chainBlock = chainBlocks?.FirstOrDefault(block => block.SubCategory == subCategory);
        chainBlock!.Id = Guid.NewGuid().ToString();
        mine?.TransportChainBlocks.Add(chainBlock);

        if (mine != null) await _mineDatabase.WriteDataAsync(mines!);
        return chainBlock;
    }

    public async Task<TransportChainBlock?> RemoveTransportChainBlockFromMine(string transportChainBlockId)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        var blockChains = mine?.TransportChainBlocks;
        var blockChainToRemove = blockChains?.FirstOrDefault(block => block.Id == transportChainBlockId);
        if (blockChainToRemove != null)
            blockChains?.Remove(blockChainToRemove);

        if (mines != null)
            await _mineDatabase.WriteDataAsync(mines);
        return blockChainToRemove;
    }
}