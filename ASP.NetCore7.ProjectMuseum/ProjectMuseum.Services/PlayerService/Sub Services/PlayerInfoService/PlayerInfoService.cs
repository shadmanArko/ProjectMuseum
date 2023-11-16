using ProjectMuseum.Models;
using ProjectMuseum.Repositories.PlayerInfoRepository;

namespace ProjectMuseum.Services.PlayerInfoService;

public class PlayerInfoService: IPlayerInfoService
{
    private readonly IPlayerInfoRepository _playerInfoRepository;

    public PlayerInfoService(IPlayerInfoRepository playerInfoRepository)
    {
        _playerInfoRepository = playerInfoRepository;
    }

    public async Task<PlayerInfo> InsertPlayerInfo(PlayerInfo playerInfo)
    {
        var newPlayerInfo = playerInfo;
        newPlayerInfo.Id = Guid.NewGuid().ToString();
        await _playerInfoRepository.Insert(playerInfo);
        return newPlayerInfo;
    }
}