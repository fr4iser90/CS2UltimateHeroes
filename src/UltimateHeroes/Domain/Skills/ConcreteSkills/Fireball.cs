using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Fireball - Active Skill: Wirft einen Feuerball
    /// </summary>
    public class Fireball : ActiveSkillBase
    {
        public override string Id => "fireball";
        public override string DisplayName => "Fireball";
        public override string Description => "Wirft einen Feuerball in Blickrichtung";
        public override int PowerWeight => 25;
        public override List<SkillTag> Tags => new() { SkillTag.Damage, SkillTag.Area };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 10f;
        
        private const int BaseDamage = 30;
        private const int BaseRadius = 150;
        private const float ProjectileDistance = 500f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null) return;
            
            // Calculate damage and radius based on level
            var damage = BaseDamage + (CurrentLevel * 5);
            var radius = BaseRadius + (CurrentLevel * 20);
            
            // Calculate explosion position (in front of player)
            var explosionPos = GameHelpers.CalculatePositionInFront(player, ProjectileDistance, 20);
            
            if (explosionPos == Vector.Zero) return;
            
            // Spawn explosion particle
            GameHelpers.SpawnParticle(explosionPos, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 2f);
            GameHelpers.SpawnParticle(explosionPos, "particles/weapons_fx/explosion_fireball.vpcf", 2f);
            
            // Get talent modifiers for damage bonus
            // Note: Talent modifiers are applied via PlayerService, but we need to get them from the player state
            // For now, we'll apply damage without modifiers (modifiers are applied on spawn)
            Dictionary<string, float>? talentModifiers = null;
            
            // Track total damage for mastery
            float totalDamageDealt = 0f;
            
            // Apply damage to all players in radius
            var playersInRadius = GameHelpers.GetPlayersInRadius(explosionPos, radius);
            foreach (var target in playersInRadius)
            {
                if (target == player) continue; // Don't damage self
                if (!target.IsValid || target.PlayerPawn.Value == null) continue;
                
                GameHelpers.DamagePlayer(target, damage, player, talentModifiers);
                totalDamageDealt += damage;
                target.PrintToChat($" {ChatColors.Orange}[Fireball]{ChatColors.Default} You took {damage} damage!");
            }
            
            // Track damage for mastery (if player has SteamID)
            if (player.AuthorizedSteamID != null && totalDamageDealt > 0)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                SkillServiceHelper.TrackSkillDamage(steamId, Id, totalDamageDealt);
            }
            
            player.PrintToChat($" {ChatColors.Orange}[Fireball]{ChatColors.Default} Exploded! Damage: {damage}, Radius: {radius}");
        }
    }
}
