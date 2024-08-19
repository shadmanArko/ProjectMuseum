using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Model;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.ShopSystem;

public partial class ShopManager: Node
{
    //have all shops 
    public int numberOfFoodSell;
    public int numberOfDrinkSell;
    public int numberOfSouvenirSell;
    public float revenueFromFoodSell;
    public float revenueFromDrinkSell;
    public float revenueFromSouvenirSell;
    private MuseumRunningDataContainer _museumRunningDataContainer;
    //Find suitable shop, destination and product
    public override async void _Ready()
    {
        base._Ready();
        await Task.Delay(1000);
        MuseumActions.DayEndReportGenerated += DayEndReportGenerated;
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
    }

    private void DayEndReportGenerated(MuseumDayEndReport obj)
    {
        numberOfFoodSell = 0;
        numberOfDrinkSell = 0;
        numberOfSouvenirSell = 0;
        revenueFromFoodSell = 0;
        revenueFromDrinkSell = 0;
        revenueFromSouvenirSell = 0;
    }


    public void SellProduct(Product product)
    {
        if (product.FulfilsGuestNeed == GuestNeedsEnum.Hunger)
        {
            numberOfFoodSell++;
            revenueFromFoodSell += product.BasePrice;
        }else if (product.FulfilsGuestNeed == GuestNeedsEnum.Thirst)
        {
            numberOfDrinkSell++;
            revenueFromDrinkSell += product.BasePrice;
        }
        else if (product.FulfilsGuestNeed == GuestNeedsEnum.Souvenir)
        {
            numberOfSouvenirSell++;
            revenueFromSouvenirSell += product.BasePrice;
        }
        MuseumActions.OnMuseumBalanceAdded?.Invoke(product.BasePrice);
    }

    public void MakeDecisionForFulfillingNeed(out Product product, out Shop shop, GuestAi guest, GuestNeedsEnum needsEnum)
    {
        product = null;
        shop = null;
        var shops = _museumRunningDataContainer.Shops;
        //if need related shops length> 0 ? continue:return null
        if (shops.Count < 1) return;
        //get all the need related shops
        var listOfNeedRelatedShops = shops.FindAll(shop1 => shop1.CoreShopFunctional.NeedsShopFullfills.Contains(needsEnum));
        if (listOfNeedRelatedShops.Count<0) return;
        //sort based on distance
        var sortedClosestShops = listOfNeedRelatedShops.OrderByDescending(shop1 =>
            DistanceToShop(shop1.XPosition, shop1.YPosition, guest.Position));
        
        foreach (var closestShop in sortedClosestShops)
        {
            GD.Print();
            //check if the shop contains item within its budget
            foreach (var shopProduct in closestShop.CoreShopFunctional.DefaultProducts.Shuffle())
            {
                if (shopProduct.FulfilsGuestNeed == needsEnum && shopProduct.BasePrice <= guest.availableMoney)
                {
                    shop = closestShop;
                    product = shopProduct;
                    return;
                }
            }
        }
    }

    float DistanceToShop(int guestXPosition, int guestYPosition, Vector2 guestPosition)
    {
        var guestCoordinate = GameManager.tileMap.LocalToMap(guestPosition);
        var shopCoordinate = new Vector2I(guestXPosition, guestYPosition);
        return guestCoordinate.ManhattanDistance(shopCoordinate);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.DayEndReportGenerated -= DayEndReportGenerated;
    }
}