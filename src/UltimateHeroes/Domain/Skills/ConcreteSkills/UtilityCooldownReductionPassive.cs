using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Utility Cooldown Reduction - Passive Skill: -X% Cooldown auf Utility Skills
    /// </summary>
    public class UtilityCooldownReductionPassive : PassiveSkillBase
    {
        public override string Id => "utility_cooldown_reduction_passive";
        public override string DisplayName => "Utility Cooldown Reduction";
        public override string Description => "-X% Cooldown auf Utility Skills";
        public override int PowerWeight => 15;
        public override List<SkillTag> Tags => new() { SkillTag.Utility };
        public override int MaxLevel => 5;
        
        private const float BaseCooldownReduction = 0.10f; // 10%
        
        /// <summary>
        /// Gets the cooldown reduction multiplier for utility skills
        /// </summary>
        public float GetCooldownReduction()
        {
            return BaseCooldownReduction + (CurrentLevel * 0.05f); // 10% - 30%
        }
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Passive is active, no spawn action needed
            // Cooldown reduction is applied in SkillService when activating skills
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // No action on hurt
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // No action on kill
        }
    }
}
