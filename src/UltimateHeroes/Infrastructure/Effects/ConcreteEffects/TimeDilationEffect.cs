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
            if (player == null || !player.IsValid) return;
            
            // Slow down enemy (reduce movement speed)
            GameHelpers.SetMovementSpeed(player, SlowMultiplier);
            
            player.PrintToChat($" {ChatColors.Red}[Time Dilation]{ChatColors.Default} You are slowed! {SlowMultiplier * 100:F0}% speed!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep movement speed reduced
            if (player != null && player.IsValid)
            {
                GameHelpers.SetMovementSpeed(player, SlowMultiplier);
            }
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
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
