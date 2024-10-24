using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class ToolbarSlot : Control
{
	private MineSceneTooltip _mineSceneTooltip;
	
	[Export] private Sprite2D _highlighter;
	[Export] private Sprite2D _itemSlot;
	
	[Export] private Control _itemCounter;
	[Export] private Label _itemCounterValue;

	[Export] private Texture2D _selectedHighlighter;
	[Export] private Texture2D _defaultHighlighter;

	private string _itemId;
	private bool _isArtifact;

	public override void _Ready()
	{
		_mineSceneTooltip = ReferenceStorage.Instance.Tooltip;
	}

	#region Select And Deselect Item

	public void SetItemAsSelected()
	{
		_highlighter.Texture = _selectedHighlighter;
	}
	
	public void SetItemAsDeselected()
	{
		_highlighter.Texture = _defaultHighlighter;
	}

	#endregion

	#region Texture

	public void SetItemTexture(string pngPath)
	{
		_itemSlot.Texture = GD.Load<Texture2D>(pngPath);
	}
	
	private void RemoveItemTexture()
	{
		_itemSlot.Texture = null;
	}

	#endregion

	#region Item Data

	public void SetItemData(string itemId, bool isArtifact)
	{
		_itemId = itemId;
		_isArtifact = isArtifact;
	}
	
	private void RemoveItemData()
	{
		_itemId = "";
		_isArtifact = false;
	}

	#endregion

	#region Item Counter

	public void TurnOnItemCounter(int countValue)
	{
		_itemCounter.Visible = true;
		_itemCounterValue.Text = countValue.ToString();
	}

	public void TurnOffItemCounter()
	{
		_itemCounter.Visible = false;
	}

	#endregion

	public void RemoveItem()
	{
		SetItemAsDeselected();
		RemoveItemTexture();
		RemoveItemData();
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		GD.Print("drag started");
		return base._GetDragData(atPosition);
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		GD.Print("hovered over this node");
		return base._CanDropData(atPosition, data);
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		base._DropData(atPosition, data);
	}

	#region Mouse Enter And Exit

	private void OnMouseEnter()
	{
		_mineSceneTooltip.ShowToolbarTooltip(_itemId, _isArtifact);
	}

	private void OnMouseExit()
	{
		_mineSceneTooltip.HideTooltip();
	}

	#endregion
}