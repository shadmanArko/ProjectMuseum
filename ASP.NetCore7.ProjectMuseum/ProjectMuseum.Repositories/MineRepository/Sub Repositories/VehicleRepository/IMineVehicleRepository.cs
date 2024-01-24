using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

public interface IMineVehicleRepository
{
    public Task<Item> AddVehicleToMine(string subCategory);
    public Task<Item?> RemoveVehicleFromMine(string vehicleId);
}