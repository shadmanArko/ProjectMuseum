using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.VehicleService;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IInventoryRepository _inventoryRepository;


    public VehicleService(IVehicleRepository vehicleRepository, IInventoryRepository inventoryRepository)
    {
        _vehicleRepository = vehicleRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Equipable> SendVehicleToInventory(string vehicleId)
    {
        var vehicle = await _vehicleRepository.RemoveVehicleFromMine(vehicleId);
        var equipable = await _inventoryRepository.AddEquipable("Vehicle", vehicle!.SubCategory, vehicle.SmallPngPath);
        return equipable!;
    }
}