using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

public interface IVehicleRepository
{
    public Task<Vehicle> AddVehicleToMine(string category, string subCategory);
    public Task<Vehicle?> RemoveVehicleFromMine(string vehicleId);
}