using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.DecorationOtherRepository;

public interface IDecorationOtherRepository
{
    Task<DecorationOther> Insert(DecorationOther DecorationOther);
    Task<DecorationOther> Update(string id, DecorationOther DecorationOther);
    Task<DecorationOther?> GetById(string id);
    Task<List<DecorationOther>?> GetAll();
    Task<List<DecorationOtherVariation>?> GetAllDecorationOtherVariations();
    Task<DecorationOtherVariation?> GetDecorationOtherVariation(string variationName);
    Task<List<DecorationOther>?> GetAllDecorationOthers();
    Task<DecorationOther?> Delete(string id);
    Task<List<DecorationOther>?> DeleteAllDecorationOthers();
    Task<DecorationOther?> AddArtifactToDecorationOther(string DecorationOtherId, string artifactId, int slot);
    Task<DecorationOther?> RemoveArtifactFromDecorationOther(string DecorationOtherId, string artifactId, int slot);
}