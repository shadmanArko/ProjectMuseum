namespace ProjectMuseum.Models;

public class StorySceneEntry
{
    public string EntryNo { get; set; }
    public string IllustrationName { get; set; }
    public string Dialogue { get; set; }
    public string Speaker { get; set; }
    public string SpeakerEmotion { get; set; }
    public bool HasCutscene { get; set; }
    public bool HasCutsceneArt { get; set; }
}