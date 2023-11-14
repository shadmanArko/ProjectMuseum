namespace ProjectMuseum.Models;

public class StoryScene
{
    public string Id { get; set; }
    public int SceneNo { get; set; }
    public List<StorySceneEntry> StorySceneEntries { get; set; }
}