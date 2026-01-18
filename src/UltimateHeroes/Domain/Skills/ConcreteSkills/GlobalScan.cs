using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Global Scan - Ultimate Skill: Revealt alle Gegner kurzzeitig
    /// </summary>
    public class GlobalScan : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "global_scan";
        public override string DisplayName => "Global Scan";
        public override string Description => "Revealt alle Gegner kurzzeitig";
        public override int PowerWeight => 40;
        public override List<SkillTag> Tags => new() { SkillTag.Utility, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 90f;
        
        private const float BaseDuration = 5f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Calculate duration based on level
            var duration = BaseDuration + (CurrentLevel * 2f);
            
            // Reveal all enemies
            int revealedCount = 0;
            
            foreach (var target in Utilities.GetPlayers())
            {
                if (target == player) continue;
                if (!target.IsValid) continue;
                
                // Make enemy visible (remove invisibility if active)
                GameHelpers.MakePlayerInvisible(target, false);
                
                revealedCount++;
                
                // Notify both players
                player.PrintToChat($" {ChatColors.Green}[Global Scan]{ChatColors.Default} Revealed {target.PlayerName}!");
                target.PrintToChat($" {ChatColors.Red}[Global Scan]{ChatColors.Default} You have been revealed!");
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Global Scan]{ChatColors.Default} Ultimate! Revealed {revealedCount} enemies for {duration:F1}s!");
            
            // TODO: Implement persistent reveal effect for duration
        }
    }
}
