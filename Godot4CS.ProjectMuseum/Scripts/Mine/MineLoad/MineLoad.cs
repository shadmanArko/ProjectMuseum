using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne.Equipables;
using Resource = ProjectMuseum.Models.MIne.Resource;

public partial class MineLoad : Node
{
    private ProceduralMineGenerationDto _proceduralMineGenerationDto;
    private RawArtifactDTO _rawArtifactDto;
    private MineCellCrackMaterial _mineCellCrackMaterial;
    private WallPlaceableDTO _wallPlaceableDto;
    private CellPlaceableDTO _cellPlaceableDto;
    private EquipableDTO _equipableDto;
    private ConsumableDTO _consumableDto;
    
    private InventoryDTO _inventoryDto;

    #region Database Variables

    private ProceduralMineGenerationData _proceduralMineGenerationDatabase;
    private List<RawArtifactDescriptive> _rawArtifactDescriptiveDatabase;
    private List<RawArtifactFunctional> _rawArtifactFunctionalDatabase;
    
    private List<SpecialBackdropPngInformation> _specialBackdropsDatabase;
    private List<ArtifactCondition> _artifactConditionsDatabase;
    private List<SiteArtifactChanceData> _siteArtifactChanceDatabase;
    private List<CellCrackMaterial> _cellCrackMaterialsDatabase;
    private List<ArtifactRarity> _artifactRarityDatabase;
    
    private List<WallPlaceable> _wallPlaceableDatabase;
    private List<CellPlaceable> _cellPlaceableDatabase;
    
    private List<EquipableMelee> _equipableMeleeDatabase;
    private List<EquipablePickaxe> _equipablePickaxeDatabase;
    private List<EquipableRange> _equipableRangeDatabase;

    private List<Consumable> _consumableDatabase;
    
    private List<Resource> _resourceDatabase;

    #endregion

    public override void _EnterTree()
    {
        InitializeDatabases();
        InitializeDiReference();
        LoadDataFromDatabaseToDiReference();
        LoadFromSaveData();
        MineActions.OnDatabaseLoad?.Invoke();
        InitializeInventory();
    }

    private void InitializeDiReference()
    {
        _proceduralMineGenerationDto = ServiceRegistry.Resolve<ProceduralMineGenerationDto>();
        _rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
        _mineCellCrackMaterial = ServiceRegistry.Resolve<MineCellCrackMaterial>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
        _wallPlaceableDto = ServiceRegistry.Resolve<WallPlaceableDTO>();
        _cellPlaceableDto = ServiceRegistry.Resolve<CellPlaceableDTO>();
        _equipableDto = ServiceRegistry.Resolve<EquipableDTO>();
        _consumableDto = ServiceRegistry.Resolve<ConsumableDTO>();
    }
    
    private void InitializeDatabases()
    {
        var gameDataFolderLocation = DataPath.GameDataFolderPath;
        
        _rawArtifactDto = new RawArtifactDTO();
        _proceduralMineGenerationDto = new ProceduralMineGenerationDto();
        _inventoryDto = new InventoryDTO();
        
        var rawArtifactDescriptiveJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}RawArtifactData/RawArtifactDescriptiveData/RawArtifactDescriptiveDataEnglish.json",FileAccess.ModeFlags.Read).GetAsText();
        _rawArtifactDescriptiveDatabase =
            JsonSerializer.Deserialize<List<RawArtifactDescriptive>>(rawArtifactDescriptiveJson);
        GD.Print($"artifact dto: {_rawArtifactDescriptiveDatabase}");
        
        var rawArtifactFunctionalJson =
            FileAccess.Open($"{gameDataFolderLocation}RawArtifactData/RawArtifactFunctionalData/RawArtifactFunctionalData.json",FileAccess.ModeFlags.Read).GetAsText();
        _rawArtifactFunctionalDatabase=
            JsonSerializer.Deserialize<List<RawArtifactFunctional>>(rawArtifactFunctionalJson);
        
        var backdropsJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}SpecialBackdropData/SpecialBackdropPngInformation.json",FileAccess.ModeFlags.Read).GetAsText();
        _specialBackdropsDatabase = new List<SpecialBackdropPngInformation>();
        _specialBackdropsDatabase = JsonSerializer.Deserialize<List<SpecialBackdropPngInformation>>(backdropsJson);
        
        var mineGenDataJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}ProceduralGenerationData/ProceduralMineGenerationData.json",FileAccess.ModeFlags.Read).GetAsText();
        _proceduralMineGenerationDatabase = JsonSerializer.Deserialize<ProceduralMineGenerationData>(mineGenDataJson);
        

        var siteArtifactDataJson = FileAccess.Open(
            $"{gameDataFolderLocation}ProceduralGenerationData/SiteArtifactChanceData/SiteArtifactChanceFunctionalData/SiteArtifactChanceFunctionalData.json",FileAccess.ModeFlags.Read).GetAsText();
        _siteArtifactChanceDatabase = JsonSerializer.Deserialize < List<SiteArtifactChanceData>>(siteArtifactDataJson);
        

        var artifactConditionJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}ArtifactScore/ArtifactCondition.json",FileAccess.ModeFlags.Read).GetAsText();
        _artifactConditionsDatabase = JsonSerializer.Deserialize<List<ArtifactCondition>>(artifactConditionJson);
        

        var artifactRarityJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}ArtifactScore/ArtifactRarity.json",FileAccess.ModeFlags.Read).GetAsText();
        _artifactRarityDatabase = JsonSerializer.Deserialize<List<ArtifactRarity>>(artifactRarityJson);

        var resourceJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}Resource/Resource.json",FileAccess.ModeFlags.Read).GetAsText();
        _resourceDatabase = JsonSerializer.Deserialize<List<Resource>>(resourceJson);

        var cellCrackMaterialJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}CellCrackMaterial/CellCrackMaterial.json",FileAccess.ModeFlags.Read).GetAsText();
        _cellCrackMaterialsDatabase = JsonSerializer.Deserialize<List<CellCrackMaterial>>(cellCrackMaterialJson);
        
        var wallPlaceableJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}WallPlaceableData/WallPlaceable.json",FileAccess.ModeFlags.Read).GetAsText();
        _wallPlaceableDatabase = JsonSerializer.Deserialize<List<WallPlaceable>>(wallPlaceableJson);

        var cellPlaceableJson =
            FileAccess.Open(
                $"{gameDataFolderLocation}CellPlaceableData/CellPlaceable.json",FileAccess.ModeFlags.Read).GetAsText();
        _cellPlaceableDatabase = JsonSerializer.Deserialize<List<CellPlaceable>>(cellPlaceableJson);

        var equipableMeleeJson = FileAccess.Open($"{gameDataFolderLocation}Equipable/EquipableMelee.json",FileAccess.ModeFlags.Read).GetAsText();
        _equipableMeleeDatabase = JsonSerializer.Deserialize<List<EquipableMelee>>(equipableMeleeJson);
        
        var equipablePickaxeJson = FileAccess.Open($"{gameDataFolderLocation}Equipable/EquipablePickaxe.json",FileAccess.ModeFlags.Read).GetAsText();
        _equipablePickaxeDatabase = JsonSerializer.Deserialize<List<EquipablePickaxe>>(equipablePickaxeJson);
        
        var equipableRangedJson = FileAccess.Open($"{gameDataFolderLocation}Equipable/EquipableRange.json",FileAccess.ModeFlags.Read).GetAsText();
        _equipableRangeDatabase = JsonSerializer.Deserialize<List<EquipableRange>>(equipableRangedJson);

        var consumableJson = FileAccess.Open($"{gameDataFolderLocation}Consumable/Consumable.json",FileAccess.ModeFlags.Read).GetAsText();
        _consumableDatabase = JsonSerializer.Deserialize<List<Consumable>>(consumableJson);
        
    }
    
    private void LoadDataFromDatabaseToDiReference()
    {
        _rawArtifactDto.RawArtifactDescriptives = _rawArtifactDescriptiveDatabase;
        _rawArtifactDto.RawArtifactFunctionals = _rawArtifactFunctionalDatabase;
        _mineCellCrackMaterial.CellCrackMaterials = _cellCrackMaterialsDatabase;
        _wallPlaceableDto.WallPlaceables = _wallPlaceableDatabase;
        _cellPlaceableDto.CellPlaceables = _cellPlaceableDatabase;
        _equipableDto.MeleeEquipables = _equipableMeleeDatabase;
        _equipableDto.PickaxeEquipables = _equipablePickaxeDatabase;
        _equipableDto.RangedEquipables = _equipableRangeDatabase;
        _consumableDto.Consumables = _consumableDatabase;

        _proceduralMineGenerationDto.ProceduralMineGenerationData = _proceduralMineGenerationDatabase;
        _proceduralMineGenerationDto.SpecialBackdropPngInformations = _specialBackdropsDatabase;
        _proceduralMineGenerationDto.ArtifactConditions = _artifactConditionsDatabase;
        _proceduralMineGenerationDto.ArtifactRarities = _artifactRarityDatabase;
        _proceduralMineGenerationDto.SiteArtifactChances = _siteArtifactChanceDatabase;
        _proceduralMineGenerationDto.Resources = _resourceDatabase;
    }

    private void LoadFromSaveData()
    {
        var saveData = SaveLoadService.Load();

        var inventory = saveData.Inventory;
        var artifactStorage = saveData.ArtifactStorage;

        _inventoryDto.Inventory = inventory;
        _inventoryDto.ArtifactStorage = artifactStorage;
    }

    private async void InitializeInventory()
    {
        await Task.Delay(2000);
        MineActions.OnInventoryInitialized?.Invoke();
    }

}