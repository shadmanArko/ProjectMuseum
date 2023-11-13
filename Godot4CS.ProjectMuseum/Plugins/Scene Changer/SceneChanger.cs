using System.Threading.Tasks;
using Godot;

public partial class SceneChanger : Node
{
	[Export] private Button _sceneChangerButton; // Reference to your button in the scene
	[Export] private string _scenePath;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// ResourceLoader.LoadThreadedRequest(_scenePath); // todo
		// Assuming your button is named "YourButton" in the scene
		//_sceneChangerButton = GetNode<Button>("YourButton");

		// Connect the button's pressed signal to the method that will handle the scene change asynchronously.
		_sceneChangerButton.Connect(BaseButton.SignalName.ButtonDown, Callable.From(OnChangeSceneRequestedAsync));
	}

	// Method to handle the asynchronous scene change.
	private async void OnChangeSceneRequestedAsync()
	{
		// You can perform any logic here before the scene change.

		// Wait for the asynchronous operation to complete (e.g., an animation, network request, etc.).
		await PerformAsyncOperation();

		// After the asynchronous operation, load and change the scene.
		await ChangeSceneAsync(_scenePath);
	}

	// Simulate an asynchronous operation (replace this with your actual asynchronous logic).
	private async Task PerformAsyncOperation()
	{
		// Simulate an asynchronous operation (replace this with your actual asynchronous logic).
		await Task.Delay(1000); // 1-second delay as an example
	}

	// Method to handle the scene change asynchronously.
	private async Task ChangeSceneAsync(string scenePath)
	{
		// Load the new scene asynchronously.
		//PackedScene newScene = GD.Load<PackedScene>(scenePath);
		//ResourceLoader.LoadThreadedRequest(scenePath); //todo
		// Instance the new scene.
		//Node newSceneInstance = newScene.Instantiate();
		
		// Replace the current scene with the new scene.
		var newScene = GetTree().ChangeSceneToFile(scenePath);
	}
}
