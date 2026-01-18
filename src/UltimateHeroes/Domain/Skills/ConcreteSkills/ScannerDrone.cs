using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Scanner Drone - Active Skill: Revealt Gegner im Radius
    /// </summary>
    public class ScannerDrone : ActiveSkillBase
    {
        public override string Id => "scanner_drone";
        public override string DisplayName => "Scanner Drone";
        public override string Description => "Revealt Gegner im Radius";
        public override int PowerWeight => 20;
        public override List<SkillTag> Tags => new() { SkillTag.Utility, SkillTag.Area };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 15f;
        
        private const float BaseRadius = 500f;
        private const float BaseDuration = 8f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null) return;
            
            // Calculate stats based on level
            var radius = BaseRadius + (CurrentLevel * 100);
            var duration = BaseDuration + (CurrentLevel * 2f);
            
            // Spawn drone particle
            GameHelpers.SpawnParticle(pawn.AbsOrigin, "particles/ui/ui_electric_exp_glow.vpcf", duration);
            
            // Find all enemies in radius
            var playersInRadius = GameHelpers.GetPlayersInRadius(pawn.AbsOrigin, radius);
            
            int revealedCount = 0;
            
            foreach (var target in playersInRadius)
            {
                if (target == player) continue;
                if (!target.IsValid) continue;
                
                // Reveal enemy (make visible, remove stealth)
                // Simplified: Just notify player
                revealedCount++;
                
                // Make player visible (remove invisibility if active)
                GameHelpers.MakePlayerInvisible(target, false);
                
                // Notify both players
                player.PrintToChat($" {ChatColors.Green}[Scanner Drone]{ChatColors.Default} Revealed {target.PlayerName}!");
                target.PrintToChat($" {ChatColors.Red}[Scanner Drone]{ChatColors.Default} You have been revealed!");
            }
            
            player.PrintToChat($" {ChatColors.Green}[Scanner Drone]{ChatColors.Default} Revealed {revealedCount} enemies! Duration: {duration:F1}s");
            
            // TODO: Implement persistent reveal effect for duration
        }
    }
}
