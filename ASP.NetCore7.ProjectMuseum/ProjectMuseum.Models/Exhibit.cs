namespace ProjectMuseum.Models;

public class Exhibit
{
    public string Id { get; set; }
    public string XPosition { get; set; }
    public string YPosition { get; set; }
    public string ExhibitDecoration { get; set; }
    public string ExhibitArtifactSlot1 { get; set; }
    public string ExhibitArtifactSlot2 { get; set; }
    public string ExhibitArtifactSlot3 { get; set; }
    public string ExhibitArtifactSlot4 { get; set; }
    public string ExhibitArtifactSlot5 { get; set; }
    public bool IsHangingExhibit { get; set; }
    public bool IsWallExhibit { get; set; }
}