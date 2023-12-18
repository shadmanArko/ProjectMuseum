namespace ProjectMuseum.Models;

public class Tutorial
{
    public string Id { get; set; }
    public int SceneNo { get; set; }
    public bool ContinuesStory { get; set; }
    public int StoryNumber { get; set; }
    public List<TutorialSceneEntry> TutorialSceneEntries { get; set; }
}