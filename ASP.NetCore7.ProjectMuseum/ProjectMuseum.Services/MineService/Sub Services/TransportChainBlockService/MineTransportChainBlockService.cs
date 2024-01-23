using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.TransportChainBlockRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.TransportChainBlockService;

public class MineTransportChainBlockService : IMineTransportChainBlockService
{
    private readonly IMineTransportChainBlockRepository _mineTransportChainBlockRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public MineTransportChainBlockService(IMineTransportChainBlockRepository mineTransportChainBlockRepository, IInventoryRepository inventoryRepository)
    {
        _mineTransportChainBlockRepository = mineTransportChainBlockRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Equipable?> SendTransportChainBlockToInventory(string transportChainBlockId)
    {
        var blockChain = await _mineTransportChainBlockRepository.RemoveTransportChainBlockFromMine(transportChainBlockId);
        var equipable = await _inventoryRepository.AddEquipable("TransportChainBlock", blockChain!.SubCategory,
            blockChain.SmallPngPath);
        return equipable;
    }
}