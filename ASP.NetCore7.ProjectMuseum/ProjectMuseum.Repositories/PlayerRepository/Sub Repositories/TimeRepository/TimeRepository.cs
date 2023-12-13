using System.IO.Pipes;
using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.PlayerRepository.Sub_Repositories.TimeRepository;

public class TimeRepository : ITimeRepository
{
    private readonly JsonFileDatabase<Time> _timeDatabase;

    public TimeRepository(JsonFileDatabase<Time> timeDatabase)
    {
        _timeDatabase = timeDatabase;
    }

    public async Task<Time?> Get()
    {
        List<Time>? times = await _timeDatabase.ReadDataAsync();
        return times?[0];
    }

    public async Task<Time?> Update(Time time)
    {
        List<Time>? times = await _timeDatabase.ReadDataAsync();
        if (times != null) times[0] = time;
        if (times != null) await _timeDatabase.WriteDataAsync(times);
        return times?[0];
    }
}