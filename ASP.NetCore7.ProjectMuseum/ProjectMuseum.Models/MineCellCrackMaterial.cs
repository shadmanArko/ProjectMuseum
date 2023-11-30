using System.Numerics;

namespace ProjectMuseum.Models;

public class MineCellCrackMaterial
{
    public List<CellCrackMaterial> CellCrackMaterials { get; set; }
}

public class CellCrackMaterial
{
    public string MaterialType { get; set; }
    public CrackImage SmallCrack { get; }
    public CrackImage MediumCrack { get; }
    public CrackImage LargeCrack { get; }
}

public class CrackImage
{
    public int TileSourceId { get; set; }
    public int AtlasCoordX { get; set; }
    public int AtlasCoordY { get; set; }
}