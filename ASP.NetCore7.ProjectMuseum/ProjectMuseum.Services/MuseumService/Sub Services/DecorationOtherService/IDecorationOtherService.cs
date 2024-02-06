using ProjectMuseum.Models;

namespace ProjectMuseum.Services.DecorationOtherService;

public interface IDecorationOtherService
{
    Task<List<DecorationOtherVariation>?> GetAllDecorationOtherVariations();
    Task<DecorationOtherVariation?> GetDecorationOtherVariation(string variationName);
    Task<List<DecorationOther>?> DeleteAllDecorationOthers();
    Task<List<DecorationOther>?> GetAllDecorationOthers();
    
}