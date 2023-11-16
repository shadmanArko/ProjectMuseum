using ProjectMuseum.Models;

namespace ProjectMuseum.Services.StorySceneService;

public interface IStorySceneService
{
    Task<StoryScene?> GetStorySceneBySceneNo(int sceneNo);
}