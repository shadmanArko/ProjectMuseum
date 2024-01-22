using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

public class MineVehicleRepository : IMineVehicleRepository
{
    private readonly JsonFileDatabase<Mine> _mineDatabase;
    private readonly JsonFileDatabase<Vehicle> _vehicleDatabase;
    
    public MineVehicleRepository(JsonFileDatabase<Mine> mineDatabase, JsonFileDatabase<Vehicle> vehicleDatabase)
    {
        _mineDatabase = mineDatabase;
        _vehicleDatabase = vehicleDatabase;
    }
    
    public async Task<Vehicle> AddVehicleToMine(string subCategory)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        var vehicles = await _vehicleDatabase.ReadDataAsync();
        var vehicle = vehicles?.FirstOrDefault(vehicle2 => vehicle2.SubCategory == subCategory);
        vehicle!.Id = Guid.NewGuid().ToString();
        mine?.Vehicles.Add(vehicle);
        if (mine != null) await _mineDatabase.WriteDataAsync(mines!);
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