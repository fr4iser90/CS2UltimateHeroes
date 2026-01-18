using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Meteor Strike - Ultimate Skill: Verzögerter Map-Impact Einschlag
    /// </summary>
    public class MeteorStrike : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "meteor_strike";
        public override string DisplayName => "Meteor Strike";
        public override string Description => "Verzögerter Map-Impact Einschlag";
        public override int PowerWeight => 50;
        public override List<SkillTag> Tags => new() { SkillTag.Damage, SkillTag.Area, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 90f;
        
        private const int BaseDamage = 100;
        private const int BaseRadius = 400;
        private const float BaseRange = 1500f;
        private const float ImpactDelay = 3f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate stats based on level
            var damage = BaseDamage + (CurrentLevel * 30);
            var radius = BaseRadius + (CurrentLevel * 50);
            var range = BaseRange + (CurrentLevel * 200);
            
            // Calculate impact position (far in front of player, like a ping)
            var impactPos = GameHelpers.CalculatePositionInFront(player, range, 0);
            
            if (impactPos == Vector.Zero) return;
            
            // Spawn warning particle
            GameHelpers.SpawnParticle(impactPos, "particles/weapons_fx/explosion_fireball.vpcf", ImpactDelay);
            
            // Notify all players
            foreach (var p in Utilities.GetPlayers())
            {
                if (p != null && p.IsValid)
                {
                    p.PrintToChat($" {ChatColors.Red}[Meteor Strike]{ChatColors.Default} Meteor incoming in {ImpactDelay}s!");
                }
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Meteor Strike]{ChatColors.Default} Ultimate! Meteor strikes in {ImpactDelay}s!");
            
            // Schedule impact
            var playerSteamId = player.AuthorizedSteamID?.SteamId64.ToString();
            var finalDamage = damage;
            var finalRadius = radius;
            var finalImpactPos = impactPos;
            var finalPlayer = player;
            
            Server.NextFrame(() =>
            {
                var frames = (int)(ImpactDelay * 64);
                var currentFrame = 0;
                
                Action? delayedImpact = null;
                delayedImpact = () =>
                {
                    currentFrame++;
                    if (currentFrame < frames)
                    {
                        Server.NextFrame(delayedImpact);
                        return;
                    }
                    
                    if (finalPlayer == null || !finalPlayer.IsValid) return;
                    
                    // Spawn massive explosion particles
                    GameHelpers.SpawnParticle(finalImpactPos, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 3f);
                    GameHelpers.SpawnParticle(finalImpactPos, "particles/weapons_fx/explosion_fireball.vpcf", 3f);
                    
                    // Apply damage to all players in radius
                    var playersInRadius = GameHelpers.GetPlayersInRadius(finalImpactPos, finalRadius);
                    
                    float totalDamageDealt = 0f;
                    
                    foreach (var target in playersInRadius)
                    {
                        if (target == finalPlayer) continue;
                        if (!target.IsValid || target.PlayerPawn.Value == null) continue;
                        
                        GameHelpers.DamagePlayer(target, finalDamage, finalPlayer);
                        totalDamageDealt += finalDamage;
                        
                        target.PrintToChat($" {ChatColors.Red}[Meteor Strike]{ChatColors.Default} IMPACT! Took {finalDamage} damage!");
                    }
                    
                    // Track damage for mastery
                    if (playerSteamId != null && totalDamageDealt > 0)
                    {
                        SkillServiceHelper.TrackSkillDamage(playerSteamId, Id, totalDamageDealt);
                    }
                };
                
                Server.NextFrame(delayedImpact);
            });
        }
    }
}
