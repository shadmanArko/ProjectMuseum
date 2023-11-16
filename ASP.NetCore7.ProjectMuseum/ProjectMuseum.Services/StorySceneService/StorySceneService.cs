using ProjectMuseum.Models;
using ProjectMuseum.Repositories.StorySceneRepository;

namespace ProjectMuseum.Services.StorySceneService;

public class StorySceneService: IStorySceneService
{
    private readonly IStorySceneRepository _storySceneRepository;

    public StorySceneService(IStorySceneRepository storySceneRepository)
    {
        _storySceneRepository = storySceneRepository;
    }
    public async Task<StoryScene?> GetStorySceneBySceneNo(int sceneNo)
    {
        var storyScene = await _storySceneRepository.GetByScene(sceneNo);
        return storyScene;
    }
}