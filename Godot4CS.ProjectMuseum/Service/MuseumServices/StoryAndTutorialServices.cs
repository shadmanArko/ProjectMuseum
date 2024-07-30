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
        var storyScenesJson = File.ReadAllText(
            "E:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Starting Data Folder/StoryScene.json");
        var storyScenes = JsonSerializer.Deserialize<List<StoryScene>>(storyScenesJson);
        var storyScene = storyScenes!.FirstOrDefault(story => story.SceneNo == sceneNo);
        return storyScene;
    }
    public Tutorial GetTutorialByNumber(int sceneNo)
    {
        var tutorialsJson = File.ReadAllText(
            "E:/Godot Projects/ProjectMuseum/ASP.NetCore7.ProjectMuseum/ProjectMuseum.APIs/Starting Data Folder/Tutorials.json");
        var tutorials = JsonSerializer.Deserialize<List<Tutorial>>(tutorialsJson);
        var tutorial = tutorials!.FirstOrDefault(story => story.SceneNo == sceneNo);
        return tutorial;
    }
}