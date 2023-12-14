namespace ProjectMuseum.Models;

public class Tutorial
{
    public string Id { get; set; }
    public int SceneNo { get; set; }
    public List<TutorialSceneEntry> TutorialSceneEntries { get; set; }
}