using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Services.MineService.Sub_Services.VehicleService;

public interface IMineVehicleService
{
    Task<Equipable> SendVehicleToInventory(string vehicleId);
}