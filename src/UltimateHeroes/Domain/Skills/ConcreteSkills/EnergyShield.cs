using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Energy Shield - Active Skill: Temporärer Schild (HP oder % Damage Reduction)
    /// </summary>
    public class EnergyShield : ActiveSkillBase
    {
        public override string Id => "energy_shield";
        public override string DisplayName => "Energy Shield";
        public override string Description => "Aktiviert einen temporären Schild";
        public override int PowerWeight => 25;
        public override List<SkillTag> Tags => new() { SkillTag.Defense, SkillTag.Utility };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 15f;
        
        private const float BaseDuration = 5f;
        private const float BaseDamageReduction = 0.5f; // 50%
        
        // Buff Definition (wird einmal erstellt, kann wiederverwendet werden)
        private static readonly Domain.Buffs.BuffDefinition ShieldBuffDefinition = new()
        {
            Id = "energy_shield",
            DisplayName = "Energy Shield",
            Type = Domain.Buffs.BuffType.Shield,
            StackingType = Domain.Buffs.BuffStackingType.Refresh
        };
        
        // BuffService wird über Helper gesetzt
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Calculate duration and damage reduction based on level
            var duration = BaseDuration + (CurrentLevel * 1f);
            var damageReduction = BaseDamageReduction + (CurrentLevel * 0.05f); // 50% - 70%
            damageReduction = System.Math.Min(damageReduction, 0.8f); // Cap at 80%
            
            // Create Shield Buff from Definition (generisch)
            var buffService = Infrastructure.Helpers.BuffServiceHelper.GetBuffService();
            if (buffService != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var shieldBuff = ShieldBuffDefinition.CreateBuff(
                    duration,
                    new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "damage_reduction", damageReduction }
                    }
                );
                buffService.ApplyBuff(steamId, shieldBuff);
            }
            
            player.PrintToChat($" {ChatColors.Blue}[Energy Shield]{ChatColors.Default} Shield activated! {damageReduction * 100:F0}% damage reduction for {duration:F1}s!");
        }
    }
}
