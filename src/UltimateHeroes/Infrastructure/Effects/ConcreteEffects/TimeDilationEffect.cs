using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Time Dilation Effect - Gegner verlangsamt, Team normal
    /// </summary>
    public class TimeDilationEffect : IEffect
    {
        public string Id => "time_dilation";
        public string DisplayName => "Time Dilation";
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        public float SlowMultiplier { get; set; } = 0.5f; // 50% speed
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var buffService = BuffServiceHelper.GetBuffService();
            
            // Create Speed Reduction Buff in Effect (not in Service)
            if (buffService != null)
            {
                var speedReduction = 1f - SlowMultiplier; // e.g., 0.5 speed = -0.5 multiplier
                var speedReductionBuff = new Domain.Buffs.Buff
                {
                    Id = "time_dilation_speed_reduction", // Fixed ID so it refreshes
                    DisplayName = "Time Dilation - Slowed",
                    Type = Domain.Buffs.BuffType.SpeedReduction,
                    Duration = Duration,
                    StackingType = Domain.Buffs.BuffStackingType.Refresh,
                    Parameters = new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "multiplier", -speedReduction } // Negative = reduction
                    }
                };
                buffService.ApplyBuff(steamId, speedReductionBuff);
            }
            
            // Apply immediately
            GameHelpers.SetMovementSpeed(player, SlowMultiplier);
            
            player.PrintToChat($" {ChatColors.Red}[Time Dilation]{ChatColors.Default} You are slowed! {SlowMultiplier * 100:F0}% speed!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep movement speed reduced (handled by BuffService)
            if (player != null && player.IsValid)
            {
                GameHelpers.SetMovementSpeed(player, SlowMultiplier);
            }
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var buffService = BuffServiceHelper.GetBuffService();
            
            // Remove speed reduction buff
            buffService?.RemoveBuff(steamId, "time_dilation_speed_reduction");
            
            // Restore normal movement
            GameHelpers.SetMovementSpeed(player, 1.0f);
            
            player.PrintToChat($" {ChatColors.Green}[Time Dilation]{ChatColors.Default} Time dilation expired!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
