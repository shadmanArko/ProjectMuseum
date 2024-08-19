using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using ProjectMuseum.Models;
using ProjectMuseum.Models.CoreShop;

namespace Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class StoryAndTutorialServices: Node
{
    public StoryScene GetByScene(int sceneNo)
    {
        var storyScenesJson = Godot.FileAccess.Open("res://Game Data/StoryData/StoryScene.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
        var storyScenes = JsonSerializer.Deserialize<List<StoryScene>>(storyScenesJson);
        var storyScene = storyScenes!.FirstOrDefault(story => story.SceneNo == sceneNo);
        return storyScene;
    }
    public Tutorial GetTutorialByNumber(int sceneNo)
    {
        var tutorialsJson = Godot.FileAccess.Open("res://Game Data/TutorialData/Tutorials.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
        var tutorials = JsonSerializer.Deserialize<List<Tutorial>>(tutorialsJson);
        var tutorial = tutorials!.FirstOrDefault(story => story.SceneNo == sceneNo);
        return tutorial;
    }
}