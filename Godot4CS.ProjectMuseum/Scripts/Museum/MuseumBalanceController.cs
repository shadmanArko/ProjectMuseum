using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class MuseumBalanceController: Node2D
{
    private HttpRequest _httpRequestForGettingBalance;
    private HttpRequest _httpRequestForReducingBalance;
    private HttpRequest _httpRequestForAddingMoney;
    private MuseumRunningDataContainer _museumRunningDataContainer;
    public override void _Ready()
    {
        _httpRequestForAddingMoney = new HttpRequest();
        _httpRequestForGettingBalance = new HttpRequest();
        _httpRequestForReducingBalance = new HttpRequest();
        AddChild(_httpRequestForAddingMoney);
        AddChild(_httpRequestForGettingBalance);
        AddChild(_httpRequestForReducingBalance);
        _httpRequestForAddingMoney.RequestCompleted += HttpRequestForAddingMoneyOnRequestCompleted;
        _httpRequestForGettingBalance.RequestCompleted += HttpRequestForGettingBalanceOnRequestCompleted;
        _httpRequestForReducingBalance.RequestCompleted += HttpRequestForReducingBalanceOnRequestCompleted;
        MuseumActions.OnMuseumBalanceAdded += OnMuseumBalanceAdded;
        MuseumActions.OnMuseumBalanceReduced += OnMuseumBalanceReduced;
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
        string url = $"{ApiAddress.UrlPrefix}Museum/GetMuseumBalance/museum0";
        // _httpRequestForGettingBalance.Request(url);
        var museum = SaveLoadService.Load().Museum;
        _museumRunningDataContainer.Museum = museum;
        UpdateBalance(museum.Money);

    }

    private void HttpRequestForReducingBalanceOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        ExtractBalance(body);
    }

    private static void ExtractBalance(byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        var museumBalance = JsonSerializer.Deserialize<float>(jsonStr);
        UpdateBalance(museumBalance);
    }

    private static void UpdateBalance(float museumBalance)
    {
        MuseumActions.OnMuseumBalanceUpdated?.Invoke(museumBalance);
    }

    private void HttpRequestForGettingBalanceOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        ExtractBalance(body);
    }

    private void HttpRequestForAddingMoneyOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        ExtractBalance(body);
    }

    private void OnMuseumBalanceUpdated(float obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnMuseumBalanceReduced(float amount)
    {
        _museumRunningDataContainer.Museum.Money -= amount;
        UpdateBalance(_museumRunningDataContainer.Museum.Money);
    }

    private void OnMuseumBalanceAdded(float amount)
    {
        _museumRunningDataContainer.Museum.Money += amount;
        UpdateBalance(_museumRunningDataContainer.Museum.Money);

    }

    public override void _ExitTree()
    {
        _httpRequestForAddingMoney.RequestCompleted -= HttpRequestForAddingMoneyOnRequestCompleted;
        _httpRequestForGettingBalance.RequestCompleted -= HttpRequestForGettingBalanceOnRequestCompleted;
        _httpRequestForReducingBalance.RequestCompleted -= HttpRequestForReducingBalanceOnRequestCompleted;
        MuseumActions.OnMuseumBalanceAdded -= OnMuseumBalanceAdded;
        MuseumActions.OnMuseumBalanceReduced -= OnMuseumBalanceReduced;
    }
}