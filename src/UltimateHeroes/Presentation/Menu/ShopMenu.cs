using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Shop;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Shop Menu für temporäre Match-Items
    /// </summary>
    public class ShopMenu
    {
        private readonly IShopService _shopService;
        private readonly IPlayerService _playerService;
        
        public ShopMenu(IShopService shopService, IPlayerService playerService)
        {
            _shopService = shopService;
            _playerService = playerService;
        }
        
        public void ShowMenu(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerItems = _shopService.GetPlayerItems(steamId);
            var allItems = _shopService.GetAllShopItems();
            
            var shopMenu = MenuManager.CreateMenu($"<font color='lightgrey' class='fontSize-m'>Ultimate Heroes - Shop</font>", 10);
            
            // Show current money
            shopMenu.Add($"<font color='green'>💰 Money: ${playerItems.CurrentMoney}</font>", 
                "<font color='grey' class='fontSize-sm'>Purchase items to boost your performance this match</font>", 
                (p, opt) => { });
            
            // Group items by category
            var categories = new[] { ItemCategory.Offensive, ItemCategory.Defensive, ItemCategory.Utility, ItemCategory.Consumable };
            
            foreach (var category in categories)
            {
                var categoryItems = allItems.Where(i => i.Category == category).ToList();
                if (categoryItems.Count == 0) continue;
                
                var categoryName = GetCategoryName(category);
                shopMenu.Add($"<font color='yellow'>━━━ {categoryName} ━━━</font>", null, (p, opt) => { });
                
                foreach (var item in categoryItems)
                {
                    var canAfford = _shopService.CanAffordItem(steamId, item.Id);
                    var isActive = playerItems.HasActiveItem(item.Id);
                    var activeItem = playerItems.ActiveItems.GetValueOrDefault(item.Id);
                    
                    var color = canAfford ? (isActive ? "orange" : "lightblue") : "grey";
                    var status = isActive ? " [ACTIVE]" : "";
                    var display = $"<font color='{color}'>${item.Cost} - {item.DisplayName}{status}</font>";
                    
                    var subDisplay = $"<font color='grey' class='fontSize-sm'>{item.Description}";
                    if (isActive && activeItem != null && activeItem.DurationSeconds > 0)
                    {
                        var remaining = activeItem.RemainingSeconds();
                        subDisplay += $"<br>Time remaining: {remaining:F0}s";
                    }
                    subDisplay += "</font>";
                    
                    var itemId = item.Id;
                    shopMenu.Add(display, subDisplay, (p, opt) =>
                    {
                        if (!canAfford)
                        {
                            p.PrintToChat($" {ChatColors.Red}[Shop]{ChatColors.Default} Not enough money! Need ${item.Cost}, have ${playerItems.CurrentMoney}");
                            return;
                        }
                        
                        if (_shopService.PurchaseItem(steamId, itemId, p))
                        {
                            // Apply item modifiers to player state
                            var playerState = _playerService.GetPlayer(steamId);
                            if (playerState != null)
                            {
                                var item = _shopService.GetShopItem(itemId);
                                if (item != null)
                                {
                                    switch (item.Effect.Type)
                                    {
                                        case ItemEffectType.DamageBoost:
                                            var damagePercent = item.Effect.Parameters.GetValueOrDefault("amount", 0.10f);
                                            playerState.ItemModifiers["damage_boost"] = damagePercent;
                                            break;
                                            
                                        case ItemEffectType.CooldownReduction:
                                            var cooldownReduction = item.Effect.Parameters.GetValueOrDefault("amount", 0.15f);
                                            playerState.ItemModifiers["cooldown_reduction"] = cooldownReduction;
                                            break;
                                    }
                                }
                            }
                            
                            // Refresh menu
                            ShowMenu(p);
                        }
                    });
                }
            }
            
            // Active Items section
            if (playerItems.ActiveItems.Count > 0)
            {
                shopMenu.Add($"<font color='yellow'>━━━ Active Items ━━━</font>", null, (p, opt) => { });
                
                foreach (var kvp in playerItems.ActiveItems)
                {
                    var item = _shopService.GetShopItem(kvp.Key);
                    if (item == null) continue;
                    
                    var activeItem = kvp.Value;
                    var remaining = activeItem.DurationSeconds > 0 ? $"{activeItem.RemainingSeconds():F0}s" : "Match";
                    
                    shopMenu.Add($"<font color='green'>{item.DisplayName}</font>", 
                        $"<font color='grey' class='fontSize-sm'>Time remaining: {remaining}</font>", 
                        (p, opt) => { });
                }
            }
            
            MenuManager.OpenMainMenu(player, shopMenu);
        }
        
        private string GetCategoryName(ItemCategory category)
        {
            return category switch
            {
                ItemCategory.Offensive => "⚔️ Offensive",
                ItemCategory.Defensive => "🛡️ Defensive",
                ItemCategory.Utility => "🔧 Utility",
                ItemCategory.Consumable => "📦 Consumable",
                _ => category.ToString()
            };
        }
    }
}
