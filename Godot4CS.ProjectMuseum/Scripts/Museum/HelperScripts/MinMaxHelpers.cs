using System;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;

public static class MinMaxHelpers
{
    public static float GetRandom(this MinMaxFloat minMaxFloatValue)
    {
        Random random = new Random();
        return (float)(random.NextDouble() * (minMaxFloatValue.Max - minMaxFloatValue.Min) + minMaxFloatValue.Min);
    }
    public static int GetRandom(this MinMaxInt minMaxIntValue)
    {
        return GD.RandRange(minMaxIntValue.Min, minMaxIntValue.Max);
    }
}