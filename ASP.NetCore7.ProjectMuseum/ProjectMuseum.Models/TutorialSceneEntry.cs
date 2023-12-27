namespace ProjectMuseum.Models;

public class TutorialSceneEntry
{
    public string EntryNo { get; set; }
    public string TutorialText { get; set; }
    public List<string> KeyBindsNeedsToPerform { get; set; }
    public List<string> ActionsNeedsToPerform { get; set; }
}