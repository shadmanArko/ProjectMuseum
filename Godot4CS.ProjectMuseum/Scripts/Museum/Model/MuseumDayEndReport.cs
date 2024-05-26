namespace Godot4CS.ProjectMuseum.Scripts.Museum.Model;

public class MuseumDayEndReport
{
	private int _numberOfShopSales;
	private int _totalRevenueFromShops;
	public int TotalRevenueFromTickets { get; set; }
	
	public int NumberOfShopSales
	{
		get => NumberOfDrinkSales + NumberOfFoodSales + NumberOfSouvenirSales;
		set => _numberOfShopSales = value;
	}

	public int NumberOfSouvenirSales { get; set; }
	public int NumberOfFoodSales { get; set; }
	public int NumberOfDrinkSales { get; set; }
    
	public int TotalRevenueFromShops
	{
		get => TotalSouvenirRevenue + TotalFoodRevenue + TotalDrinkRevenue;
		set => _totalRevenueFromShops = value;

	}
    
	public int TotalSouvenirRevenue { get; set; }
	public int TotalFoodRevenue { get; set; }
	public int TotalDrinkRevenue { get; set; }
}