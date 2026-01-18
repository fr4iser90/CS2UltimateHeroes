using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// HealingAura - Passive Skill: Heilt Spieler und Teammates
    /// </summary>
    public class HealingAura : PassiveSkillBase
    {
        public override string Id => "healing_aura";
        public override string DisplayName => "Healing Aura";
        public override string Description => "Heilt dich und Teammates in der Nähe";
        public override int PowerWeight => 15;
        public override List<SkillTag> Tags => new() { SkillTag.Support, SkillTag.Area };
        public override int MaxLevel => 5;
        
        private const float BaseHealPerSecond = 2f;
        private const int BaseRadius = 200;
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Start periodic healing (every 1 second)
            // Note: This should be managed by the plugin's timer system
            // For now, we'll heal on spawn and let the plugin handle periodic calls
            HealInRadius(player);
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // Nothing special on hurt
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // Nothing special on kill
        }
        
        private void HealInRadius(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null) return;
            
            var healPerSecond = BaseHealPerSecond + (CurrentLevel * 0.5f);
            var radius = BaseRadius + (CurrentLevel * 30);
            
            // Find all players in radius (including self and teammates)
            var playersInRadius = GameHelpers.GetPlayersInRadius(pawn.AbsOrigin, radius);
            
            foreach (var target in playersInRadius)
            {
                if (!target.IsValid || target.PlayerPawn.Value == null) continue;
                
                // Heal player
                var healAmount = (int)System.Math.Ceiling(healPerSecond);
                GameHelpers.HealPlayer(target, healAmount);
            }
            
            // Spawn healing particle occasionally
            if (playersInRadius.Count > 0)
            {
                var healParticlePos = new Vector(pawn.AbsOrigin.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + 20);
                GameHelpers.SpawnParticle(healParticlePos, "particles/status_fx/status_effect_heal.vpcf", 1f);
            }
        }
    }
}
