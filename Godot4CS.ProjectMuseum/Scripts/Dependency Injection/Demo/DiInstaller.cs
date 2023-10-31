using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public partial class DiInstaller : Node
{
    public DiInstaller()
    {
        GD.Print("DiInstaller");
        ServiceRegistry.RegisterTransient<RandomGuidGenerator>();
        ServiceRegistry.Initialize();
    }
}