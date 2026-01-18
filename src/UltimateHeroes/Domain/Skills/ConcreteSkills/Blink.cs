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
    /// Blink - Active Skill: Teleportiert Spieler in Blickrichtung
    /// </summary>
    public class Blink : ActiveSkillBase
    {
        public override string Id => "blink";
        public override string DisplayName => "Blink";
        public override string Description => "Teleportiert dich in Blickrichtung";
        public override int PowerWeight => 20;
        public override List<SkillTag> Tags => new() { SkillTag.Mobility };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 5f;
        
        private const int BaseRange = 300;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate range based on level
            var range = BaseRange + (CurrentLevel * 50);
            
            // Calculate destination position
            var destination = GameHelper.CalculatePositionInFront(player, range);
            
            if (destination == Vector.Zero) return;
            
            // Spawn particles at origin
            var originParticlePos = new Vector(pawn.AbsOrigin.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + 20);
            GameHelper.SpawnParticle(originParticlePos, "particles/ui/ui_electric_exp_glow.vpcf", 1f);
            GameHelper.SpawnParticle(pawn.AbsOrigin, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 1f);
            
            // Teleport player
            GameHelper.TeleportPlayer(player, destination);
            
            // Spawn particles at destination
            Server.NextFrame(() =>
            {
                if (player != null && player.IsValid && player.PlayerPawn.Value != null)
                {
                    var newPos = player.PlayerPawn.Value.AbsOrigin;
                    if (newPos != null)
                    {
                        var destParticlePos = new Vector(newPos.X, newPos.Y, newPos.Z + 20);
                        GameHelper.SpawnParticle(destParticlePos, "particles/ui/ui_electric_exp_glow.vpcf", 1f);
                        GameHelper.SpawnParticle(newPos, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 1f);
                    }
                }
            });
            
            player.PrintToChat($" {ChatColors.Cyan}[Blink]{ChatColors.Default} Teleported {range} units forward!");
        }
    }
}
