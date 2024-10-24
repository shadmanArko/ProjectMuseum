using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.UI;

public partial class ButtonsTransition : HBoxContainer
{
	[Export] private Array<Control> _buttonHolders;
	[Export] private Control _mainButtonsBg;
	
	//[Export] private HBoxContainer _mainButtonContainer;
	[Export] private HBoxContainer _subButtonContainer;
	
	[Export] private HBoxContainer _adminContainer;
	[Export] private HBoxContainer _exhibitsContainer;
	[Export] private HBoxContainer _facilitiesContainer;
	[Export] private HBoxContainer _roomContainer;
	[Export] private HBoxContainer _staffContainer;

	[Export] private bool _toggleMode;
	
	private Tween _tween;

	private Color _invisibleColor = new(0, 0, 0, 0); 
	public override void _Ready()
	{
		SubscribeToActions();
		//_buttonNodes = new Array<Node>();
	}

	private void SubscribeToActions()
	{
		MuseumActions.OnBottomPanelButtonClicked += OnClickButton;
	}

	private void OnClickButton(string str)
	{
		//_buttonNodes = GetChildren();

		if (_toggleMode)
		{
			TurnOffMainContainerButtons(str);
			TurnOnSubButtons(str);
		}
		else
		{
			TurnOffSubButtons();
			TurnOnMainContainerButtons();
		}
		_toggleMode = !_toggleMode;

		// foreach (var button1 in _buttons)
		// {
		// 	var tempButton = button1 as Button;
		// 	if(tempButton == button) continue;
		// 	_tween = CreateTween();
		// 	GD.Print("making invisible");
		// 	_tween.TweenProperty(tempButton, "self_modulate", _invisibleColor, 0.05f);
		// }
		//
		// _tween.Finished += TurnOffAllButtons;
		//
		// void TurnOffAllButtons()
		// {
		// 	GD.Print("Turning off all buttons");
		// 	foreach (var node in _buttons)
		// 	{
		// 		var tempButton = node as Button;
		// 		if(tempButton == button) continue;
		// 		tempButton.Visible = false;
		// 	}
		// 	ExpandContainer();
		// }
	}

	private void TurnOnMainContainerButtons()
	{
		foreach (var button in _buttonHolders)
		{
			//var button = node as Button;
			if (button == null) GD.PrintErr("Button found Null");
			button!.Visible = true;
		}

		_mainButtonsBg.Visible = true;
	}

	private void TurnOffMainContainerButtons(string str)
	{
		foreach (var buttonHolder in _buttonHolders)
		{
			//var button = node as Button;
			var button = buttonHolder.GetChild(0).GetNode<Button>(".");
			if (button == null)
			{
				GD.PrintErr("Button found Null");
				return;
			}
			GD.Print(button.Text+" "+str);
			if(button!.Text.Equals(str)) continue;
			buttonHolder.Visible = false;
		}
		_mainButtonsBg.Visible = false;

	}

	private void TurnOffSubButtons()
	{
		_subButtonContainer.Visible = true;
		var subPanels = _subButtonContainer.GetChildren();
		foreach (var panel in subPanels)
		{
			var container = panel as HBoxContainer;
			container!.Visible = false;
		}
		
		_subButtonContainer.Visible = true;
	}

	private void TurnOnSubButtons(string str)
	{
		TurnOffSubButtons();
		
		switch (str)
		{
			case "Administration":
				_adminContainer.Visible = true;
				break;
			case "Exhibit":
				_exhibitsContainer.Visible = true;
				break;
			case "Facilities":
				_facilitiesContainer.Visible = true;
				break;
			case "Room":
				_roomContainer.Visible = true;
				break;
			case "Staff":
				_staffContainer.Visible = true;
				break;
		}
	}

	private void ExpandContainer()
	{
		_tween = CreateTween();
		_adminContainer.CustomMinimumSize = new Vector2(500, 50);
		_adminContainer.UpdateMinimumSize();
		_adminContainer.GrowHorizontal = Control.GrowDirection.Begin;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnBottomPanelButtonClicked -= OnClickButton;
	}
}