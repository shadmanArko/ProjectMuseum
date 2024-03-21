namespace ProjectMuseum.Models;

public class ProceduralMineGenerationData
{
    public int MineSizeX { get; set; }
    public int MineSizeY { get; set; }
    public int CellSize { get; set; }

    public bool HasBossCave { get; set; }
    public int BossCaveSizeX { get; set; }
    public int BossCaveSizeY { get; set; }
    public int StalagmiteCount { get; set; }
    public int StalactiteCount { get; set; }

    public int NumberOfMaxCaves { get; set; }
    public int CaveMinSizeX { get; set; }
    public int CaveMinSizeY { get; set; }
    public int CaveMaxSizeX { get; set; }
    public int CaveMaxSizeY { get; set; }
    public int MinDistanceBetweenCaves { get; set; }
    public string Region { get; set; }
    public string Site { get; set; }
    public int TotalNoOfArtifacts { get; set; }
}