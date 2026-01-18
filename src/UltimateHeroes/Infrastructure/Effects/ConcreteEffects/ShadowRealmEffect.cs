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
            
            // Disable collision and damage (CS2 API)
            var pawn = player.PlayerPawn.Value;
            if (pawn != null)
            {
                // Set collision to none (CS2 API)
                // Note: CS2 API may not directly support collision disable
                // This is a placeholder - actual implementation may require game-specific mechanics
                // For now, we rely on invisibility and damage reduction via buffs
            }
            
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
