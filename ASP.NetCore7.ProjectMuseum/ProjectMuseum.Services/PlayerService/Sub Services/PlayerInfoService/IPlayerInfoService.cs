namespace ProjectMuseum.Services.PlayerInfoService;
using ProjectMuseum.Models;
public interface IPlayerInfoService
{
    Task<PlayerInfo> InsertPlayerInfo(PlayerInfo playerInfo);
    Task<PlayerInfo> UpdateCompletedTutorial(int completedTutorialNumber);
    Task<PlayerInfo> UpdateCompletedStory(int completedStoryNumber);
    Task<PlayerInfo?> GetPlayerInfo();
}