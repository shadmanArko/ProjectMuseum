namespace ProjectMuseum.Models;

public class StoryScene
{
    public string Id { get; set; }
    public int SceneNo { get; set; }
    public bool HasTutorial { get; set; }
    public int TutorialNumber { get; set; }
    public List<StorySceneEntry> StorySceneEntries { get; set; }
}