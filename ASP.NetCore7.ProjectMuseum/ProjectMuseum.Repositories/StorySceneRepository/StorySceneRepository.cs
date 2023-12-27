using ProjectMuseum.Models;

namespace ProjectMuseum.Repositories.StorySceneRepository;

public class StorySceneRepository : IStorySceneRepository
{
    private readonly JsonFileDatabase<StoryScene> _storySceneDatabase;
    private readonly JsonFileDatabase<Tutorial> _tutorialDatabase;

    public StorySceneRepository(JsonFileDatabase<StoryScene> storySceneDatabase, JsonFileDatabase<Tutorial> tutorialDatabase)
    {
        _storySceneDatabase = storySceneDatabase;
        _tutorialDatabase = tutorialDatabase;
    }
    public async Task<StoryScene?> GetByScene(int sceneNo)
    {
        var storyScenes = await _storySceneDatabase.ReadDataAsync();
        var storyScene = storyScenes!.FirstOrDefault(story => story.SceneNo == sceneNo);
        return storyScene;
    }
    public async Task<Tutorial?> GetTutorialByNumber(int sceneNo)
    {
        var tutorials = await _tutorialDatabase.ReadDataAsync();
        var tutorial = tutorials!.FirstOrDefault(story => story.SceneNo == sceneNo);
        return tutorial;
    }
}