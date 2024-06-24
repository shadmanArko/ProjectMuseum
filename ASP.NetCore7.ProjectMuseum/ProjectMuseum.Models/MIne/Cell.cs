

namespace ProjectMuseum.Models;

public class Cell
{
	public string? Id { get; set; }
	public int PositionX{ get; set; }
	public int PositionY{ get; set; }

	public int MaxHitPoint { get; set; }
	public int HitPoint{ get; set; }
	
	public bool IsBreakable{ get; set; }
	public bool IsBroken { get; set; }
	public bool IsRevealed { get; set; }
	public bool IsInstantiated{ get; set; }
	
	public bool TopBrokenSide { get; set; }
	public bool RightBrokenSide { get; set; }
	public bool BottomBrokenSide { get; set; }
	public bool LeftBrokenSide { get; set; }
	
	public bool HasWallPlaceable { get; set; }
	public bool HasCellPlaceable { get; set; }
	public bool HasSpecialWall { get; set; }
	public bool HasArtifact{ get; set; }
	public bool HasResource { get; set; }
	public bool HasTransportBlock { get; set; }
	public bool HasTransportVehicle { get; set; }
	public bool HasCave { get; set; }
	
	public string ArtifactId { get; set; }
	public string ArtifactMaterial { get; set; }
}
