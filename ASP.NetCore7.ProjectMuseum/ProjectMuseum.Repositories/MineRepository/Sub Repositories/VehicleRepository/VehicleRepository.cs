using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

public class VehicleRepository : IVehicleRepository
{
    private readonly JsonFileDatabase<Mine> _mineDatabase;
    
    public VehicleRepository(JsonFileDatabase<Mine> mineDatabase)
    {
        _mineDatabase = mineDatabase;
    }
    
    public async Task<Vehicle> AddVehicleToMine(Vehicle vehicle)
    {
        var mine = await _mineDatabase.ReadDataAsync();
        //TODO: Get stats of vehicle from database and create new vehicle and add to vehicles list
        var vehicles = mine?[0].Vehicles;
        vehicles?.Add(vehicle);
        if (mine != null) await _mineDatabase.WriteDataAsync(mine);
        return vehicle;
    }

    public async Task<Vehicle?> RemoveVehicleFromMine(string vehicleId)
    {
        var mine = await _mineDatabase.ReadDataAsync();
        var vehicles = mine?[0].Vehicles;
        var vehicleToRemove = vehicles?.FirstOrDefault(tempVehicle => tempVehicle.Id == vehicleId);
        if (vehicleToRemove != null)
        {
            vehicles?.Remove(vehicleToRemove);
        }

        if (mine != null)
        {
            await _mineDatabase.WriteDataAsync(mine);
        }
        return vehicleToRemove;
    }
}