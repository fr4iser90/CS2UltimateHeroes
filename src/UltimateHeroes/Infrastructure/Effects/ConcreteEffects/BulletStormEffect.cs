using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Bullet Storm Effect - Massive Feuerrate + Infinite Ammo
    /// </summary>
    public class BulletStormEffect : IEffect
    {
        public string Id => "bullet_storm";
        public string DisplayName => "Bullet Storm";
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        public float FireRateMultiplier { get; set; } = 2.0f; // 2x fire rate
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // TODO: Apply fire rate multiplier and infinite ammo
            // This would require modifying weapon properties
            player.PrintToChat($" {ChatColors.Gold}[Bullet Storm]{ChatColors.Default} Ultimate! Infinite ammo + {FireRateMultiplier}x fire rate!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep infinite ammo active
            if (player != null && player.IsValid && player.PlayerPawn.Value != null)
            {
                var pawn = player.PlayerPawn.Value;
                // TODO: Set ammo to max
            }
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            player.PrintToChat($" {ChatColors.Gold}[Bullet Storm]{ChatColors.Default} Ultimate expired!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
