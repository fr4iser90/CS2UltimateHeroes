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
        
        // BuffService wird über Helper gesetzt
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Calculate duration and fire rate based on level
            var duration = BaseDuration + (CurrentLevel * 3f);
            var fireRateMultiplier = BaseFireRateMultiplier + (CurrentLevel * 0.3f);
            
            // Create Bullet Storm Buffs in Skill (not in Service)
            var buffService = Infrastructure.Helpers.BuffServiceHelper.GetBuffService();
            if (buffService != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                
                // Create Infinite Ammo Buff
                var infiniteAmmoBuff = new Domain.Buffs.Buff
                {
                    Id = "bullet_storm_infinite_ammo", // Fixed ID so it refreshes
                    DisplayName = "Bullet Storm - Infinite Ammo",
                    Type = Domain.Buffs.BuffType.InfiniteAmmo,
                    Duration = duration,
                    StackingType = Domain.Buffs.BuffStackingType.Refresh
                };
                buffService.ApplyBuff(steamId, infiniteAmmoBuff);
                
                // Create Fire Rate Boost Buff
                var fireRateBuff = new Domain.Buffs.Buff
                {
                    Id = "bullet_storm_fire_rate", // Fixed ID so it refreshes
                    DisplayName = "Bullet Storm - Fire Rate",
                    Type = Domain.Buffs.BuffType.FireRateBoost,
                    Duration = duration,
                    StackingType = Domain.Buffs.BuffStackingType.Refresh,
                    Parameters = new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "multiplier", fireRateMultiplier - 1f } // e.g., 2.0x = +100% = 1.0 multiplier
                    }
                };
                buffService.ApplyBuff(steamId, fireRateBuff);
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Bullet Storm]{ChatColors.Default} Ultimate! {fireRateMultiplier:F1}x fire rate + infinite ammo for {duration:F1}s!");
        }
    }
}
