namespace Godot4CS.ProjectMuseum.Scripts.Museum.Model;

public class MuseumDayEndReport
{
	private int _numberOfShopSales;
	private float _totalRevenueFromShops;
	public float NumberOfTicketsSold { get; set; }
	public float TotalRevenueFromTickets { get; set; }
	
	public int NumberOfShopSales
	{
		get => NumberOfDrinkSales + NumberOfFoodSales + NumberOfSouvenirSales;
		set => _numberOfShopSales = value;
	}

	public int NumberOfSouvenirSales { get; set; }
	public int NumberOfFoodSales { get; set; }
	public int NumberOfDrinkSales { get; set; }
    
	public float TotalRevenueFromShops
	{
		get => TotalSouvenirRevenue + TotalFoodRevenue + TotalDrinkRevenue;
		set => _totalRevenueFromShops = value;

	}
    
	public float TotalSouvenirRevenue { get; set; }
	public float TotalFoodRevenue { get; set; }
	public float TotalDrinkRevenue { get; set; }
}