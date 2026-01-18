using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Domain.Shop;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Shop Management (temporäre Match-Items)
    /// </summary>
    public class ShopService : IShopService
    {
        private readonly Dictionary<string, ShopItem> _shopItems = new();
        private readonly Dictionary<string, PlayerMatchItems> _playerItems = new();
        
        public ShopService()
        {
            InitializeDefaultShopItems();
        }
        
        public void RegisterShopItem(ShopItem item)
        {
            _shopItems[item.Id] = item;
        }
        
        public ShopItem? GetShopItem(string itemId)
        {
            return _shopItems.GetValueOrDefault(itemId);
        }
        
        public List<ShopItem> GetAllShopItems()
        {
            return _shopItems.Values.ToList();
        }
        
        public List<ShopItem> GetShopItemsByCategory(ItemCategory category)
        {
            return _shopItems.Values.Where(i => i.Category == category).ToList();
        }
        
        public PlayerMatchItems GetPlayerItems(string steamId)
        {
            if (!_playerItems.TryGetValue(steamId, out var items))
            {
                items = new PlayerMatchItems { SteamId = steamId };
                _playerItems[steamId] = items;
            }
            
            // Cleanup expired items
            items.CleanupExpiredItems();
            
            return items;
        }
        
        public bool CanAffordItem(string steamId, string itemId)
        {
            var item = GetShopItem(itemId);
            if (item == null) return false;
            
            var playerItems = GetPlayerItems(steamId);
            return playerItems.CurrentMoney >= item.Cost;
        }
        
        public bool PurchaseItem(string steamId, string itemId, CCSPlayerController? player = null)
        {
            var item = GetShopItem(itemId);
            if (item == null) return false;
            
            var playerItems = GetPlayerItems(steamId);
            
            if (!CanAffordItem(steamId, itemId))
            {
                player?.PrintToChat($" {ChatColors.Red}[Shop]{ChatColors.Default} Not enough money! Need ${item.Cost}, have ${playerItems.CurrentMoney}");
                return false;
            }
            
            // Deduct money
            playerItems.CurrentMoney -= item.Cost;
            
            // Add item (or refresh if already active)
            var activeItem = new ActiveItem
            {
                ItemId = itemId,
                PurchasedAt = DateTime.UtcNow,
                DurationSeconds = item.Effect.Parameters.GetValueOrDefault("duration", 0f)
            };
            
            playerItems.ActiveItems[itemId] = activeItem;
            
            // Apply effect
            if (player != null && player.IsValid)
            {
                ApplyItemEffect(steamId, itemId, player);
            }
            
            player?.PrintToChat($" {ChatColors.Green}[Shop]{ChatColors.Default} Purchased: {item.DisplayName} (${item.Cost})");
            
            return true;
        }
        
        public void ApplyItemEffect(string steamId, string itemId, CCSPlayerController player)
        {
            var item = GetShopItem(itemId);
            if (item == null || player == null || !player.IsValid) return;
            
            var playerItems = GetPlayerItems(steamId);
            if (!playerItems.HasActiveItem(itemId)) return;
            
            var playerPawn = player.PlayerPawn.Value;
            if (playerPawn == null) return;
            
            switch (item.Effect.Type)
            {
                case ItemEffectType.ArmorBoost:
                    var armorAmount = (int)item.Effect.Parameters.GetValueOrDefault("amount", 50f);
                    // TODO: Apply armor boost via GameHelpers
                    break;
                    
                case ItemEffectType.HealthBoost:
                    var healthAmount = (int)item.Effect.Parameters.GetValueOrDefault("amount", 20f);
                    GameHelpers.HealPlayer(player, healthAmount);
                    break;
                    
                case ItemEffectType.SpeedBoost:
                    var speedPercent = item.Effect.Parameters.GetValueOrDefault("amount", 0.10f);
                    // TODO: Apply speed boost
                    break;
                    
                case ItemEffectType.DamageBoost:
                    // TODO: Track damage boost for next damage calculation
                    break;
                    
                case ItemEffectType.CooldownReduction:
                    // TODO: Apply cooldown reduction
                    break;
            }
        }
        
        public void RemoveItemEffect(string steamId, string itemId, CCSPlayerController player)
        {
            // TODO: Remove item effects when expired
        }
        
        public void ResetPlayerItemsForNewMatch(string steamId)
        {
            var playerItems = GetPlayerItems(steamId);
            playerItems.ResetForNewMatch();
        }
        
        public void ResetAllPlayersForNewMatch()
        {
            foreach (var kvp in _playerItems)
            {
                kvp.Value.ResetForNewMatch();
            }
        }
        
        private void InitializeDefaultShopItems()
        {
            // Defensive Items
            RegisterShopItem(new ShopItem
            {
                Id = "armor_boost",
                DisplayName = "Armor Boost",
                Description = "+50 Armor for this match",
                Cost = 650,
                Category = ItemCategory.Defensive,
                Effect = new ItemEffect
                {
                    Type = ItemEffectType.ArmorBoost,
                    Parameters = new Dictionary<string, float> { { "amount", 50f } }
                }
            });
            
            RegisterShopItem(new ShopItem
            {
                Id = "health_boost",
                DisplayName = "Health Boost",
                Description = "+20 HP for this match",
                Cost = 400,
                Category = ItemCategory.Defensive,
                Effect = new ItemEffect
                {
                    Type = ItemEffectType.HealthBoost,
                    Parameters = new Dictionary<string, float> { { "amount", 20f } }
                }
            });
            
            // Offensive Items
            RegisterShopItem(new ShopItem
            {
                Id = "damage_boost",
                DisplayName = "Damage Boost",
                Description = "+10% Damage for this match",
                Cost = 800,
                Category = ItemCategory.Offensive,
                Effect = new ItemEffect
                {
                    Type = ItemEffectType.DamageBoost,
                    Parameters = new Dictionary<string, float> { { "amount", 0.10f } }
                }
            });
            
            // Utility Items
            RegisterShopItem(new ShopItem
            {
                Id = "speed_boost",
                DisplayName = "Speed Boost",
                Description = "+10% Movement Speed for 60s",
                Cost = 500,
                Category = ItemCategory.Utility,
                Effect = new ItemEffect
                {
                    Type = ItemEffectType.SpeedBoost,
                    Parameters = new Dictionary<string, float> 
                    { 
                        { "amount", 0.10f },
                        { "duration", 60f }
                    }
                }
            });
            
            RegisterShopItem(new ShopItem
            {
                Id = "cooldown_reduction",
                DisplayName = "Cooldown Reduction",
                Description = "-15% Cooldown for 90s",
                Cost = 600,
                Category = ItemCategory.Utility,
                Effect = new ItemEffect
                {
                    Type = ItemEffectType.CooldownReduction,
                    Parameters = new Dictionary<string, float> 
                    { 
                        { "amount", 0.15f },
                        { "duration", 90f }
                    }
                }
            });
        }
    }
}
