using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.StorySceneRepository;

public interface IStorySceneRepository
{
    Task<StoryScene?> GetByScene(int sceneNo);
}