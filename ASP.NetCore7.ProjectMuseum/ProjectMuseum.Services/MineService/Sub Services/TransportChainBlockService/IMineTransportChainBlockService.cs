using ProjectMuseum.Models;

namespace ProjectMuseum.Services.MineService.Sub_Services.TransportChainBlockService;

public interface IMineTransportChainBlockService
{
    Task<Equipable?> SendTransportChainBlockToInventory(string transportChainBlockId);
}