using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class ToolbarSlot : Node
{
	[Export] private Sprite2D _highlighter;
	[Export] private Sprite2D _itemSlot;

	[Export] private Texture2D _selectedHighlighter;
	[Export] private Texture2D _defaultHighlighter;

	public void SetItemAsSelected()
	{
		_highlighter.Texture = _selectedHighlighter;
	}
	
	public void SetItemAsDeselected()
	{
		_highlighter.Texture = _defaultHighlighter;
	}
}