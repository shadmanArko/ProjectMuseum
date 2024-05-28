using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Model;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.Museum.ShopSystem;

public partial class DayEndReportManager : Node
{
	[Export] private ShopManager _shopManager;

	[Export] private TicketCounter _ticketCounter;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.DayEnded += DayEnded;
	}

	private void DayEnded()
	{
		var dayEndReport = new MuseumDayEndReport();
		dayEndReport.NumberOfTicketsSold = _ticketCounter.numberOfTicketsSold;
		dayEndReport.TotalRevenueFromTickets = _ticketCounter.totalRevenueFromTickets;
		dayEndReport.TotalDrinkRevenue = _shopManager.revenueFromDrinkSell;
		dayEndReport.TotalFoodRevenue = _shopManager.revenueFromFoodSell;
		dayEndReport.TotalSouvenirRevenue = _shopManager.revenueFromSouvenirSell;
		dayEndReport.NumberOfDrinkSales = _shopManager.numberOfDrinkSell;
		dayEndReport.NumberOfFoodSales = _shopManager.numberOfFoodSell;
		dayEndReport.NumberOfSouvenirSales = _shopManager.numberOfSouvenirSell;
		MuseumActions.DayEndReportGenerated?.Invoke(dayEndReport);
		GD.Print("day ended with " +dayEndReport);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	//todo Get all relevent data 
	//todo Calculate all data , store data
}
