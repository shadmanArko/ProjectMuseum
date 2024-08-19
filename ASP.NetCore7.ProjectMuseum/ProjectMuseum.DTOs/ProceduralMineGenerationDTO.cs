using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne;


namespace ProjectMuseum.DTOs;

public class ProceduralMineGenerationDto
{
    public ProceduralMineGenerationData ProceduralMineGenerationData { get; set; }
    public List<SpecialBackdropPngInformation> SpecialBackdropPngInformations { get; set; }
    public List<ArtifactCondition> ArtifactConditions { get; set; }
    public List<ArtifactRarity> ArtifactRarities { get; set; }
    public List<SiteArtifactChanceData> SiteArtifactChances { get; set; }

    public List<Resource> Resources { get; set; }
    
}