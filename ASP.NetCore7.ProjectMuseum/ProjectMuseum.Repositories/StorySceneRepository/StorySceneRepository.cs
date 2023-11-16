using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.StorySceneRepository;

public class StorySceneRepository : IStorySceneRepository
{
    private readonly JsonFileDatabase<StoryScene> _storySceneDatabase;

    public StorySceneRepository(JsonFileDatabase<StoryScene> storySceneDatabase)
    {
        _storySceneDatabase = storySceneDatabase;
    }
    public async Task<StoryScene?> GetByScene(int sceneNo)
    {
        var storyScenes = await _storySceneDatabase.ReadDataAsync();
        var storyScene = storyScenes!.FirstOrDefault(story => story.SceneNo == sceneNo);
        return storyScene;
    }
}