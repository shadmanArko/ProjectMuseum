using ProjectMuseum.Models;
using ProjectMuseum.Models.Artifact_and_Inventory;

namespace ProjectMuseum.DTOs;

public class InventoryDTO
{
    public Inventory Inventory { get; set; }
    public ArtifactStorage ArtifactStorage { get; set; }
}