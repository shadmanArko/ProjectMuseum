using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

public interface IVehicleRepository
{
    public Task<Vehicle> AddVehicleToMine(Vehicle vehicle);
    public Task<Vehicle?> RemoveVehicleFromMine(string vehicleId);
}