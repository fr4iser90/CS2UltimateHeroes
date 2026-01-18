using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Stun Effect - Bewegt den Spieler nicht
    /// </summary>
    public class StunEffect : IEffect
    {
        public string Id => "stun";
        public string DisplayName => "Stunned";
        public float Duration { get; set; } = 2f;
        public DateTime AppliedAt { get; set; }
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // Disable movement (CS2 API)
            var pawn = player.PlayerPawn.Value;
            if (pawn.MovementServices != null)
            {
                // Set movement speed to 0 (effectively disabling movement)
                pawn.MovementServices.MoveSpeedFactor = 0f;
            }
            
            player.PrintToChat($" {ChatColors.Red}[Stunned]{ChatColors.Default} You are stunned for {Duration:F1}s!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep movement disabled (handled by game mechanics)
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // Re-enable movement (CS2 API)
            var pawn = player.PlayerPawn.Value;
            if (pawn.MovementServices != null)
            {
                // Restore normal movement speed
                pawn.MovementServices.MoveSpeedFactor = 1.0f;
            }
            
            player.PrintToChat($" {ChatColors.Green}[Stun]{ChatColors.Default} Stun effect removed!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
