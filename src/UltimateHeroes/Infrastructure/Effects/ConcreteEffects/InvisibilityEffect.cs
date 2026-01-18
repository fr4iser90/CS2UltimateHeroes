using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Invisibility Effect - Macht Spieler unsichtbar
    /// </summary>
    public class InvisibilityEffect : IEffect
    {
        public string Id => "invisibility";
        public string DisplayName => "Invisible";
        public float Duration { get; set; } = 5f;
        public DateTime AppliedAt { get; set; }
        
        private float _originalSpeed = 1f;
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // Make player invisible
            GameHelpers.MakePlayerInvisible(player, true);
            
            // Increase movement speed by 20%
            var pawn = player.PlayerPawn.Value;
            if (pawn.MovementServices != null)
            {
                _originalSpeed = pawn.MovementServices.MoveSpeedFactor;
                pawn.MovementServices.MoveSpeedFactor = _originalSpeed * 1.2f;
            }
            
            player.PrintToChat($" {ChatColors.Purple}[Stealth]{ChatColors.Default} You are invisible for {Duration:F1}s!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep invisibility active
            if (player != null && player.IsValid && player.PlayerPawn.Value != null)
            {
                GameHelpers.MakePlayerInvisible(player, true);
            }
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // Make player visible again
            GameHelpers.MakePlayerInvisible(player, false);
            
            // Reset movement speed
            var pawn = player.PlayerPawn.Value;
            if (pawn.MovementServices != null)
            {
                pawn.MovementServices.MoveSpeedFactor = _originalSpeed;
            }
            
            player.PrintToChat($" {ChatColors.Default}[Stealth]{ChatColors.Default} Invisibility effect removed!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
