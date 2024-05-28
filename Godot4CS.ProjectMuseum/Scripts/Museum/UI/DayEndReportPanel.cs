using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Model;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class DayEndReportPanel : Panel
{
	[Export] private Label _numberOfTicketsSold;
	[Export] private Label _totalRevenueFromTickets;
	[Export] private Label _numberOfShopSales;
	[Export] private Label _souvenirSales;
	[Export] private Label _foodSales;
	[Export] private Label _drinksSales;
	[Export] private Label _totalRevenueFromShops;
	[Export] private Label _souvenirRevenue;
	[Export] private Label _foodRevenue;
	[Export] private Label _drinksRevenue;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.DayEndReportGenerated += DayEndReportGenerated;
	}

	private void DayEndReportGenerated(MuseumDayEndReport obj)
	{
		_numberOfTicketsSold.Text = obj.NumberOfTicketsSold.ToString("0")+"X";
		_totalRevenueFromTickets.Text = obj.TotalRevenueFromTickets.ToString("0");
		_numberOfShopSales.Text = obj.NumberOfShopSales.ToString("0")+"X";
		_souvenirSales.Text = obj.NumberOfSouvenirSales.ToString("0");
		_foodSales.Text = obj.NumberOfFoodSales.ToString("0");
		_drinksSales.Text = obj.NumberOfDrinkSales.ToString("0");
		_totalRevenueFromShops.Text = obj.TotalRevenueFromShops.ToString("0");
		_souvenirRevenue.Text = obj.TotalSouvenirRevenue.ToString("0");
		_foodRevenue.Text = obj.TotalFoodRevenue.ToString("0");
		_drinksRevenue.Text = obj.TotalDrinkRevenue.ToString("0");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
