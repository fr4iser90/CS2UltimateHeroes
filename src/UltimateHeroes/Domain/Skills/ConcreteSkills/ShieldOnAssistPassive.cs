using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
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
            // This is called on kill, but we want assists
            // Note: Assists are tracked separately in the event system
            // For now, this passive would need to be triggered from the event handler
            // This is a placeholder - actual implementation would require assist tracking
        }
    }
}
