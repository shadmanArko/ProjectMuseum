namespace ProjectMuseum.Models;

public class TileWallInfo
{
    public string TileId { get; set; }
    public string BackLeftWallId{ get; set; }
    public string BackRightWallId{ get; set; }
    public string FrontLeftWallId{ get; set; }
    public string FrontRightWallId{ get; set; }

    public TileWallInfo(string tileId, string backLeftWallId, string backRightWallId, string frontLeftWallId, string frontRightWallId)
    {
        this.TileId = tileId;
        this.BackLeftWallId = backLeftWallId;
        this.BackRightWallId = backRightWallId;
        this.FrontLeftWallId = frontLeftWallId;
        this.FrontRightWallId = frontRightWallId;
    }
}