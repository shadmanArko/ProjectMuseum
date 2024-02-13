using ProjectMuseum.Models.MIne;

namespace ProjectMuseum.Models;

public class Mine
{
    public List<Cell> Cells{ get; set; }
    public List<Resource> Resources { get; set; }
    public List<SpecialBackdropPngInformation> SpecialBackdropPngInformations { get; set; }
    public List<WallPlaceable> WallPlaceables { get; set; }
    public List<CellPlaceable> CellPlaceables { get; set; }
    public int CellSize{ get; set; }
    public int GridWidth{ get; set; }
    public int GridLength{ get; set; }
    
    
}