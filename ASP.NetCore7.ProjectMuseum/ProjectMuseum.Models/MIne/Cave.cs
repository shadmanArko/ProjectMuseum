namespace ProjectMuseum.Models;

public class Cave
{
    public string Id { get; set; }
    public bool IsRevealed { get; set; }

    public int TopBound { get; set; }
    public int LeftBound { get; set; }
    public int RightBound { get; set; }
    public int BottomBound { get; set; }
    public List<string> CellIds { get; set; }
    public List<string> StalagmiteCellIds { get; set; }
    public List<string> StalactiteCellIds { get; set; }
}