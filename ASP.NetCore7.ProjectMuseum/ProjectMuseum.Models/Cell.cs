

namespace ProjectMuseum.Models;

public class Cell
{
	public string Id { get; set; }
	public int PositionX{ get; set; }
	public int PositionY{ get; set; }
	public bool IsBreakable{ get; set; }
	public bool IsBroken { get; set; }
	public bool IsRevealed { get; set; }
	public bool HasArtifact{ get; set; }
	public bool IsInstantiated{ get; set; }
	public int HitPoint{ get; set; }
	public bool TopBrokenSide { get; set; }
	public bool RightBrokenSide { get; set; }
	public bool BottomBrokenSide { get; set; }
	public bool LeftBrokenSide { get; set; }
	
}
