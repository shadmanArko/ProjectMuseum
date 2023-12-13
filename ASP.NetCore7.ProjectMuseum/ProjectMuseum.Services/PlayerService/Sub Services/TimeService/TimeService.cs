using ProjectMuseum.Models;
using ProjectMuseum.Repositories.PlayerRepository.Sub_Repositories.TimeRepository;

namespace ProjectMuseum.Services.PlayerService.Sub_Services.TimeService;

public class TimeService : ITimeService
{
    private readonly ITimeRepository _timeRepository;

    public TimeService(ITimeRepository timeRepository)
    {
        _timeRepository = timeRepository;
    }

    public async Task<Time?> GetTime()
    {
        return await _timeRepository.Get();
    }

    public async Task<Time?> SaveTime(Time time)
    {
        return await _timeRepository.Update(time);
    }
}