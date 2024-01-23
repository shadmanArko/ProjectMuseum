using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Elevator;

public partial class ElevatorPlatformController : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    private HttpRequest _constructPlatformInMineHttpRequest;
    private HttpRequest _deconstructPlatformInMineHttpRequest;

    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        CreateHttpRequests();
        InitializeDiInstaller();
        SubscribeToActions();
    }

    private void CreateHttpRequests()
    {
        CreateConstructPlatformInMineHttpRequest();
    }
    
    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        
    }


    #region Construct Platform

    private void CreateConstructPlatformInMineHttpRequest()
    {
        _constructPlatformInMineHttpRequest = new HttpRequest();
        AddChild(_constructPlatformInMineHttpRequest);
        _constructPlatformInMineHttpRequest.RequestCompleted += OnConstructPlatformInMineHttpRequestCompleted;
    }
    
    public void ConstructPlatformInMine()
    {
        
    }

    private void OnConstructPlatformInMineHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        
    }

    #endregion

    #region Deconstruct Platform

    public void DeconstructPlatformInMine()
    {
        
    }

    #endregion

    public void Buy()
    {
        
    }

    public void Sell()
    {
        
    }

    public void PlayerEnter()
    {
        
    }

    public void PlayerExit()
    {
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}