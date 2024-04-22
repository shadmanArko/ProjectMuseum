using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactEraRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactRarityRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactScoreRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.ArtifactScoringRepository.ArtifactThemeMatchingTagCountRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.RawArtifactRepository.RawArtifactFunctionalRepository;
using ProjectMuseum.Services.ExhibitService;
using ProjectMuseum.Services.MineService.Sub_Services.RawArtifactService;
using ProjectMuseum.Services.MuseumService.Sub_Services.DisplayArtifactService;
using ProjectMuseum.Services.MuseumService.Sub_Services.MuseumZoneService;
using ProjectMuseum.Services.MuseumTileService;

namespace ProjectMuseum.Services.MuseumService.Sub_Services.ArtifactScoringService;

public class ArtifactScoringService : IArtifactScoringService
{
    private readonly IArtifactConditionRepository _artifactConditionRepository;
    private readonly IArtifactEraRepository _artifactEraRepository;
    private readonly IArtifactRarityRepository _artifactRarityRepository;
    private readonly IArtifactThemeMatchingTagCountRepo _artifactThemeMatchingTagCountRepo;
    private readonly IMuseumZoneService _zoneService;
    private readonly IDisplayArtifactService _displayArtifactService;
    private readonly IArtifactScoreRepository _artifactScoreRepository;
    private readonly IMuseumTileService _museumTileService;
    private readonly IExhibitService _exhibitService;
    private readonly IRawArtifactFunctionalService _rawArtifactFunctionalService;
    
    public ArtifactScoringService(IArtifactConditionRepository artifactConditionRepository, IArtifactEraRepository artifactEraRepository, IArtifactRarityRepository artifactRarityRepository, IArtifactThemeMatchingTagCountRepo artifactThemeMatchingTagCountRepo, IMuseumZoneService zoneService, IDisplayArtifactService displayArtifactService, IArtifactScoreRepository artifactScoreRepository, IMuseumTileService museumTileService, IExhibitService exhibitService, IRawArtifactFunctionalService rawArtifactFunctionalService, IRawArtifactFunctionalRepository rawArtifactFunctionalRepository)
    {
        _artifactConditionRepository = artifactConditionRepository;
        _artifactEraRepository = artifactEraRepository;
        _artifactRarityRepository = artifactRarityRepository;
        _artifactThemeMatchingTagCountRepo = artifactThemeMatchingTagCountRepo;
        _zoneService = zoneService;
        _displayArtifactService = displayArtifactService;
        _artifactScoreRepository = artifactScoreRepository;
        _museumTileService = museumTileService;
        _exhibitService = exhibitService;
        _rawArtifactFunctionalService = rawArtifactFunctionalService;
    }

    public async Task<float> GetArtifactScore(Artifact artifact, float zoneThemeMultiplier)
    {
        var conditionValue = await _artifactConditionRepository.GetConditionValueByCondition(artifact.Condition);
        var rarityValue = await _artifactRarityRepository.GetRarityValueByRarity(artifact.Rarity);
        var rawArtifacts = await _rawArtifactFunctionalService.GetAllRawArtifactFunctional();
        var eraValue = await _artifactEraRepository.GetEraValueByEra(rawArtifacts.FirstOrDefault(functional => functional.Id == artifact.RawArtifactId).Era);
        float artifactScore = 5 * conditionValue * zoneThemeMultiplier * rarityValue * eraValue;
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
    
    

    public async Task<float> GetThemeMultiplierOfZone(List<Artifact> artifacts)
    {
        Dictionary<string, int> tagCounter = new Dictionary<string, int>();
        
        void IncrementTagCount(string tag)
        {
            if (tagCounter.ContainsKey(tag))
            {
                tagCounter[tag]++;
            }
            else
            {
                tagCounter[tag] = 1;
            }
        }
        
        
        
        foreach (var artifact in artifacts)
        {
            var rawArtifacts = await _rawArtifactFunctionalService.GetAllRawArtifactFunctional();
            IncrementTagCount(rawArtifacts.FirstOrDefault(functional => functional.Id == artifact.RawArtifactId).Era);
            IncrementTagCount(rawArtifacts.FirstOrDefault(functional => functional.Id == artifact.RawArtifactId).Region);
            IncrementTagCount(rawArtifacts.FirstOrDefault(functional => functional.Id == artifact.RawArtifactId).Object);
            IncrementTagCount(rawArtifacts.FirstOrDefault(functional => functional.Id == artifact.RawArtifactId).ObjectClass);
            IncrementTagCount(rawArtifacts.FirstOrDefault(functional => functional.Id == artifact.RawArtifactId).ObjectSize);

            foreach (var material in rawArtifacts.FirstOrDefault(functional => functional.Id == artifact.RawArtifactId).Materials)
            {
                IncrementTagCount(material);
            }
        }
        
        
        
        // foreach (var artifact in artifacts)
        // {
        //     if (tagCounter.ContainsKey(artifact.Era))
        //     {
        //         tagCounter[artifact.Era]++;
        //     } else
        //     {
        //         tagCounter[artifact.Era] = 1;
        //     }
        //     
        //     if (tagCounter.ContainsKey(artifact.Region))
        //     {
        //         tagCounter[artifact.Region]++;
        //     } else
        //     {
        //         tagCounter[artifact.Region] = 1;
        //     }
        //     
        //     if (tagCounter.ContainsKey(artifact.Object))
        //     {
        //         tagCounter[artifact.Object]++;
        //     } else
        //     {
        //         tagCounter[artifact.Object] = 1;
        //     }
        //     
        //     if (tagCounter.ContainsKey(artifact.ObjectClass))
        //     {
        //         tagCounter[artifact.ObjectClass]++;
        //     } else
        //     {
        //         tagCounter[artifact.ObjectClass] = 1;
        //     }
        //     
        //     if (tagCounter.ContainsKey(artifact.ObjectSize))
        //     {
        //         tagCounter[artifact.ObjectSize]++;
        //     } else
        //     {
        //         tagCounter[artifact.ObjectSize] = 1;
        //     }
        //
        //     foreach (var material in artifact.Materials)
        //     {
        //         if (tagCounter.ContainsKey(material))
        //         {
        //             tagCounter[material]++;
        //         } else
        //         {
        //             tagCounter[material] = 1;
        //         }
        //     }
        //     
        //     
        // }
        
        int commonThemeCount = tagCounter.Count(kv => kv.Value == artifacts.Count);

        var themeMultiplier = await _artifactThemeMatchingTagCountRepo.GetArtifactThemeMatchingMultiplierByThemeCount(commonThemeCount);
        return themeMultiplier;
        
        
    }

    public async Task RefreshArtifactScore()
    {
        var artifactScores = await _artifactScoreRepository.GetAllArtifactScore();
        var exhibits = await _exhibitService.GetAllExhibits();
        foreach (var artifactScore in artifactScores)
        {
            artifactScore.IsInZone = false;
        }

        var zones = await _zoneService.GetAll();
        foreach (var zone in zones)
        {
            List<string> artifactIdsInZone = new List<string>();
            List<Artifact> artifactsInZone = new List<Artifact>();
            foreach (var tileId in zone.OccupiedMuseumTileIds)
            {
                var tile = await _museumTileService.GetMuseumTileById(tileId);
                if (tile.HasExhibit)
                {
                    
                    var exhibit = exhibits.FirstOrDefault(ex => ex.Id == tile.ExhibitId);

                    if (exhibit.ArtifactIds != null && exhibit.ArtifactIds.Count > 0)
                    {
                        foreach (var artifactId in exhibit.ArtifactIds)
                        {
                            if (!artifactIdsInZone.Contains(artifactId))
                            {
                                artifactIdsInZone.Add(artifactId);
                            }
                        }
                        
                    }
                }
            }
            
            foreach (var artifactId in artifactIdsInZone)
            {
                var artifact = await _displayArtifactService.GetArtifactById(artifactId);
                if (artifact != null) artifactsInZone.Add(artifact);
            }

            var zoneThemeMultiplier = await GetThemeMultiplierOfZone(artifactsInZone);
            
            foreach (var artifactId in artifactIdsInZone)
            {
                ArtifactScore? foundArtifactId = artifactScores.Find(artifact => artifact.ArtifactId == artifactId);

                Artifact? artifact = await _displayArtifactService.GetArtifactById(artifactId);

                float artifactScore = await GetArtifactScore(artifact, zoneThemeMultiplier);
                
                if (foundArtifactId != null)
                {
                    foundArtifactId.IsInZone = true;
                    foundArtifactId.Score = artifactScore;
                }
                else
                {
                    var newArtifactScore = new ArtifactScore
                    {
                        ArtifactId = artifactId,
                        Score = artifactScore,
                        IsInZone = true
                    };
                    artifactScores.Add(newArtifactScore);
                }
            }

            
        }
        
        

        var displayArtifacts = await _displayArtifactService.GetAllArtifacts();

        foreach (var artifact in displayArtifacts)
        {
            ArtifactScore? foundArtifact = artifactScores.Find(artifact1 => artifact1.ArtifactId == artifact.Id);
            
            float artifactScore = await GetArtifactScore(artifact, 0.8f);
                
            if (foundArtifact != null)
            {
                foundArtifact.IsInZone = false;
                foundArtifact.Score = artifactScore;
            }
            else
            {
                var newArtifactScore = new ArtifactScore
                {
                    ArtifactId = artifact.Id,
                    Score = artifactScore,
                    IsInZone = false
                };
                artifactScores.Add(newArtifactScore);
            }
        }
    }
    
    
}