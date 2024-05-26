using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Models.MIne.Equipables;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations.InventoryControllers;

public partial class EquipableController : InventoryController
{
    private HttpRequest _getAllMeleeEquipablesHttpRequest;
    private HttpRequest _getAllRangedEquipablesHttpRequest;
    private HttpRequest _getAllPickaxeEquipablesHttpRequest;
    
    private PlayerControllerVariables _playerControllerVariables;
    private EquipableDTO _equipableDto;

    private InventoryItem _inventoryItem;

    #region Initializers

    public override void _EnterTree()
    {
        CreateHttpRequests();
    }

    public override void _Ready()
    {
        InitializeDiReference();
        SetProcess(false);
        GetAllMeleeEquipables();
        GetAllRangedEquipables();
        GetAllPickaxeEquipables();
        GD.PrintErr("Equipable Controller activated");
    }

    private void InitializeDiReference()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _equipableDto = ServiceRegistry.Resolve<EquipableDTO>();
    }
    
    private void CreateHttpRequests()
    {
        _getAllMeleeEquipablesHttpRequest = new HttpRequest();
        AddChild(_getAllMeleeEquipablesHttpRequest);
        _getAllMeleeEquipablesHttpRequest.RequestCompleted +=
            OnGetAllMeleeEquipablesHttpRequestComplete;
        
        _getAllRangedEquipablesHttpRequest = new HttpRequest();
        AddChild(_getAllRangedEquipablesHttpRequest);
        _getAllRangedEquipablesHttpRequest.RequestCompleted +=
            OnGetAllRangedEquipablesHttpRequestComplete;
        
        _getAllPickaxeEquipablesHttpRequest = new HttpRequest();
        AddChild(_getAllPickaxeEquipablesHttpRequest);
        _getAllPickaxeEquipablesHttpRequest.RequestCompleted +=
            OnGetAllPickaxeEquipablesHttpRequestComplete;
    }
    
    #region Populate Equipables DTO

    #region Melee Equipables

    private void GetAllMeleeEquipables()
    {
        var url = ApiAddress.MineApiPath + "GetAllMeleeEquipables";
        _getAllMeleeEquipablesHttpRequest.CancelRequest();
        _getAllMeleeEquipablesHttpRequest.Request(url);
    }
    
    private void OnGetAllMeleeEquipablesHttpRequestComplete(long result, long responseCode,
        string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var meleeEquipables = JsonSerializer.Deserialize<List<EquipableMelee>>(jsonStr);
        
        if (meleeEquipables == null)
        {
            GD.PrintErr("melee equipable is null in melee equipable DTO");
            return;
        }
        GD.PrintErr($"Melee count: {meleeEquipables.Count}");
        _equipableDto.MeleeEquipables = meleeEquipables;
    }

    #endregion
    
    #region Ranged Equipables

    private void GetAllRangedEquipables()
    {
        var url = ApiAddress.MineApiPath + "GetAllRangedEquipables";
        _getAllRangedEquipablesHttpRequest.CancelRequest();
        _getAllRangedEquipablesHttpRequest.Request(url);
    }
    
    private void OnGetAllRangedEquipablesHttpRequestComplete(long result, long responseCode,
        string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var rangedEquipables = JsonSerializer.Deserialize<List<EquipableRange>>(jsonStr);
        
        if (rangedEquipables == null)
        {
            GD.PrintErr("ranged equipable is null in ranged equipable DTO");
            return;
        }
        GD.PrintErr($"ranged count: {rangedEquipables.Count}");
        _equipableDto.RangedEquipables = rangedEquipables;
    }

    #endregion
    
    #region Pickaxe Equipables

    private void GetAllPickaxeEquipables()
    {
        var url = ApiAddress.MineApiPath + "GetAllPickaxeEquipables";
        _getAllPickaxeEquipablesHttpRequest.CancelRequest();
        _getAllPickaxeEquipablesHttpRequest.Request(url);
    }
    
    private void OnGetAllPickaxeEquipablesHttpRequestComplete(long result, long responseCode,
        string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        GD.PrintErr($"pickaxe list: {body}");
        var pickaxeEquipables = JsonSerializer.Deserialize<List<EquipablePickaxe>>(jsonStr);
        
        if (pickaxeEquipables == null)
        {
            GD.PrintErr("pickaxe equipable is null in pickaxe equipable DTO");
            return;
        }
        GD.PrintErr($"pickaxe count: {pickaxeEquipables.Count}");
        _equipableDto.PickaxeEquipables = pickaxeEquipables;
    }

    #endregion
    
    #endregion
    
    public override void ActivateController(InventoryItem inventoryItem)
    {
        IsControllerActivated = true;
        _inventoryItem = inventoryItem;
        SubscribeToActions();
        GD.Print("Equipable controller activated");
        GD.Print($"equipable: {inventoryItem.Variant}");
    }

    public override void DeactivateController()
    {
        if (!IsControllerActivated) return;
        IsControllerActivated = false;
        MineActions.OnLeftMouseHeldActionEnded?.Invoke();
        UnsubscribeToActions();
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #region Subscribe and Unsubscribe to Actions

    private void SubscribeToActions()
    {
        switch (_inventoryItem.Category)
        {
            case "Pickaxe":
                MineActions.OnLeftMouseHeldActionStarted += DigActionStart;
                MineActions.OnLeftMouseHeldActionEnded += DigActionEnd;
                GD.Print("subscribed to pickaxe actions");
                break;
            case "Melee":
                MineActions.OnLeftMouseClickAction += MeleeActionStart;
                GD.Print("subscribed to melee actions");
                break;
            case "Ranged":
                break;
        }

        MineActions.DeselectAllInventoryControllers += DeactivateController;
    }

    private void UnsubscribeToActions()
    {
        switch (_inventoryItem.Category)
        {
            case "Pickaxe":
                MineActions.OnLeftMouseHeldActionStarted -= DigActionStart;
                MineActions.OnLeftMouseHeldActionEnded -= DigActionEnd;
                break;
            case "Melee":
                MineActions.OnLeftMouseClickAction -= MeleeActionStart;
                break;
            case "Ranged":
                break;
        }

        MineActions.DeselectAllInventoryControllers -= DeactivateController;
    }

    #endregion

    #region Melee Attack

    private void MeleeActionStart()
    {
        if (!_playerControllerVariables.CanAttack) return;
        var meleeWeapon = _equipableDto.MeleeEquipables.FirstOrDefault(temp => temp.Variant == _inventoryItem.Variant);
        _playerControllerVariables.EnemyDamagePoint = meleeWeapon!.Damage;
        _playerControllerVariables.EnemyHitCoolDown = meleeWeapon!.Cooldown;
        GD.Print($"Melee damage to enemy: {_playerControllerVariables.EnemyDamagePoint}");
        MineActions.OnMeleeAttackActionStarted?.Invoke();
        MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("AttackActionCompleted");
    }

    #endregion

    #region Dig Start and End

    private void DigActionStart()
    {
        _playerControllerVariables.IsDigging = true;
        // GD.PrintErr($"equipableDTO pickaxe is null: {_equipableDto.PickaxeEquipables == null}");
        // GD.PrintErr($"pickaxe list count: {_equipableDto.PickaxeEquipables.Count}");
        
        var pickAxe = _equipableDto.PickaxeEquipables.FirstOrDefault(temp => temp.Variant == _inventoryItem.Variant);
        _playerControllerVariables.CellDamagePoint = pickAxe!.Damage;
        _playerControllerVariables.CellHitCoolDown = pickAxe!.Cooldown;
        SetProcess(true);
    }

    private void DigActionEnd()
    {
        SetProcess(false);
        _playerControllerVariables.IsDigging = false;
    }

    #endregion

    public override void _Process(double delta)
    {
        if (_playerControllerVariables.CanDig && _playerControllerVariables.PlayerEnergy > 0)
        {
            _playerControllerVariables.Velocity = new Vector2(0, _playerControllerVariables.Velocity.Y);
            MineActions.OnDigActionStarted?.Invoke();
            GD.Print($"Pickaxe damage to cell: {_playerControllerVariables.CellDamagePoint}");
            MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("DigActionCompleted");
        }
        else
            DigActionEnd();
    }

    #endregion
}