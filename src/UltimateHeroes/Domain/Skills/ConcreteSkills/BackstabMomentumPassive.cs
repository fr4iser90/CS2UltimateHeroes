using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Backstab Momentum - Passive Skill: Backstab reduziert Cooldowns
    /// </summary>
    public class BackstabMomentumPassive : PassiveSkillBase
    {
        public override string Id => "backstab_momentum_passive";
        public override string DisplayName => "Backstab Momentum";
        public override string Description => "Backstab reduziert Cooldowns";
        public override int PowerWeight => 15;
        public override List<SkillTag> Tags => new() { SkillTag.Stealth };
        public override int MaxLevel => 5;
        
        private const float BaseCooldownReduction = 0.25f; // 25% cooldown reduction
        
        /// <summary>
        /// Gets the cooldown reduction multiplier for backstab kills
        /// </summary>
        public float GetCooldownReduction()
        {
            return BaseCooldownReduction + (CurrentLevel * 0.05f); // 25% - 45%
        }
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Passive is active, no spawn action needed
            // Cooldown reduction is applied in SkillService when backstab is detected
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // No action on hurt
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // Backstab detection would need to be implemented in the kill handler
            // For now, this is a placeholder
            if (player != null && player.IsValid)
            {
                player.PrintToChat($" {ChatColors.Purple}[Backstab Momentum]{ChatColors.Default} Cooldowns reduced!");
            }
        }
    }
}
