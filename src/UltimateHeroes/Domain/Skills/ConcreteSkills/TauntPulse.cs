using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;
using UltimateHeroes.Application.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Taunt Pulse - Active Skill: Zwingt Gegner im Radius, dich anzugreifen
    /// </summary>
    public class TauntPulse : ActiveSkillBase
    {
        public override string Id => "taunt_pulse";
        public override string DisplayName => "Taunt Pulse";
        public override string Description => "Zwingt Gegner im Radius, dich anzugreifen";
        public override int PowerWeight => 30;
        public override List<SkillTag> Tags => new() { SkillTag.Defense, SkillTag.CrowdControl };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 20f;
        
        private const float BaseRadius = 300f;
        private const float BaseDuration = 3f;
        
        // EffectManager wird über SkillService gesetzt
        public static EffectManager? EffectManager { get; set; }
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null || player.AuthorizedSteamID == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null) return;
            
            // Calculate stats based on level
            var radius = BaseRadius + (CurrentLevel * 50);
            var duration = BaseDuration + (CurrentLevel * 0.5f);
            
            // Spawn pulse particle
            GameHelpers.SpawnParticle(pawn.AbsOrigin, "particles/explosions_fx/explosion_smokegrenade_distort.vpcf", 2f);
            
            // Find all enemies in radius
            var playersInRadius = GameHelpers.GetPlayersInRadius(pawn.AbsOrigin, radius);
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            
            int tauntedCount = 0;
            
            foreach (var target in playersInRadius)
            {
                if (target == player) continue;
                if (!target.IsValid || target.AuthorizedSteamID == null) continue;
                
                // Apply Taunt Effect (via EffectManager, which uses BuffService internally)
                if (EffectManager != null)
                {
                    var targetSteamId = target.AuthorizedSteamID.SteamId64.ToString();
                    var effect = new TauntEffect
                    {
                        Duration = duration,
                        TaunterSteamId = steamId,
                        DamageReduction = 0.5f, // 50% damage reduction if not attacking taunter
                        SpreadMultiplier = 2.0f // 2x weapon spread
                    };
                    EffectManager.ApplyEffect(targetSteamId, effect);
                    tauntedCount++;
                }
                
                target.PrintToChat($" {ChatColors.Red}[Taunt Pulse]{ChatColors.Default} You are taunted! Attack {player.PlayerName}!");
            }
            
            player.PrintToChat($" {ChatColors.Red}[Taunt Pulse]{ChatColors.Default} Taunted {tauntedCount} enemies! Duration: {duration:F1}s");
        }
    }
}
