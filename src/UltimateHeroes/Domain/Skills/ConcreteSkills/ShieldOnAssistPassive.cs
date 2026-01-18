using System.Collections.Generic;
using UltimateHeroes.Infrastructure.Helpers;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Infrastructure.Helpers;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;
using UltimateHeroes.Infrastructure.Helpers;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Shield on Assist - Passive Skill: Schild bei Assist (nicht Kill)
    /// </summary>
    public class ShieldOnAssistPassive : PassiveSkillBase
    {
        public override string Id => "shield_on_assist_passive";
        public override string DisplayName => "Shield on Assist";
        public override string Description => "Gibt Schild bei Assist (nicht Kill)";
        public override int PowerWeight => 10;
        public override List<SkillTag> Tags => new() { SkillTag.Defense, SkillTag.Support };
        public override int MaxLevel => 5;
        
        private const int BaseShieldAmount = 10;
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Passive is active, no spawn action needed
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // No action on hurt
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // This is called when player gets an assist (not a kill)
            // The event handler calls this for assists
            if (player == null || !player.IsValid) return;
            
            // Calculate shield amount based on level
            var shieldAmount = BaseShieldAmount + (CurrentLevel * 5); // 10-30 shield
            
            // Apply shield via BuffService
            var buffService = Infrastructure.Helpers.BuffServiceHelper.GetBuffService();
            if (buffService != null && player.AuthorizedSteamID != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var damageReduction = 0.1f + (CurrentLevel * 0.02f); // 10% - 18% damage reduction
                buffService.ApplyShield(steamId, damageReduction, 10f); // 10 second shield
            }
            
            // Also add armor directly
            GameHelper.AddArmor(player, shieldAmount);
            
            player.PrintToChat($" {ChatColors.Blue}[Shield on Assist]{ChatColors.Default} +{shieldAmount} armor + shield!");
        }
    }
}
