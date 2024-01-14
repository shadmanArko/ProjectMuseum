using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class ToolbarSlot : Node
{
	private MineSceneTooltip _mineSceneTooltip;
	
	[Export] private Sprite2D _highlighter;
	[Export] private Sprite2D _itemSlot;

	[Export] private Texture2D _selectedHighlighter;
	[Export] private Texture2D _defaultHighlighter;
	[Export] private Texture2D _itemSprite;

	public string ItemId;
	public bool IsArtifact;

	public override void _Ready()
	{
		_mineSceneTooltip = ReferenceStorage.Instance.Tooltip;
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
		_itemSlot.Texture = GD.Load<Texture2D>(pngPath);
	}
	
	private void RemoveItemTexture()
	{
		_itemSlot.Texture = null;
	}

	public void SetItemData(string itemId, bool isArtifact)
	{
		ItemId = itemId;
		IsArtifact = isArtifact;
	}
	
	private void RemoveItemData()
	{
		ItemId = "";
		IsArtifact = false;
	}

	public void RemoveItem()
	{
		SetItemAsDeselected();
		RemoveItemTexture();
		RemoveItemData();
	}

	private void OnMouseEnter()
	{
		_mineSceneTooltip.ShowToolbarTooltip(ItemId, IsArtifact);
	}

	private void OnMouseExit()
	{
		_mineSceneTooltip.HideTooltip();
	}
}