using System.Diagnostics;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.ASPLauncher;

public partial class AspLauncher : Node2D
{
    public override void _EnterTree()
    {
        const string batchScriptPath = "C:\\Users\\Red Thorn PC\\Desktop\\AspLauncher.bat";
        Process.Start(batchScriptPath);
    }
}