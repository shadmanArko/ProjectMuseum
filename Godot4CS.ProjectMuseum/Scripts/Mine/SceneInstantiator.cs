using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public class SceneInstantiator
{
	public static Node InstantiateScene(string scenePath, Node parentNode, Vector2 position)
	{
		var scene = ResourceLoader.Load<PackedScene>(scenePath).Instantiate() as Node2D;
		if (scene is null)
		{
			GD.PrintErr("COULD NOT GENERATE SCENE. FATAL ERROR");
			return null;
		}
		parentNode.AddChild(scene);
		scene!.Set("position", position);

		return scene;
	}
}