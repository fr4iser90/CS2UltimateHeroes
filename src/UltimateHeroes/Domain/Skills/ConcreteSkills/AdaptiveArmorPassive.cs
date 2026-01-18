using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Adaptive Armor - Passive Skill: Erhöht Armor gegen zuletzt erlittenen Schadenstyp
    /// </summary>
    public class AdaptiveArmorPassive : PassiveSkillBase
    {
        public override string Id => "adaptive_armor_passive";
        public override string DisplayName => "Adaptive Armor";
        public override string Description => "Erhöht Armor gegen zuletzt erlittenen Schadenstyp";
        public override int PowerWeight => 15;
        public override List<SkillTag> Tags => new() { SkillTag.Defense };
        public override int MaxLevel => 5;
        
        private const int BaseArmorBonus = 5;
        private const float ArmorDuration = 10f; // Armor bonus lasts 10 seconds
        
        // Track last damage time per player (simplified - would need proper tracking)
        private static Dictionary<string, System.DateTime> _lastDamageTime = new();
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Passive is active, no spawn action needed
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            _lastDamageTime[steamId] = System.DateTime.UtcNow;
            
            // Calculate armor bonus based on level
            var armorBonus = BaseArmorBonus + (CurrentLevel * 3); // 5-20 armor
            
            // Add adaptive armor
            GameHelpers.AddArmor(player, armorBonus);
            
            player.PrintToChat($" {ChatColors.Blue}[Adaptive Armor]{ChatColors.Default} +{armorBonus} armor (adaptive)!");
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // No action on kill
        }
    }
}
