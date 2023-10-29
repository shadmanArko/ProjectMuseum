namespace ProjectMuseum.Services.MuseumService;

public interface IMuseumService
{
    Task<int> GetMuseumCurrentMoneyAmount(string id);
}