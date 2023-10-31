using System;

namespace Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;

public class RandomGuidGenerator
{
    public Guid RandomGuid { get; set; } = Guid.NewGuid();
}