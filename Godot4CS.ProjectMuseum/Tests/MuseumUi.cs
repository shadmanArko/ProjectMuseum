using Godot;
using System;
using System.Collections.Generic;
using Godot.DependencyInjection.Attributes;
using ProjectMuseum.Models;

public partial class MuseumUi : Control  // Replace with the appropriate node type for your UI
{
    private PackedScene item1;
    private PackedScene item2;

    [Inject]
    public List<ExhibitPlacementConditionData> ExhibitPlacementConditionDatas { get; set; }
    
    [Inject]
    public void Inject(List<ExhibitPlacementConditionData> exhibitPlacementConditionDatas)
    {
        ExhibitPlacementConditionDatas = exhibitPlacementConditionDatas;
    }
    public override void _Ready()
    {
        item1 = (PackedScene)ResourceLoader.Load("res://item_1.tscn");
        item2 = (PackedScene)ResourceLoader.Load("res://item_2.tscn");
    }

    public void OnExhibit0Pressed()
    {
        var instance = (Node)item1.Instantiate();
        GetTree().Root.AddChild(instance);
        var scriptInstance = instance.GetNode("." /* Replace with the actual path to the script node */);

        if (scriptInstance != null)
        {
            // Now you can access properties or call methods on the script instance
            scriptInstance.Set("selectedItem", true);
        }
        else
        {
            GD.Print("Item script not found");
        }
    }

    public void OnExhibit3Pressed()
    {
        var instance = (Node)item2.Instantiate();
        GetTree().Root.AddChild(instance);
        var scriptInstance = instance.GetNode("." /* Replace with the actual path to the script node */);

        if (scriptInstance != null)
        {
            // Now you can access properties or call methods on the script instance
            scriptInstance.Set("selectedItem", true);
        }
        else
        {
            GD.Print("Item script not found");
        }
    }
}