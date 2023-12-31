using System.Diagnostics;
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
    public async Task<PlayerInfo> UpdateCompletedTutorial(int completedTutorialNumber)
    {
        var info = await _playerInfoRepository.UpdateCompletedTutorial(completedTutorialNumber);
        return info;
    }
    public async Task<PlayerInfo> UpdateCompletedStory(int completedStoryNumber)
    {
        var info = await _playerInfoRepository.UpdateCompletedStory(completedStoryNumber);
        return info;
    }
    public async Task<PlayerInfo?> GetPlayerInfo()
    {
        var info = await _playerInfoRepository.GetLastPlayerInfo();
        return info;
    }
}