using System;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Infrastructure.Effects;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Shield Effect - Temporärer Schild (Damage Reduction)
    /// </summary>
    public class ShieldEffect : IEffect
    {
        public string Id => "shield";
        public string DisplayName => "Energy Shield";
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        public float DamageReduction { get; set; } = 0.5f; // 50% damage reduction
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Visual indicator (could add particle effect)
            player.PrintToChat($" {ChatColors.Blue}[Shield]{ChatColors.Default} Energy Shield activated! {DamageReduction * 100:F0}% damage reduction!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Shield is passive, no tick needed
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            player.PrintToChat($" {ChatColors.Blue}[Shield]{ChatColors.Default} Energy Shield expired!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
