namespace ProjectMuseum.Services.PlayerInfoService;
using ProjectMuseum.Models;
public interface IPlayerInfoService
{
    Task<PlayerInfo> InsertPlayerInfo(PlayerInfo playerInfo);
}