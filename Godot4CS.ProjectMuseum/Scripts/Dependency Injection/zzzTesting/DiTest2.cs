using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public partial class DiTest2 : Node
{
    private readonly RandomGuidGenerator _randomGuidGenerator;
    // public DiTest2(RandomGuidGenerator _randomGuidGenerator)
    // {
    //     _randomGuidGenerator = ServiceRegistry.Resolve<RandomGuidGenerator>();
    //     GD.Print(_randomGuidGenerator.RandomGuid);
    // }
    
    // public override void _Ready()
    // {
    //     GD.Print("_randomGuidGenerator.RandomGuid");
    //     var	_randomGuidGenerator = ServiceRegistry.Resolve<RandomGuidGenerator>();
    //     GD.Print(_randomGuidGenerator.RandomGuid);
    // }

    
}