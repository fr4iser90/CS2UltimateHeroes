using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Piercing Shot - Active Skill: Schuss durchdringt X Gegner
    /// </summary>
    public class PiercingShot : ActiveSkillBase
    {
        public override string Id => "piercing_shot";
        public override string DisplayName => "Piercing Shot";
        public override string Description => "Schuss durchdringt X Gegner";
        public override int PowerWeight => 25;
        public override List<SkillTag> Tags => new() { SkillTag.Damage };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 8f;
        
        private const int BaseDamage = 40;
        private const float BaseRange = 1000f;
        private const int BasePierces = 2;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate stats based on level
            var damage = BaseDamage + (CurrentLevel * 8);
            var range = BaseRange + (CurrentLevel * 100);
            var maxPierces = BasePierces + CurrentLevel; // 2-7 pierces
            
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
            
            // Spawn projectile particle
            GameHelper.SpawnParticle(origin, "particles/weapons_fx/explosion_fireball.vpcf", 1f);
            
            // Raycast to find all players in line
            var hitPlayers = GetPlayersInLine(origin, forward, range, maxPierces);
            
            float totalDamageDealt = 0f;
            int pierceCount = 0;
            
            foreach (var target in hitPlayers)
            {
                if (target == player) continue;
                if (!target.IsValid || target.PlayerPawn.Value == null) continue;
                
                // Apply damage
                GameHelper.DamagePlayer(target, damage, player);
                totalDamageDealt += damage;
                pierceCount++;
                
                target.PrintToChat($" {ChatColors.Orange}[Piercing Shot]{ChatColors.Default} Pierced! Took {damage} damage!");
                
                if (pierceCount >= maxPierces) break;
            }
            
            // Track damage for mastery
            if (player.AuthorizedSteamID != null && totalDamageDealt > 0)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                SkillServiceHelper.TrackSkillDamage(steamId, Id, totalDamageDealt);
            }
            
            player.PrintToChat($" {ChatColors.Orange}[Piercing Shot]{ChatColors.Default} Pierced {pierceCount} enemies! Damage: {damage}");
        }
        
        private List<CCSPlayerController> GetPlayersInLine(Vector origin, Vector forward, float range, int maxPierces)
        {
            var players = new List<CCSPlayerController>();
            var hitPositions = new List<Vector>();
            
            // Simple raycast simulation - find players in line
            for (float dist = 50f; dist <= range && players.Count < maxPierces; dist += 50f)
            {
                var checkPos = new Vector(
                    origin.X + forward.X * dist,
                    origin.Y + forward.Y * dist,
                    origin.Z + forward.Z * dist
                );
                
                // Check for players at this distance
                var nearbyPlayers = GameHelper.GetPlayersInRadius(checkPos, 50f);
                
                foreach (var target in nearbyPlayers)
                {
                    if (target == null || !target.IsValid || target.PlayerPawn.Value == null) continue;
                    if (players.Contains(target)) continue;
                    
                    var targetPawn = target.PlayerPawn.Value;
                    if (targetPawn.AbsOrigin == null) continue;
                    
                    // Check if player is roughly in line
                    var toTarget = new Vector(
                        targetPawn.AbsOrigin.X - origin.X,
                        targetPawn.AbsOrigin.Y - origin.Y,
                        targetPawn.AbsOrigin.Z - origin.Z
                    );
                    var distance = (float)System.Math.Sqrt(toTarget.X * toTarget.X + toTarget.Y * toTarget.Y + toTarget.Z * toTarget.Z);
                    if (distance < 0.1f) continue;
                    
                    var direction = new Vector(
                        toTarget.X / distance,
                        toTarget.Y / distance,
                        toTarget.Z / distance
                    );
                    
                    // Check alignment (dot product should be close to 1)
                    var dot = direction.X * forward.X + direction.Y * forward.Y + direction.Z * forward.Z;
                    if (dot > 0.9f) // Roughly in line
                    {
                        players.Add(target);
                        if (players.Count >= maxPierces) break;
                    }
                }
            }
            
            return players;
        }
    }
}
