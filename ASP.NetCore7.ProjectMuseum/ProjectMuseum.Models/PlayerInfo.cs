namespace ProjectMuseum.Models;

public class PlayerInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public bool Tutorial { get; set; }
    public int CompletedStoryScene { get; set; }
    public int CompletedTutorialScene { get; set; }
}