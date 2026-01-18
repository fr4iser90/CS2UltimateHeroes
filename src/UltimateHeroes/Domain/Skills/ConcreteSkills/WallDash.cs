using System.Collections.Generic;
using UltimateHeroes.Infrastructure.Helpers;
using CounterStrikeSharp.API;
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
    /// Wall Dash - Active Skill: Dash entlang von Wänden
    /// </summary>
    public class WallDash : ActiveSkillBase
    {
        public override string Id => "wall_dash";
        public override string DisplayName => "Wall Dash";
        public override string Description => "Dash entlang von Wänden";
        public override int PowerWeight => 20;
        public override List<SkillTag> Tags => new() { SkillTag.Mobility };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 8f;
        
        private const float BaseDashDistance = 400f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate dash distance based on level
            var dashDistance = BaseDashDistance + (CurrentLevel * 100);
            
            // Calculate dash direction (forward + slight upward)
            var dashPos = GameHelper.CalculatePositionInFront(player, dashDistance, 30);
            
            if (dashPos == Vector.Zero) return;
            
            // Spawn dash particle
            GameHelper.SpawnParticle(pawn.AbsOrigin, "particles/ui/ui_electric_exp_glow.vpcf", 1f);
            
            // Teleport player (simplified wall dash - just dash forward)
            GameHelper.TeleportPlayer(player, dashPos);
            
            // Spawn particle at destination
            Server.NextFrame(() =>
            {
                if (player != null && player.IsValid && player.PlayerPawn.Value?.AbsOrigin != null)
                {
                    var newPos = player.PlayerPawn.Value.AbsOrigin;
                    GameHelper.SpawnParticle(newPos, "particles/ui/ui_electric_exp_glow.vpcf", 1f);
                }
            });
            
            player.PrintToChat($" {ChatColors.LightBlue}[Wall Dash]{ChatColors.Default} Dashed {dashDistance:F0} units!");
        }
    }
}
