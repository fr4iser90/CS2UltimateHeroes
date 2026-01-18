using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Grenade Toss - Active Skill: Verzögerte Explosion
    /// </summary>
    public class GrenadeToss : ActiveSkillBase
    {
        public override string Id => "grenade_toss";
        public override string DisplayName => "Grenade Toss";
        public override string Description => "Wirft eine Granate mit verzögerter Explosion";
        public override int PowerWeight => 20;
        public override List<SkillTag> Tags => new() { SkillTag.Damage, SkillTag.Area };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 10f;
        
        private const int BaseDamage = 35;
        private const int BaseRadius = 200;
        private const float BaseThrowDistance = 300f;
        private const float ExplosionDelay = 2f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate stats based on level
            var damage = BaseDamage + (CurrentLevel * 7);
            var radius = BaseRadius + (CurrentLevel * 25);
            var throwDistance = BaseThrowDistance + (CurrentLevel * 50);
            
            // Calculate throw position (in front of player, with arc)
            var throwPos = GameHelpers.CalculatePositionInFront(player, throwDistance, 50);
            
            if (throwPos == Vector.Zero) return;
            
            // Spawn grenade indicator particle
            GameHelpers.SpawnParticle(throwPos, "particles/weapons_fx/explosion_fireball.vpcf", ExplosionDelay);
            
            player.PrintToChat($" {ChatColors.Orange}[Grenade Toss]{ChatColors.Default} Grenade thrown! Explodes in {ExplosionDelay}s!");
            
            // Schedule explosion
            var playerSteamId = player.AuthorizedSteamID?.SteamId64.ToString();
            var finalDamage = damage;
            var finalRadius = radius;
            var finalThrowPos = throwPos;
            var finalPlayer = player;
            
            Server.NextFrame(() =>
            {
                // Note: AddTimer is available in CounterStrikeSharp plugin base class
                // For now, use Server.NextFrame with delay simulation
                var frames = (int)(ExplosionDelay * 64); // ~64 frames per second
                var currentFrame = 0;
                
                Action? delayedExplosion = null;
                delayedExplosion = () =>
                {
                    currentFrame++;
                    if (currentFrame < frames)
                    {
                        Server.NextFrame(delayedExplosion);
                        return;
                    }
                    
                    if (finalPlayer == null || !finalPlayer.IsValid) return;
                    
                    // Spawn explosion particles
                    GameHelpers.SpawnParticle(finalThrowPos, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 2f);
                    GameHelpers.SpawnParticle(finalThrowPos, "particles/weapons_fx/explosion_fireball.vpcf", 2f);
                    
                    // Apply damage to all players in radius
                    var playersInRadius = GameHelpers.GetPlayersInRadius(finalThrowPos, finalRadius);
                    
                    float totalDamageDealt = 0f;
                    
                    foreach (var target in playersInRadius)
                    {
                        if (target == finalPlayer) continue;
                        if (!target.IsValid || target.PlayerPawn.Value == null) continue;
                        
                        GameHelpers.DamagePlayer(target, finalDamage, finalPlayer);
                        totalDamageDealt += finalDamage;
                        
                        target.PrintToChat($" {ChatColors.Orange}[Grenade Toss]{ChatColors.Default} Exploded! Took {finalDamage} damage!");
                    }
                    
                    // Track damage for mastery
                    if (playerSteamId != null && totalDamageDealt > 0)
                    {
                        SkillServiceHelper.TrackSkillDamage(playerSteamId, Id, totalDamageDealt);
                    }
                };
                
                Server.NextFrame(delayedExplosion);
            });
        }
    }
}
