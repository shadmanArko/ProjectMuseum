using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class ToolbarSlot : Node
{
	[Export] private Sprite2D _highlighter;
	[Export] private Sprite2D _itemSlot;

	[Export] private Texture2D _selectedHighlighter;
	[Export] private Texture2D _defaultHighlighter;
	[Export] private Texture2D _itemSprite;

	public override void _Ready()
	{
		_itemSlot.Texture = _itemSprite;
	}

	public void SetItemAsSelected()
	{
		_highlighter.Texture = _selectedHighlighter;
	}
	
	public void SetItemAsDeselected()
	{
		_highlighter.Texture = _defaultHighlighter;
	}

	public void SetItemTexture(string pngPath)
	{
		_itemSprite = GD.Load<Texture2D>(pngPath);
	}
}