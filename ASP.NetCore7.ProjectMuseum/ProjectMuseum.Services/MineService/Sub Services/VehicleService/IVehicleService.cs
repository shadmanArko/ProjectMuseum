using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Services.MineService.Sub_Services.VehicleService;

public interface IVehicleService
{
    Task<Equipable> SendVehicleToInventory(string vehicleId);
}