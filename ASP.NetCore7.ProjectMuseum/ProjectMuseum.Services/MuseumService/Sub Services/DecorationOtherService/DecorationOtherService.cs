using ProjectMuseum.Models;
using ProjectMuseum.Repositories.DecorationOtherRepository;
using ProjectMuseum.Services.DecorationOtherService;

namespace ProjectMuseum.Services.DecorationOtherServices;

public class DecorationOtherService : IDecorationOtherService
{
    private IDecorationOtherRepository _decorationOtherRepository;

    public DecorationOtherService(IDecorationOtherRepository decorationOtherRepository)
    {
        _decorationOtherRepository = decorationOtherRepository;
    }


    public async Task<List<DecorationOtherVariation>?> GetAllDecorationOtherVariations()
    {
        return await _decorationOtherRepository.GetAllDecorationOtherVariations();
    }

    public async Task<DecorationOtherVariation?> GetDecorationOtherVariation(string variationName)
    {
        return await _decorationOtherRepository.GetDecorationOtherVariation(variationName);

    }

    public  async Task<List<DecorationOther>?> DeleteAllDecorationOthers()
    {
        return await _decorationOtherRepository.DeleteAllDecorationOthers();

    }

    public async Task<List<DecorationOther>?> GetAllDecorationOthers()
    {
        return await _decorationOtherRepository.GetAllDecorationOthers();

    }
}