namespace ProjectMuseum.Models.TransportChainBlocks;

public class TransportChainBlockStat : TransportChainBlock
{
    public bool ContainsVehicle { get; set; }
    
    public int PositionX { get; set; }
    public int PositionY { get; set; }
}