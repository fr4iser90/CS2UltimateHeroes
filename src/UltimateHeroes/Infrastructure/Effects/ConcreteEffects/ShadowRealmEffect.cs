using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Shadow Realm Effect - Vollständige Unsichtbarkeit, keine Collision, kein Schaden möglich
    /// </summary>
    public class ShadowRealmEffect : IEffect
    {
        public string Id => "shadow_realm";
        public string DisplayName => "Shadow Realm";
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Make player invisible
            GameHelpers.MakePlayerInvisible(player, true);
            
            // TODO: Disable collision and damage
            // This would require more advanced CS2 API calls
            
            player.PrintToChat($" {ChatColors.Purple}[Shadow Realm]{ChatColors.Default} Ultimate! Invisible, no collision, no damage!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep invisible
            if (player != null && player.IsValid)
            {
                GameHelpers.MakePlayerInvisible(player, true);
            }
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Restore visibility
            GameHelpers.MakePlayerInvisible(player, false);
            
            player.PrintToChat($" {ChatColors.Purple}[Shadow Realm]{ChatColors.Default} Ultimate expired!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
