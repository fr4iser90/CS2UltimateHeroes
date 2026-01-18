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
    /// Fortified Plating - Passive Skill: +X% Damage Reduction unter 50% HP
    /// </summary>
    public class FortifiedPlatingPassive : PassiveSkillBase
    {
        public override string Id => "fortified_plating_passive";
        public override string DisplayName => "Fortified Plating";
        public override string Description => "+X% Damage Reduction unter 50% HP";
        public override int PowerWeight => 15;
        public override List<SkillTag> Tags => new() { SkillTag.Defense };
        public override int MaxLevel => 5;
        
        private const float BaseDamageReduction = 0.15f; // 15%
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Passive is active, no spawn action needed
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            // Note: Health is int, not int? - use directly
            var currentHealth = pawn.Health;
            if (currentHealth <= 0) return;
            var maxHealth = pawn.MaxHealth;
            var healthPercent = (float)currentHealth / maxHealth;
            
            // Only apply if below 50% HP
            if (healthPercent < 0.5f)
            {
                var damageReduction = BaseDamageReduction + (CurrentLevel * 0.05f); // 15% - 35%
                var reducedDamage = (int)(damage * (1f - damageReduction));
                
                // Heal back the reduced damage
                if (reducedDamage < damage)
                {
                    var healAmount = damage - reducedDamage;
                    GameHelper.HealPlayer(player, healAmount);
                    
                    player.PrintToChat($" {ChatColors.Blue}[Fortified Plating]{ChatColors.Default} Reduced {healAmount} damage!");
                }
            }
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // No action on kill
        }
    }
}
