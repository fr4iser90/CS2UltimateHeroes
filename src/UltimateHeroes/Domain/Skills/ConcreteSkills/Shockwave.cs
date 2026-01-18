using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Shockwave - Active Skill: Kegelförmige Druckwelle, knockt Gegner zurück
    /// </summary>
    public class Shockwave : ActiveSkillBase
    {
        public override string Id => "shockwave";
        public override string DisplayName => "Shockwave";
        public override string Description => "Kegelförmige Druckwelle, knockt Gegner zurück";
        public override int PowerWeight => 30;
        public override List<SkillTag> Tags => new() { SkillTag.Damage, SkillTag.CrowdControl, SkillTag.Area };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 12f;
        
        private const int BaseDamage = 25;
        private const float BaseRange = 400f;
        private const float BaseConeAngle = 45f; // degrees
        private const float BaseKnockbackForce = 300f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate stats based on level
            var damage = BaseDamage + (CurrentLevel * 5);
            var range = BaseRange + (CurrentLevel * 50);
            var knockbackForce = BaseKnockbackForce + (CurrentLevel * 50);
            
            var origin = pawn.AbsOrigin;
            var angles = pawn.EyeAngles;
            
            // Convert angles to radians
            var pitch = angles.X * (System.Math.PI / 180.0);
            var yaw = angles.Y * (System.Math.PI / 180.0);
            
            // Calculate forward direction
            var forwardX = System.Math.Cos(pitch) * System.Math.Cos(yaw);
            var forwardY = System.Math.Cos(pitch) * System.Math.Sin(yaw);
            var forwardZ = -System.Math.Sin(pitch);
            
            var forward = new Vector((float)forwardX, (float)forwardY, (float)forwardZ);
            
            // Spawn shockwave particle
            GameHelpersHelper.SpawnParticle(origin, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 2f);
            
            // Find all players in cone
            var playersInCone = GetPlayersInCone(origin, forward, range, BaseConeAngle);
            
            float totalDamageDealt = 0f;
            
            foreach (var target in playersInCone)
            {
                if (target == player) continue;
                if (!target.IsValid || target.PlayerPawn.Value == null) continue;
                
                var targetPawn = target.PlayerPawn.Value;
                if (targetPawn.AbsOrigin == null) continue;
                
                // Calculate knockback direction
                var toTarget = new Vector(
                    targetPawn.AbsOrigin.X - origin.X,
                    targetPawn.AbsOrigin.Y - origin.Y,
                    targetPawn.AbsOrigin.Z - origin.Z
                );
                var distance = Vector.Length(toTarget);
                if (distance < 0.1f) continue;
                
                var direction = new Vector(
                    toTarget.X / distance,
                    toTarget.Y / distance,
                    toTarget.Z / distance
                );
                
                // Apply knockback (simplified - push player away)
                var knockbackPos = new Vector(
                    targetPawn.AbsOrigin.X + direction.X * knockbackForce * 0.1f,
                    targetPawn.AbsOrigin.Y + direction.Y * knockbackForce * 0.1f,
                    targetPawn.AbsOrigin.Z + direction.Z * knockbackForce * 0.1f
                );
                
                GameHelpersHelper.TeleportPlayer(target, knockbackPos);
                
                // Apply damage
                GameHelpersHelper.DamagePlayer(target, damage, player);
                totalDamageDealt += damage;
                
                target.PrintToChat($" {ChatColors.Orange}[Shockwave]{ChatColors.Default} Knocked back! Took {damage} damage!");
            }
            
            // Track damage for mastery
            if (player.AuthorizedSteamID != null && totalDamageDealt > 0)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                SkillServiceHelper.TrackSkillDamage(steamId, Id, totalDamageDealt);
            }
            
            player.PrintToChat($" {ChatColors.Orange}[Shockwave]{ChatColors.Default} Shockwave! Damage: {damage}, Range: {range:F0}");
        }
        
        private List<CCSPlayerController> GetPlayersInCone(Vector origin, Vector forward, float range, float coneAngle)
        {
            var players = new List<CCSPlayerController>();
            var cosAngle = System.Math.Cos(coneAngle * System.Math.PI / 180.0);
            
            foreach (var target in Utilities.GetPlayers())
            {
                if (target == null || !target.IsValid || target.PlayerPawn.Value == null) continue;
                
                var targetPawn = target.PlayerPawn.Value;
                if (targetPawn.AbsOrigin == null) continue;
                
                var toTarget = new Vector(
                    targetPawn.AbsOrigin.X - origin.X,
                    targetPawn.AbsOrigin.Y - origin.Y,
                    targetPawn.AbsOrigin.Z - origin.Z
                );
                
                var distance = Vector.Length(toTarget);
                if (distance > range || distance < 0.1f) continue;
                
                // Normalize direction
                var direction = new Vector(
                    toTarget.X / distance,
                    toTarget.Y / distance,
                    toTarget.Z / distance
                );
                
                // Check if in cone (dot product with forward)
                var dot = direction.X * forward.X + direction.Y * forward.Y + direction.Z * forward.Z;
                if (dot >= cosAngle)
                {
                    players.Add(target);
                }
            }
            
            return players;
        }
    }
}
