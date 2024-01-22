using ProjectMuseum.Models;
using ProjectMuseum.Repositories;
using ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

namespace ProjectMuseum.Services.MineService.Sub_Services.VehicleService;

public class MineVehicleService : IMineVehicleService
{
    private readonly IMineVehicleRepository _mineVehicleRepository;
    private readonly IInventoryRepository _inventoryRepository;


    public MineVehicleService(IMineVehicleRepository mineVehicleRepository, IInventoryRepository inventoryRepository)
    {
        _mineVehicleRepository = mineVehicleRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Equipable> SendVehicleToInventory(string vehicleId)
    {
        var vehicle = await _mineVehicleRepository.RemoveVehicleFromMine(vehicleId);
        var equipable = await _inventoryRepository.AddEquipable("Vehicle", vehicle!.SubCategory, vehicle.SmallPngPath);
        return equipable!;
    }
}