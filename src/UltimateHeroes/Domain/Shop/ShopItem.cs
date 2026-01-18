using System;
using System.Collections.Generic;

namespace UltimateHeroes.Domain.Shop
{
    /// <summary>
    /// Repräsentiert ein temporäres Match-Item aus dem Shop
    /// </summary>
    public class ShopItem
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Cost { get; set; } // Geld/XP-Kosten
        public ItemEffect Effect { get; set; } = new();
        public ItemCategory Category { get; set; } = ItemCategory.Utility;
    }
    
    /// <summary>
    /// Effekt eines Shop Items
    /// </summary>
    public class ItemEffect
    {
        public ItemEffectType Type { get; set; }
        public Dictionary<string, float> Parameters { get; set; } = new(); // z.B. {"amount": 50, "duration": 60}
    }
    
    public enum ItemEffectType
    {
        ArmorBoost,      // +X Armor
        HealthBoost,      // +X HP
        SpeedBoost,       // +X% Movement Speed
        DamageBoost,      // +X% Damage
        CooldownReduction, // -X% Cooldown
        GrenadePack,      // +X Grenades
        AmmoPack,         // +X Ammo
        Regeneration      // Regenerate X HP/s
    }
    
    public enum ItemCategory
    {
        Offensive,    // Damage, Ammo
        Defensive,    // Armor, Health
        Utility,      // Speed, Cooldown
        Consumable    // Grenades, Ammo Packs
    }
}
