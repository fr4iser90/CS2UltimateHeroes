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
    /// Grapple Hook - Active Skill: Zieh dich an Oberflächen oder Gegner heran
    /// </summary>
    public class GrappleHook : ActiveSkillBase
    {
        public override string Id => "grapple_hook";
        public override string DisplayName => "Grapple Hook";
        public override string Description => "Zieh dich an Oberflächen oder Gegner heran";
        public override int PowerWeight => 25;
        public override List<SkillTag> Tags => new() { SkillTag.Mobility, SkillTag.Utility };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 12f;
        
        private const float BaseRange = 600f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate range based on level
            var range = BaseRange + (CurrentLevel * 100);
            
            // Calculate hook destination (in front of player, like a ping)
            var hookPos = GameHelper.CalculatePositionInFront(player, range, 0);
            
            if (hookPos == Vector.Zero) return;
            
            // Spawn hook particle trail
            GameHelper.SpawnParticle(pawn.AbsOrigin, "particles/ui/ui_electric_exp_glow.vpcf", 1f);
            
            // Teleport player to hook position (simplified - instant grapple)
            GameHelper.TeleportPlayer(player, hookPos);
            
            // Spawn particle at destination
            Server.NextFrame(() =>
            {
                if (player != null && player.IsValid && player.PlayerPawn.Value?.AbsOrigin != null)
                {
                    var newPos = player.PlayerPawn.Value.AbsOrigin;
                    GameHelper.SpawnParticle(newPos, "particles/ui/ui_electric_exp_glow.vpcf", 1f);
                }
            });
            
            player.PrintToChat($" {ChatColors.Cyan}[Grapple Hook]{ChatColors.Default} Grappled {range:F0} units!");
        }
    }
}
