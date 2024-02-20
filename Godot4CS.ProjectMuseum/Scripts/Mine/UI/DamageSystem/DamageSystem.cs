using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI.DamageSystem;

public partial class DamageSystem : Node2D
{
	[Export] private string _damageVisualizerPath;
	public void ShowDamageValue(int value, Vector2 pos)
	{
		var damageVisualizer = ResourceLoader.Load<PackedScene>(_damageVisualizerPath).Instantiate() as DamageVisualizer;
		if (damageVisualizer == null)
		{
			GD.PrintErr("Could not instantiate damage visualizer");
			return;
		}
		ReferenceStorage.Instance.MineGenerationVariables.MineGenView.CallDeferred("add_child", damageVisualizer);
		damageVisualizer.Position = pos;
		damageVisualizer.SetDamageValue(value);
	}
}