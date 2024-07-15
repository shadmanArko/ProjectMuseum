using System.Collections.Generic;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;

public static class StringHelpers
{
    public static bool IsUnassigned(this string variable)
    {
        if (variable == "" || variable == "string" || variable == "String") return true;
        else
        {
            return false;
        }
    }
    
}