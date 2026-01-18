using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Time Dilation - Ultimate Skill: Gegner verlangsamt, Team normal
    /// </summary>
    public class TimeDilation : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "time_dilation";
        public override string DisplayName => "Time Dilation";
        public override string Description => "Gegner verlangsamt, Team normal";
        public override int PowerWeight => 60;
        public override List<SkillTag> Tags => new() { SkillTag.CrowdControl, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 120f;
        
        private const float BaseDuration = 8f;
        private const float BaseSlowMultiplier = 0.5f; // 50% speed
        
        // EffectManager wird über SkillService gesetzt
        public static EffectManager? EffectManager { get; set; }
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null) return;
            
            // Calculate duration and slow based on level
            var duration = BaseDuration + (CurrentLevel * 2f);
            var slowMultiplier = BaseSlowMultiplier - (CurrentLevel * 0.05f); // 50% - 40% speed
            slowMultiplier = System.Math.Max(slowMultiplier, 0.3f); // Cap at 30% speed
            
            // Spawn time dilation particle
            GameHelpers.SpawnParticle(pawn.AbsOrigin, "particles/ui/ui_electric_exp_glow.vpcf", duration);
            
            // Apply slow to all enemies
            foreach (var target in Utilities.GetPlayers())
            {
                if (target == player) continue;
                if (!target.IsValid || target.AuthorizedSteamID == null) continue;
                
                // Apply Time Dilation Effect
                if (EffectManager != null)
                {
                    var targetSteamId = target.AuthorizedSteamID.SteamId64.ToString();
                    var effect = new TimeDilationEffect
                    {
                        Duration = duration,
                        SlowMultiplier = slowMultiplier
                    };
                    EffectManager.ApplyEffect(targetSteamId, effect);
                }
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Time Dilation]{ChatColors.Default} Ultimate! Enemies slowed for {duration:F1}s!");
        }
    }
}
