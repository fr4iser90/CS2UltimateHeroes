using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

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
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // TODO: Make player invisible
            // - Set alpha/visibility
            // - Increase movement speed by 20%
            
            // Placeholder: Notify player
            player.PrintToChat($" {ChatColors.Purple}[Stealth]{ChatColors.Default} You are invisible for {Duration:F1}s!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep invisibility active
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // TODO: Make player visible again
            // - Reset alpha/visibility
            // - Reset movement speed
            
            // Placeholder: Notify player
            player.PrintToChat($" {ChatColors.Default}[Stealth]{ChatColors.Default} Invisibility effect removed!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
