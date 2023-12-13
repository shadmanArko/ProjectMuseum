using ProjectMuseum.Models;

namespace ProjectMuseum.Services.PlayerService.Sub_Services.TimeService;

public interface ITimeService
{
    Task<Time?> GetTime();
    Task<Time?> SaveTime(Time time);
}