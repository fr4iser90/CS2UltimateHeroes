using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Taunt Effect - CS2-kompatibel: Reduced Damage + Weapon Spread (statt echter Aggro-Zwang)
    /// </summary>
    public class TauntEffect : IEffect
    {
        public string Id => "taunt";
        public string DisplayName => "Taunted";
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        public string TaunterSteamId { get; set; } = string.Empty;
        public float DamageReduction { get; set; } = 0.5f; // 50% damage reduction if not attacking taunter
        public float SpreadMultiplier { get; set; } = 2.0f; // 2x weapon spread
        
        // Buff Definition (wird einmal erstellt, kann wiederverwendet werden)
        private static readonly Domain.Buffs.BuffDefinition TauntBuffDefinition = new()
        {
            Id = "taunt",
            DisplayName = "Taunted",
            Type = Domain.Buffs.BuffType.Taunt,
            StackingType = Domain.Buffs.BuffStackingType.Refresh
        };
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var buffService = BuffServiceHelper.GetBuffService();
            
            // Create Taunt Buff from Definition (generisch)
            if (buffService != null)
            {
                var tauntBuff = TauntBuffDefinition.CreateBuff(
                    Duration,
                    new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "taunter_steamid", float.Parse(TaunterSteamId) }, // Hack: Store as float
                        { "damage_reduction", DamageReduction },
                        { "spread_multiplier", SpreadMultiplier }
                    }
                );
                buffService.ApplyBuff(steamId, tauntBuff);
            }
            
            player.PrintToChat($" {ChatColors.Red}[Taunt]{ChatColors.Default} You are taunted! Attack the taunter or suffer {DamageReduction * 100:F0}% damage reduction + {SpreadMultiplier}x spread!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Taunt effects are handled by BuffService
            // Weapon spread is applied via Weapon Modifier System (TODO)
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var buffService = BuffServiceHelper.GetBuffService();
            
            // Remove taunt buff
            buffService?.RemoveBuff(steamId, "taunt");
            
            player.PrintToChat($" {ChatColors.Green}[Taunt]{ChatColors.Default} Taunt expired!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
