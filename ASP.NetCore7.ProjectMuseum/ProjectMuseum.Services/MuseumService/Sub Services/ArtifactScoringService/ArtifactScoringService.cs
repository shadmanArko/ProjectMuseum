using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactEraRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactRarityRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactThemeMatchingTagCountRepository;
using ProjectMuseum.Services.MuseumService.Sub_Services.MuseumZoneService;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService;

public class ArtifactScoringService : IArtifactScoringService
{
    private readonly IArtifactConditionRepository _artifactConditionRepository;
    private readonly IArtifactEraRepository _artifactEraRepository;
    private readonly IArtifactRarityRepository _artifactRarityRepository;
    private readonly IArtifactThemeMatchingTagCountRepo _artifactThemeMatchingTagCountRepo;
    private readonly IMuseumZoneService _zoneService;

    public ArtifactScoringService(IArtifactConditionRepository artifactConditionRepository, IArtifactEraRepository artifactEraRepository, IArtifactRarityRepository artifactRarityRepository, IArtifactThemeMatchingTagCountRepo artifactThemeMatchingTagCountRepo, IMuseumZoneService zoneService)
    {
        _artifactConditionRepository = artifactConditionRepository;
        _artifactEraRepository = artifactEraRepository;
        _artifactRarityRepository = artifactRarityRepository;
        _artifactThemeMatchingTagCountRepo = artifactThemeMatchingTagCountRepo;
        _zoneService = zoneService;
    }

    public async Task<float> GetArtifactScoreWhichIsNotInZone(Artifact artifact)
    {
        var conditionValue = await _artifactConditionRepository.GetConditionValueByCondition(artifact.Condition);
        var rarityValue = await _artifactRarityRepository.GetRarityValueByRarity(artifact.Rarity);
        var eraValue = await _artifactEraRepository.GetEraValueByEra(artifact.Era);
        var themeValue = await _artifactThemeMatchingTagCountRepo.GetArtifactThemeMatchingMultiplierByThemeCount(0);
        float artifactScore = 5 * conditionValue * themeValue * rarityValue * eraValue;
        return artifactScore;
    }

    public async Task GetThemeValueOfZone(string museumZoneId)
    {
        // Get List of Zones
        // Get Exhibit from all the zone
        // Get All artifact from Each Zone
        // Change the Artifact Score From Each Zone
        // Store in a temp place all the artifacts which is in zone
        // Get the artifacts which are not in zone
        // Change the Artifact Score of those artifacts
    }

    public async Task<float> GetThemeMultiplierUsingZoneId(List<Artifact> artifacts)
    {
        Dictionary<string, int> tagCounter = new Dictionary<string, int>();
        foreach (var artifact in artifacts)
        {
            if (tagCounter.ContainsKey(artifact.Era))
            {
                tagCounter[artifact.Era]++;
            } else
            {
                tagCounter[artifact.Era] = 1;
            }
            
            if (tagCounter.ContainsKey(artifact.Region))
            {
                tagCounter[artifact.Region]++;
            } else
            {
                tagCounter[artifact.Region] = 1;
            }
            
            if (tagCounter.ContainsKey(artifact.Object))
            {
                tagCounter[artifact.Object]++;
            } else
            {
                tagCounter[artifact.Object] = 1;
            }
            
            if (tagCounter.ContainsKey(artifact.ObjectClass))
            {
                tagCounter[artifact.ObjectClass]++;
            } else
            {
                tagCounter[artifact.ObjectClass] = 1;
            }
            
            if (tagCounter.ContainsKey(artifact.ObjectSize))
            {
                tagCounter[artifact.ObjectSize]++;
            } else
            {
                tagCounter[artifact.ObjectSize] = 1;
            }

            foreach (var material in artifact.Materials)
            {
                if (tagCounter.ContainsKey(material))
                {
                    tagCounter[material]++;
                } else
                {
                    tagCounter[material] = 1;
                }
            }
            
            
        }
        
        int commonThemeCount = tagCounter.Count(kv => kv.Value == artifacts.Count);

        var themeMultiplier = await _artifactThemeMatchingTagCountRepo.GetArtifactThemeMatchingMultiplierByThemeCount(commonThemeCount);
        return themeMultiplier;
    }
}