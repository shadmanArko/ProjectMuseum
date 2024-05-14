using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;
using ProjectMuseum.Models.CoreShop;

namespace ProjectMuseum.Models;

public class Shop
{
    public string Id { get; set; }
    public CoreShopFunctional CoreShopFunctional { get; set; }
    public CoreShopDescriptive CoreShopDescriptive { get; set; }
    public int XPosition { get; set; }
    public int YPosition { get; set; }
    public int RotationFrame { get; set; }
    public bool IsBroken { get; set; }
    public int TotalBreakdowns { get; set; }
    public int TotalSales { get; set; }
    public float TotalRevenueEarned { get; set; }
    
}