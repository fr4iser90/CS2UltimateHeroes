using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Stealth - Active Skill: Macht Spieler unsichtbar
    /// </summary>
    public class Stealth : ActiveSkillBase
    {
        public override string Id => "stealth";
        public override string DisplayName => "Stealth";
        public override string Description => "Macht dich unsichtbar für kurze Zeit";
        public override int PowerWeight => 30;
        public override List<SkillTag> Tags => new() { SkillTag.Stealth, SkillTag.Utility };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 15f;
        
        private const float BaseDuration = 5f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // Calculate duration based on level
            var duration = BaseDuration + (CurrentLevel * 1f);
            
            // TODO: Implement actual stealth logic
            // - Make player invisible (alpha/visibility)
            // - Increase movement speed by 20%
            // - Break on damage/shoot
            // - Apply effect with duration
            
            // Placeholder: Just notify player
            player.PrintToChat($" {ChatColors.Purple}[Stealth]{ChatColors.Default} Invisible for {duration:F1}s!");
        }
    }
}
