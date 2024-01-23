using ProjectMuseum.Models.TransportChainBlocks;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.TransportChainBlockRepository;

public interface IMineTransportChainBlockRepository
{
    Task<TransportChainBlock> AddTransportChainBlockToMine(string subCategory);
    Task<TransportChainBlock?> RemoveTransportChainBlockFromMine(string transportChainBlockId);
}