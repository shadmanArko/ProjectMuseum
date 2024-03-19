using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.SpecialBackdropRepository;

public interface ISpecialBackdropRepository
{
    Task<List<SpecialBackdropPngInformation>> SetSpecialBackdrops(List<SpecialBackdropPngInformation> specialBackdrops);
}