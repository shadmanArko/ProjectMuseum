using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.PlayerInfoRepository;

public class PlayerInfoRepository: IPlayerInfoRepository
{
    private readonly JsonFileDatabase<PlayerInfo> _playerInfoDatabase;

    public PlayerInfoRepository(JsonFileDatabase<PlayerInfo> playerInfoDatabase)
    {
        _playerInfoDatabase = playerInfoDatabase;
    }
    
    public async Task<PlayerInfo> Insert(PlayerInfo playerInfo)
    {
        var playerInfos = await _playerInfoDatabase.ReadDataAsync();
        playerInfos?.Add(playerInfo);
        if (playerInfos != null) await _playerInfoDatabase.WriteDataAsync(playerInfos);
        return playerInfo;
    }

    public Task<PlayerInfo> Update(string id, PlayerInfo playerInfo)
    {
        throw new NotImplementedException();
    }

    public Task<PlayerInfo?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<PlayerInfo?> GetLastPlayerInfo()
    {
        var playerInfos = await _playerInfoDatabase.ReadDataAsync();
        return playerInfos?.Last();
    }

    public async Task<PlayerInfo> UpdateCompletedStory(int completedStoryNumber)
    {
        var playerInfos = await _playerInfoDatabase.ReadDataAsync();
        var currentPlayerInfo = playerInfos?.Last();
        currentPlayerInfo!.CompletedStoryScene = completedStoryNumber;
        if (playerInfos != null) await _playerInfoDatabase.WriteDataAsync(playerInfos);
        return  currentPlayerInfo;
    }

    public async Task<PlayerInfo> UpdateCompletedTutorial(int completedTutorialNumber)
    {
        var playerInfos = await _playerInfoDatabase.ReadDataAsync();
        var currentPlayerInfo = playerInfos?.Last();
        currentPlayerInfo!.CompletedTutorialScene = completedTutorialNumber;
        if (playerInfos != null) await _playerInfoDatabase.WriteDataAsync(playerInfos);
        return  currentPlayerInfo;
    }

    public Task<List<PlayerInfo>?> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<PlayerInfo?> Delete(string id)
    {
        throw new NotImplementedException();
    }
}