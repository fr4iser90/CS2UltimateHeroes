using CounterStrikeSharp.API.Core;

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
            
            // TODO: Disable movement
            // player.PlayerPawn.Value.MovementServices?.Disable();
            
            // Placeholder: Notify player
            player.PrintToChat($" {ChatColors.Red}[Stunned]{ChatColors.Default} You are stunned for {Duration:F1}s!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep movement disabled (handled by game mechanics)
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // TODO: Re-enable movement
            // player.PlayerPawn.Value.MovementServices?.Enable();
            
            // Placeholder: Notify player
            player.PrintToChat($" {ChatColors.Green}[Stun]{ChatColors.Default} Stun effect removed!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
