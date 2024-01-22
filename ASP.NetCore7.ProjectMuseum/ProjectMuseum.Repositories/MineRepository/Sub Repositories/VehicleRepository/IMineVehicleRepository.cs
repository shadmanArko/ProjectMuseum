using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

public interface IMineVehicleRepository
{
    public Task<Vehicle> AddVehicleToMine(string subCategory);
    public Task<Vehicle?> RemoveVehicleFromMine(string vehicleId);
}