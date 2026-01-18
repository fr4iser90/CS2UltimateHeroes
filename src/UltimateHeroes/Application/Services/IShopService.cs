using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Shop;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für Shop Management (temporäre Match-Items)
    /// </summary>
    public interface IShopService
    {
        // Shop Items
        void RegisterShopItem(ShopItem item);
        ShopItem? GetShopItem(string itemId);
        List<ShopItem> GetAllShopItems();
        List<ShopItem> GetShopItemsByCategory(ItemCategory category);
        
        // Player Items
        PlayerMatchItems GetPlayerItems(string steamId);
        bool CanAffordItem(string steamId, string itemId);
        bool PurchaseItem(string steamId, string itemId, CCSPlayerController? player = null);
        void ApplyItemEffect(string steamId, string itemId, CCSPlayerController player);
        void RemoveItemEffect(string steamId, string itemId, CCSPlayerController player);
        
        // Match Management
        void ResetPlayerItemsForNewMatch(string steamId);
        void ResetAllPlayersForNewMatch();
    }
}
