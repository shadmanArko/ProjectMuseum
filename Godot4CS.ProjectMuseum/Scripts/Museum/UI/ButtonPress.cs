using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.UI;

public partial class ButtonPress : Button
{
	[Export] private string _stringName;

	public void OnButtonPressed()
	{
		MuseumActions.OnBottomPanelButtonClicked?.Invoke(_stringName);
		GD.PrintErr(_stringName +" button is pressed");
	}
}