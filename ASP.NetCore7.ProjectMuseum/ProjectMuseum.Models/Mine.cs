using ProjectMuseum.Models.TransportChainBlocks;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Models;

public class Mine
{
    public List<Cell> Cells{ get; set; }
    public List<Item> Items { get; set; }
    public List<Vehicle> Vehicles { get; set; }
    public List<TransportChainBlock> TransportChainBlocks { get; set; }
    public int CellSize{ get; set; }
    public int GridWidth{ get; set; }
    public int GridLength{ get; set; }
    
    
}