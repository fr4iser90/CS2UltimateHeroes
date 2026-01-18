using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Execution Mark Effect - Markierter Gegner nimmt extrem erhöhten Schaden
    /// </summary>
    public class ExecutionMarkEffect : IEffect
    {
        public string Id => "execution_mark";
        public string DisplayName => "Execution Mark";
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        public float DamageMultiplier { get; set; } = 2.0f; // 2x damage
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            player.PrintToChat($" {ChatColors.Red}[Execution Mark]{ChatColors.Default} You are marked! Take {DamageMultiplier}x damage!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Mark is passive, no tick needed
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            player.PrintToChat($" {ChatColors.Green}[Execution Mark]{ChatColors.Default} Mark expired!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
