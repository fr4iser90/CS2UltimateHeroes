using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// EMP Pulse - Active Skill: Disable Skills/Sentries/Ultimates für X Sekunden
    /// </summary>
    public class EMPPulse : ActiveSkillBase
    {
        public override string Id => "emp_pulse";
        public override string DisplayName => "EMP Pulse";
        public override string Description => "Disable Skills/Sentries/Ultimates für X Sekunden";
        public override int PowerWeight => 35;
        public override List<SkillTag> Tags => new() { SkillTag.Utility, SkillTag.CrowdControl };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 25f;
        
        private const float BaseRadius = 400f;
        private const float BaseDuration = 4f;
        
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
            
            // Spawn EMP particle
            GameHelpers.SpawnParticle(pawn.AbsOrigin, "particles/ui/ui_electric_exp_glow.vpcf", 2f);
            
            // Find all enemies in radius
            var playersInRadius = GameHelpers.GetPlayersInRadius(pawn.AbsOrigin, radius);
            
            int disabledCount = 0;
            
            foreach (var target in playersInRadius)
            {
                if (target == player) continue;
                if (!target.IsValid || target.AuthorizedSteamID == null) continue;
                
                // Apply EMP Effect (disables skills - simplified as stun-like effect)
                if (EffectManager != null)
                {
                    var targetSteamId = target.AuthorizedSteamID.SteamId64.ToString();
                    // Use StunEffect as placeholder for EMP (disables movement/actions)
                    var effect = new StunEffect
                    {
                        Duration = duration
                    };
                    EffectManager.ApplyEffect(targetSteamId, effect);
                    disabledCount++;
                }
                
                target.PrintToChat($" {ChatColors.Red}[EMP Pulse]{ChatColors.Default} Skills disabled for {duration:F1}s!");
            }
            
            player.PrintToChat($" {ChatColors.Red}[EMP Pulse]{ChatColors.Default} Disabled {disabledCount} enemies! Duration: {duration:F1}s");
        }
    }
}
