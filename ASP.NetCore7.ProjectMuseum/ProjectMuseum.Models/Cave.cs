namespace ProjectMuseum.Models;

public class Cave
{
    public string Id { get; set; }
    public List<string> CellIds { get; set; }
    public List<string> StalagmiteCellIds { get; set; }
    public List<string> StalactiteCellIds { get; set; }
}