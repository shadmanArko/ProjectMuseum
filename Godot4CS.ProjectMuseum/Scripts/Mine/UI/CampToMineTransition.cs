using System.Text;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Time = ProjectMuseum.Models.Time;


namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class CampToMineTransition : Button
{
    private PlayerControllerVariables _playerControllerVariables;
    private CampExitPromptUi _campExitPromptUi;

    private AutoAnimationController _autoAnimationController;

    private HttpRequest _getTimeHttpRequest;
    private HttpRequest _updateTimeHttpRequest;
    
    public override void _Ready()
    {
        CreateHttpRequest();
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _autoAnimationController = ReferenceStorage.Instance.AutoAnimationController;
        _campExitPromptUi = ReferenceStorage.Instance.CampExitPromptUi;
        _campExitPromptUi.SleepForTheNightButton.ButtonUp += DeactivateCampExitPromptUi;
        _campExitPromptUi.ReturnToMuseumButton.ButtonUp += DeactivateCampExitPromptUi;
        _campExitPromptUi.ReturnToMineButton.ButtonUp += DeactivateCampExitPromptUi;
    }

    private void CreateHttpRequest()
    {
        _getTimeHttpRequest = new HttpRequest();
        AddChild(_getTimeHttpRequest);
        _getTimeHttpRequest.RequestCompleted += OnGetTimeHttpRequestCompleted;
        
        _updateTimeHttpRequest = new HttpRequest();
        AddChild(_updateTimeHttpRequest);
        _updateTimeHttpRequest.RequestCompleted += OnUpdateTimeHttpRequestCompleted;
    }

    private void GetAndSaveTime()
    {
        _getTimeHttpRequest.Request(ApiAddress.PlayerApiPath + "GetTime");
    }

    private void OnGetTimeHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var time = JsonSerializer.Deserialize<Time>(jsonStr);
        time.Days++;
        UpdateTime(time);
    }
    
    private void UpdateTime(Time time)
    {
        var daysInMuseum = time.Days;
        var daysInMine = ReferenceStorage.Instance.MineTimeSystem.GetTime().Days;
        var totalTimePassed = daysInMuseum + daysInMine;
        time.Days = totalTimePassed;
        
        string[] headers = { "Content-Type: application/json"};
        var body = JsonConvert.SerializeObject(time);
        string url = ApiAddress.PlayerApiPath + "SaveTime";
        _updateTimeHttpRequest.Request(url, headers, HttpClient.Method.Post, body);
        GD.Print($"days in museum: {daysInMuseum}, days in mine: {daysInMine}, total days: {time.Days}");
    }
    
    private void OnUpdateTimeHttpRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        ReferenceStorage.Instance.SceneLoader.LoadMuseumScene();
    }

    private async void TransitFromCampToMineTheNextDay()
    {
        GD.PrintErr("TransitFromCampToMine");
        var sceneTransition = ReferenceStorage.Instance.SceneTransition;
        await sceneTransition.FadeIn();
        await Task.Delay(2000);
        await sceneTransition.FadeOut();

        if (ReferenceStorage.Instance.MineTimeSystem.GetTime().Days <= 5)
        {
            ReferenceStorage.Instance.MineTimeSystem.StartNextDayMineExcavation();
            HealthSystem.RestorePlayerFullHealth(200, _playerControllerVariables);
            EnergySystem.RestoreFullEnergy(200, _playerControllerVariables);
            _autoAnimationController.SetPlayerRun();
        }
        else
            ReferenceStorage.Instance.MineTimeSystem.StartNextDayMineExcavation();
    }

    private void TransitionFromCampToMineOnTheSameDay()
    {
        _autoAnimationController.SetPlayerRun();
    }

    private async void TransitFromCampToMuseum()
    {
        DeactivateCampExitPromptUi();
        var sceneTransition = ReferenceStorage.Instance.SceneTransition;
        await sceneTransition.FadeIn();
        _playerControllerVariables.CanMove = false;
        GetAndSaveTime();
        await Task.Delay(2000);
    }
    
    private void DeactivateCampExitPromptUi()
    {
        _campExitPromptUi.Visible = false;
    }
}