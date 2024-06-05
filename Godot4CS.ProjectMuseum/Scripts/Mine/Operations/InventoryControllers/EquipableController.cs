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
        var pickaxeEquipables = JsonSerializer.Deserialize<List<EquipablePickaxe>>(jsonStr);
        
        if (pickaxeEquipables == null)
        {
            GD.PrintErr("pickaxe equipable is null in pickaxe equipable DTO");
            return;
        }
        
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
        GD.Print("DEACTIVATING EQUIPABLE CONTROLLER");
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #region Subscribe and Unsubscribe to Actions

    private void SubscribeToActions()
    {
        switch (_inventoryItem.Category)
        {
            case "Pickaxe":
                GD.Print("SUBSCRIBING TO PICKAXE ACTIONS");
                MineActions.OnLeftMouseHeldActionStarted += DigActionStart;
                MineActions.OnLeftMouseHeldActionEnded += DigActionEnd;
                MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("PickaxeSelected");
                break;
            case "Melee":
                MineActions.OnLeftMouseClickAction += MeleeActionStart;
                MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("SwordSelected");
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
        
        MineActions.OnMeleeAttackActionStarted?.Invoke();
        MuseumActions.OnPlayerPerformedTutorialRequiringAction.Invoke("AttackActionCompleted");
    }

    #endregion

    #region Dig Start and End

    private void DigActionStart()
    {
        _playerControllerVariables.IsDigging = true;
        _playerControllerVariables.CanMove = false;
        var pickAxe = _equipableDto.PickaxeEquipables.FirstOrDefault(temp => temp.Variant == _inventoryItem.Variant);
        _playerControllerVariables.CellDamagePoint = pickAxe!.Damage;
        _playerControllerVariables.CellHitCoolDown = pickAxe!.Cooldown;
        SetPhysicsProcess(true);
    }

    private void DigActionEnd()
    {
        SetPhysicsProcess(false);
        _playerControllerVariables.IsDigging = false;
        _playerControllerVariables.CanMove = true;
        _playerControllerVariables.CanMoveLeftAndRight = true;
    }

    #endregion

    public override void _PhysicsProcess(double delta)
    {
        if (_playerControllerVariables.CanDig && _playerControllerVariables.PlayerEnergy > 0)
        {
            _playerControllerVariables.CanMove = false;
            _playerControllerVariables.CanMoveLeftAndRight = false;
            _playerControllerVariables.Player.Velocity = new Vector2(0, _playerControllerVariables.Velocity.Y);
            MineActions.OnDigActionStarted?.Invoke();
        }
        else
            DigActionEnd();
    }

    #endregion
}