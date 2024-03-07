using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Graphics : TabBar
{
	// [Export]
	// public NodePath OptionButtonPath; // Path to your OptionButton node
	// [Export]
	// public NodePath OptionMenuPath;  // Path to your OptionMenu node
	//
	[Export] private OptionButton _windowModeOptionButton;
	[Export] private OptionButton _resolutionOptionButton;
	// //private OptionMenu optionMenu;
	Vector2I resolution = new Vector2I(500, 1000);
	public enum WindowMode
	{
		FullScreen,
		Windowed,
		// BorderlessWindow,
		// BorderlessFullScreen,
	}

	public List<string> Resolution = new List<string>
	{
		"1920x1080",
		"1600x900",
		// "1440x810",
		"1280x720",
		// "1024x576",
		// "960x540",
		// "800x450",
		// "720x405",
		// "640x360"
	};

	public override void _Ready()
	{
		//_windowModeOptionButton = GetNode<OptionButton>(OptionButtonPath);
		//optionMenu = GetNode<OptionMenu>(OptionMenuPath);

		// Add window mode options to the menu
		foreach (WindowMode mode in Enum.GetValues<WindowMode>())
		{
			_windowModeOptionButton.AddItem(mode.ToString());
		}

		foreach (var res in Resolution)
		{
			_resolutionOptionButton.AddItem(res);
		}
		
		GD.Print(DisplayServer.ScreenGetSize());
		DisplayServer.WindowSetSize(resolution);

		//_windowModeOptionButton.ItemSelected += _on_window_mode_selected(_windowModeOptionButton.GetItemIndex())

		// Set initial selection and window mode
		// optionMenu.Select(Convert.ToInt32(WindowMode.BorderlessFullScreen));
		// OS.WindowFullscreen = true; // Set initial full-screen mode
	}

	public void _on_OptionButton_pressed()
	{
		// Handle option button press (optional for visual feedback)
	}

	public void OnResolutionSelected(int index)
	{
		var resolutionText = _resolutionOptionButton.GetItemText(index);
		GD.Print(resolutionText);

		switch (resolutionText)
		{
			case "1920x1080":
		 		resolution =new Vector2I(1920, 1080);	
				DisplayServer.WindowSetSize(resolution);
				break;
			case "1600x900":
				resolution =new Vector2I(1600, 900);	
				DisplayServer.WindowSetSize(resolution);
				break;
			case "1440x810":
				resolution =new Vector2I(1440, 810);	
				DisplayServer.WindowSetSize(resolution);
				break;
			case "1280x720":
				resolution =new Vector2I(1280, 720);	
				DisplayServer.WindowSetSize(resolution);
				break;
			case "1024x576":
				resolution =new Vector2I(1024, 576);	
				DisplayServer.WindowSetSize(resolution);
				break;
			case "960x540":
				resolution =new Vector2I(960, 540);	
				DisplayServer.WindowSetSize(resolution);
				break;
			case "800x450":
				resolution =new Vector2I(800, 450);	
				DisplayServer.WindowSetSize(resolution);
				break;
			case "720x405":
				resolution =new Vector2I(720, 405);	
				DisplayServer.WindowSetSize(resolution);
				break;
			case "640x360":
				resolution =new Vector2I(640, 360);	
				DisplayServer.WindowSetSize(resolution);
				break;
		}
	}
	
	public void OnWindowModeSelected(int index)
	{
		//WindowMode newMode = (WindowMode)Enum.Parse(typeof(WindowMode), _windowModeOptionButton.GetText(index));
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
