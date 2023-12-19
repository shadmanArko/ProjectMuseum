using ProjectMuseum.Models;

namespace ProjectMuseum.DTOs;

public class RawArtifactDTO
{
    public List<RawArtifactFunctional> RawArtifactFunctionals { get; set; }
    public List<RawArtifactDescriptive> RawArtifactDescriptives { get; set; }
}