using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using Godot;
using Godot.Collections;

namespace Godot4CS.ProjectMuseum.Scripts.UI;

public partial class ButtonsTransition : Node
{
	[Export] private Array<Node> _buttons;
	private Tween _tween;

	private Color _invisibleColor = new Color(0, 0, 0, 0); 
	public override void _Ready()
	{
		
		_buttons = new Array<Node>();
	}

	public void OnClickButton()
	{
		_buttons = GetChildren();
		var button = _buttons[3] as Button;

		foreach (var button1 in _buttons)
		{
			var tempButton = button1 as Button;
			if(tempButton == button) continue;
			_tween = CreateTween();
			GD.Print("making invisible");
			_tween.TweenProperty(tempButton, "self_modulate", _invisibleColor, 0.2f).Finished +=TurnOffAllButtons;
		}
	}

	private void TurnOffAllButtons()
	{
		GD.Print("Turning off all buttons");
	}
}