using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ColorControllers;

public class ColorBlendController
{
    private static readonly Color RedColor = new Color("#bc302b");
    private static readonly Color GreenColor = new Color("#649d47");


    public static void SetColorToRed(Node2D node)
    {
        node.Material.Set("shader_parameter/blend_color", RedColor);
    }
    
    public static void SetColorToGreen(Node2D node)
    {
        node.Material.Set("shader_parameter/blend_color", GreenColor);
    }
    
}