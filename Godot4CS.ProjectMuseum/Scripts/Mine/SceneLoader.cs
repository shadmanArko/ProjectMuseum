using System.Threading.Tasks;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class SceneLoader : Node
{
	[Export] private string _museumScenePath;
	public async void LoadMuseumScene()
	{
		await LoadScene(_museumScenePath);
	}
	
	private async Task LoadScene(string scenePath)
	{
		await PerformAsyncOperation();
		await ChangeSceneAsync(scenePath);
	}
	
	private async Task PerformAsyncOperation()
	{
		await Task.Delay(1000);
	}
	
	private async Task ChangeSceneAsync(string scenePath)
	{
		// Replace the current scene with the new scene.
		var newScene = GetTree().ChangeSceneToFile(scenePath);
	}
}