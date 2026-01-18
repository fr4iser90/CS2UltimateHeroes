using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Taunt Effect - Zwingt Gegner, dich anzugreifen (simplified: Stun)
    /// </summary>
    public class TauntEffect : IEffect
    {
        public string Id => "taunt";
        public string DisplayName => "Taunted";
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        public string TaunterSteamId { get; set; } = string.Empty;
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Apply stun (disable movement)
            if (player.PlayerPawn.Value?.MovementServices != null)
            {
                player.PlayerPawn.Value.MovementServices.MoveSpeedFactor = 0f;
            }
            
            player.PrintToChat($" {ChatColors.Red}[Taunt]{ChatColors.Default} You are taunted! You must attack the taunter!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep movement disabled
            if (player != null && player.IsValid && player.PlayerPawn.Value?.MovementServices != null)
            {
                player.PlayerPawn.Value.MovementServices.MoveSpeedFactor = 0f;
            }
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Restore movement
            if (player.PlayerPawn.Value?.MovementServices != null)
            {
                player.PlayerPawn.Value.MovementServices.MoveSpeedFactor = 1f;
            }
            
            player.PrintToChat($" {ChatColors.Red}[Taunt]{ChatColors.Default} Taunt expired!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
