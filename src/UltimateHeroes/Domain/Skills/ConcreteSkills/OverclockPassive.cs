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
    /// Overclock - Passive Skill: Skills werden stärker, kosten aber HP
    /// </summary>
    public class OverclockPassive : PassiveSkillBase
    {
        public override string Id => "overclock_passive";
        public override string DisplayName => "Overclock";
        public override string Description => "Skills werden stärker, kosten aber HP";
        public override int PowerWeight => 20;
        public override List<SkillTag> Tags => new() { SkillTag.Utility };
        public override int MaxLevel => 5;
        
        private const float BasePowerBonus = 0.20f; // 20% stronger
        private const int BaseHpCost = 5;
        
        /// <summary>
        /// Gets the power bonus multiplier for skills
        /// </summary>
        public float GetPowerBonus()
        {
            return BasePowerBonus + (CurrentLevel * 0.05f); // 20% - 40%
        }
        
        /// <summary>
        /// Gets the HP cost for using skills
        /// </summary>
        public int GetHpCost()
        {
            return BaseHpCost + (CurrentLevel * 2); // 5 - 15 HP
        }
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Passive is active, no spawn action needed
            // HP cost is applied in SkillService when activating skills
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
