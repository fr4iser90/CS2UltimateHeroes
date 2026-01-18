using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Bullet Storm - Ultimate Skill: Massive Feuerrate + Infinite Ammo für X Sekunden
    /// </summary>
    public class BulletStorm : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "bullet_storm";
        public override string DisplayName => "Bullet Storm";
        public override string Description => "Massive Feuerrate + Infinite Ammo";
        public override int PowerWeight => 45;
        public override List<SkillTag> Tags => new() { SkillTag.Damage, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 120f;
        
        private const float BaseDuration = 10f;
        private const float BaseFireRateMultiplier = 2.0f;
        
        // EffectManager wird über SkillService gesetzt
        public static EffectManager? EffectManager { get; set; }
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Calculate duration and fire rate based on level
            var duration = BaseDuration + (CurrentLevel * 3f);
            var fireRateMultiplier = BaseFireRateMultiplier + (CurrentLevel * 0.3f);
            
            // Apply Bullet Storm Effect
            if (EffectManager != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var effect = new BulletStormEffect
                {
                    Duration = duration,
                    FireRateMultiplier = fireRateMultiplier
                };
                EffectManager.ApplyEffect(steamId, effect);
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Bullet Storm]{ChatColors.Default} Ultimate! {fireRateMultiplier:F1}x fire rate for {duration:F1}s!");
        }
    }
}
