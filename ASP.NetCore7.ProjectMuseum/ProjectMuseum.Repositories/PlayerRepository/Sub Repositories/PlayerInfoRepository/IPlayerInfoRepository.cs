using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.PlayerInfoRepository;

public interface IPlayerInfoRepository
{
    Task<PlayerInfo> Insert(PlayerInfo playerInfo);
    Task<PlayerInfo> Update(string id, PlayerInfo playerInfo);
    Task<PlayerInfo?> GetById(string id);
    Task<PlayerInfo?> GetLastPlayerInfo();
    Task<PlayerInfo> UpdateCompletedStory(int completedStoryNumber);
    Task<PlayerInfo> UpdateCompletedTutorial(int completedTutorialNumber);
    Task<List<PlayerInfo>?> GetAll();
    Task<PlayerInfo?> Delete(string id);
}