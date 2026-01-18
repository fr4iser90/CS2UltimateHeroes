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
        
        // Buff Definitions (wird einmal erstellt, kann wiederverwendet werden)
        private static readonly Domain.Buffs.BuffDefinition InfiniteAmmoBuffDefinition = new()
        {
            Id = "bullet_storm_infinite_ammo",
            DisplayName = "Bullet Storm - Infinite Ammo",
            Type = Domain.Buffs.BuffType.InfiniteAmmo,
            StackingType = Domain.Buffs.BuffStackingType.Refresh
        };
        
        private static readonly Domain.Buffs.BuffDefinition FireRateBuffDefinition = new()
        {
            Id = "bullet_storm_fire_rate",
            DisplayName = "Bullet Storm - Fire Rate",
            Type = Domain.Buffs.BuffType.FireRateBoost,
            StackingType = Domain.Buffs.BuffStackingType.Refresh
        };
        
        // BuffService wird über Helper gesetzt
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Calculate duration and fire rate based on level
            var duration = BaseDuration + (CurrentLevel * 3f);
            var fireRateMultiplier = BaseFireRateMultiplier + (CurrentLevel * 0.3f);
            
            // Create Buffs from Definitions (generisch)
            var buffService = Infrastructure.Helpers.BuffServiceHelper.GetBuffService();
            if (buffService != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                
                // Create Infinite Ammo Buff from Definition
                var infiniteAmmoBuff = InfiniteAmmoBuffDefinition.CreateBuff(duration);
                buffService.ApplyBuff(steamId, infiniteAmmoBuff);
                
                // Create Fire Rate Boost Buff from Definition
                var fireRateBuff = FireRateBuffDefinition.CreateBuff(
                    duration,
                    new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "multiplier", fireRateMultiplier - 1f } // e.g., 2.0x = +100% = 1.0 multiplier
                    }
                );
                buffService.ApplyBuff(steamId, fireRateBuff);
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Bullet Storm]{ChatColors.Default} Ultimate! {fireRateMultiplier:F1}x fire rate + infinite ammo for {duration:F1}s!");
        }
    }
}
