using Godot;
using System;

public partial class Graphics : TabBar
{
	// [Export]
	// public NodePath OptionButtonPath; // Path to your OptionButton node
	// [Export]
	// public NodePath OptionMenuPath;  // Path to your OptionMenu node
	//
	[Export] private OptionButton optionButton;
	// //private OptionMenu optionMenu;

	public enum WindowMode
	{
		FullScreen,
		Windowed,
		// BorderlessWindow,
		// BorderlessFullScreen,
	}

	public override void _Ready()
	{
		//optionButton = GetNode<OptionButton>(OptionButtonPath);
		//optionMenu = GetNode<OptionMenu>(OptionMenuPath);

		// Add window mode options to the menu
		foreach (WindowMode mode in Enum.GetValues<WindowMode>())
		{
			optionButton.AddItem(mode.ToString());
		}
		
		//optionButton.ItemSelected += _on_window_mode_selected(optionButton.GetItemIndex())

		// Set initial selection and window mode
		// optionMenu.Select(Convert.ToInt32(WindowMode.BorderlessFullScreen));
		// OS.WindowFullscreen = true; // Set initial full-screen mode
	}

	public void _on_OptionButton_pressed()
	{
		// Handle option button press (optional for visual feedback)
	}
	
	public void OnWindowModeSelected(int index)
	{
		//WindowMode newMode = (WindowMode)Enum.Parse(typeof(WindowMode), optionButton.GetText(index));
		WindowMode newMode = (WindowMode)index;

		switch (newMode)
		{
			case WindowMode.FullScreen:
				//OS.WindowFullscreen = true;
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				break;
			case WindowMode.Windowed:
				//OS.WindowFullscreen = false;
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				//OS.WindowSize = new Vector2(800, 600); // Adjust default window size as needed
				break;
			// case WindowMode.BorderlessWindow:
			// 	//OS.WindowFullscreen = false;
			// 	DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			// 	DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
			// 	break;
			// case WindowMode.BorderlessFullScreen:
			// 	DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			// 	DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
			// 	break;
		}
	}
}
