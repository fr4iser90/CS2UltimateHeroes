using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Teleport - Ultimate Skill: Teleportiert zu beliebiger Position (zu Ping-Position)
    /// </summary>
    public class Teleport : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "teleport";
        public override string DisplayName => "Teleport";
        public override string Description => "Teleportiert dich zu einer beliebigen Position auf der Map";
        public override int PowerWeight => 40;
        public override List<SkillTag> Tags => new() { SkillTag.Mobility, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 60f;
        
        private const float MaxRange = 2000f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate destination (far in front of player, like a ping)
            var destination = GameHelpers.CalculatePositionInFront(player, MaxRange, 0);
            
            if (destination == Vector.Zero) return;
            
            // Spawn particles at origin
            var originParticlePos = new Vector(pawn.AbsOrigin.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + 20);
            GameHelpers.SpawnParticle(originParticlePos, "particles/ui/ui_electric_exp_glow.vpcf", 2f);
            GameHelpers.SpawnParticle(pawn.AbsOrigin, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 2f);
            
            // Teleport player
            GameHelpers.TeleportPlayer(player, destination);
            
            // Spawn particles at destination
            Server.NextFrame(() =>
            {
                if (player != null && player.IsValid && player.PlayerPawn.Value != null)
                {
                    var newPos = player.PlayerPawn.Value.AbsOrigin;
                    if (newPos != null)
                    {
                        var destParticlePos = new Vector(newPos.X, newPos.Y, newPos.Z + 20);
                        GameHelpers.SpawnParticle(destParticlePos, "particles/ui/ui_electric_exp_glow.vpcf", 2f);
                        GameHelpers.SpawnParticle(newPos, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 2f);
                    }
                }
            });
            
            player.PrintToChat($" {ChatColors.Gold}[Teleport]{ChatColors.Default} Ultimate! Teleported!");
        }
    }
}
