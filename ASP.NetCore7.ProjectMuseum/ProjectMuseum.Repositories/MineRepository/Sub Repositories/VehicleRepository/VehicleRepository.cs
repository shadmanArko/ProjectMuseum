using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

public class VehicleRepository : IVehicleRepository
{
    private readonly JsonFileDatabase<Mine> _mineDatabase;
    private readonly JsonFileDatabase<Vehicle> _vehicleDatabase;
    
    public VehicleRepository(JsonFileDatabase<Mine> mineDatabase, JsonFileDatabase<Vehicle> vehicleDatabase)
    {
        _mineDatabase = mineDatabase;
        _vehicleDatabase = vehicleDatabase;
    }
    
    public async Task<Vehicle> AddVehicleToMine(string category, string subCategory)
    {
        var mine = await _mineDatabase.ReadDataAsync();
        var vehicles = await _vehicleDatabase.ReadDataAsync();
        var vehiclesWithSameCategory = vehicles?.FindAll(vehicle1 => vehicle1.Category == category);
        var vehicle = vehiclesWithSameCategory?.FirstOrDefault(vehicle2 => vehicle2.SubCategory == subCategory);
        vehicles?.Add(vehicle!);
        if (mine != null) await _mineDatabase.WriteDataAsync(mine);
        return vehicle!;
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