using Godot;
using System;
using System.Linq;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using ProjectMuseum.DTOs;

public partial class Tooltip4 : Control
{
    private RawArtifactDTO _rawArtifactDto;
    
	[Export] private RichTextLabel _label;
    [Export] private VBoxContainer _vBoxContainer;
    [Export] private MarginContainer _marginContainer;
    [Export] private Panel _panel;
    private Rect2 _rect;
    

    public override void _Ready()
    {
        InstallDiReference();
        
        //label = GetNode<Label>("Label");
        // _vBoxContainer = GetNode<VBoxContainer>("MarginContainer/VBoxContainer");
        // _marginContainer = GetNode<MarginContainer>("MarginContainer");
        // _panel = GetNode<Panel>("Panel");
       // label = GetNode<Label>("MarginContainer/VBoxContainer/Label");
         Hide();
        _rect = GetRect();
        _rect.Size = new Vector2(_vBoxContainer.GetRect().Size.X + 10, _vBoxContainer.GetRect().Size.Y + 5);

        _panel.Size = _marginContainer.Size;
        //GD.Print(_rect.Size);
    }
    
    private void InstallDiReference()
    {
        _rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
    }

    public void SetText(string text)
    {
        _label.Text = text;
       _rect.Size = new Vector2(_vBoxContainer.GetRect().Size.X + 10, _vBoxContainer.GetRect().Size.Y + 5);
       _panel.Size = _marginContainer.Size;
       Show();
       //GD.Print(_rect.Size);
    }

    public void ShowTooltip()
    {
        Show();
    }

    public void HideTooltip()
    {
        Hide();
    }

    public override void _Process(double delta)
    {
        var centerPositionOfScreen = GetViewportRect().Size / 2;
        var mousePosition = GetViewport().GetMousePosition();

        // Bottom Left
        if (mousePosition.X < centerPositionOfScreen.X && mousePosition.Y > centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(25, -_rect.Size.Y-75) ; 
        }
        
        // Top Left
        else if (mousePosition.X < centerPositionOfScreen.X && mousePosition.Y < centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(50, 50) ; 
        }
        
        // Top Right
        else if (mousePosition.X > centerPositionOfScreen.X && mousePosition.Y < centerPositionOfScreen.Y)
        {
            Position = GetViewport().GetMousePosition() + new Vector2(-_rect.Size.X-50, 50) ;
        }
        
        // Bottom Right
        else
        {
            Position = GetViewport().GetMousePosition() + new Vector2(-_rect.Size.X-50, -_rect.Size.Y-75) ; 
        }
        
        _rect.Size = new Vector2(_vBoxContainer.GetRect().Size.X + 10, _vBoxContainer.GetRect().Size.Y + 5);
        _panel.Size = _marginContainer.Size;
        
        // Follow the mouse position
        //GD.Print(GetViewportRect().Size/2);
    }
    
    public void ShowToolbarToolTooltip(string itemId, bool isArtifact)
    {
        if (isArtifact)
        {
            Show();
            var descriptiveData = _rawArtifactDto.RawArtifactDescriptives.FirstOrDefault(tempId => tempId.Id == itemId);
            var functionalData = _rawArtifactDto.RawArtifactFunctionals.FirstOrDefault(tempId => tempId.Id == itemId);
            
            var text = $"[img={100}x{100}]{functionalData?.LargeImageLocation}[/img] " +
                       $"[p][color=green][b] {descriptiveData?.ArtifactName}[/b][/color] [/p]" +
                       $"[p]{descriptiveData?.Description}[/p]";
            _label.Text = text;
            GD.Print(text);
        }
    }
}
