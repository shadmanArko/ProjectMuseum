using ProjectMuseum.Models;
using ProjectMuseum.Models.Vehicles;

namespace ProjectMuseum.Repositories.MineRepository.Sub_Repositories.VehicleRepository;

public class MineVehicleRepository : IMineVehicleRepository
{
    private readonly JsonFileDatabase<Mine> _mineDatabase;
    private readonly JsonFileDatabase<Item> _vehicleDatabase;
    
    public MineVehicleRepository(JsonFileDatabase<Mine> mineDatabase, JsonFileDatabase<Item> vehicleDatabase)
    {
        _mineDatabase = mineDatabase;
        _vehicleDatabase = vehicleDatabase;
    }
    
    public async Task<Item> AddVehicleToMine(string subCategory)
    {
        var mines = await _mineDatabase.ReadDataAsync();
        var mine = mines?[0];
        var vehicles = await _vehicleDatabase.ReadDataAsync();
        var vehicle = vehicles?.FirstOrDefault(vehicle2 => vehicle2.SubCategory == subCategory);
        vehicle!.Id = Guid.NewGuid().ToString();
        var newVehicle = new Vehicle();
        mine?.Vehicles.Add(newVehicle); //TODO: Initialize new vehicle from the category and variant
        if (mine != null) await _mineDatabase.WriteDataAsync(mines!);
        return vehicle;
    }

    public async Task<Item?> RemoveVehicleFromMine(string vehicleId)
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