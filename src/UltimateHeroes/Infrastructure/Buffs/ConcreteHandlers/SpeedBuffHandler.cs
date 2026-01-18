using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;
using UltimateHeroes.Infrastructure.Buffs;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Infrastructure.Buffs.ConcreteHandlers
{
    /// <summary>
    /// Handler für Speed Buffs (Boost & Reduction)
    /// </summary>
    public class SpeedBuffHandler : IBuffHandler
    {
        public BuffType HandlesType => BuffType.SpeedBoost; // Also handles SpeedReduction
        
        public void OnApply(CCSPlayerController player, Buff buff)
        {
            if (player == null || !player.IsValid) return;
            
            var multiplier = buff.Parameters.GetValueOrDefault("multiplier", 0f);
            
            // Handle both SpeedBoost (positive) and SpeedReduction (negative)
            if (buff.Type == BuffType.SpeedReduction)
            {
                // SpeedReduction uses negative multiplier
                GameHelpers.SetMovementSpeed(player, 1.0f + multiplier);
            }
            else
            {
                // SpeedBoost uses positive multiplier
                GameHelpers.SetMovementSpeed(player, 1.0f + multiplier);
            }
        }
        
        public void OnRemove(CCSPlayerController player, Buff buff)
        {
            if (player == null || !player.IsValid) return;
            
            // Reset to base speed
            GameHelpers.SetMovementSpeed(player, 1.0f);
        }
        
        public void OnTick(CCSPlayerController player, Buff buff)
        {
            // Re-apply speed (in case it was overridden)
            OnApply(player, buff);
        }
    }
    
    /// <summary>
    /// Handler für Speed Reduction (separate handler for clarity)
    /// </summary>
    public class SpeedReductionBuffHandler : IBuffHandler
    {
        public BuffType HandlesType => BuffType.SpeedReduction;
        
        public void OnApply(CCSPlayerController player, Buff buff)
        {
            if (player == null || !player.IsValid) return;
            
            var multiplier = buff.Parameters.GetValueOrDefault("multiplier", 0f);
            GameHelpers.SetMovementSpeed(player, 1.0f + multiplier); // Negative value = reduction
        }
        
        public void OnRemove(CCSPlayerController player, Buff buff)
        {
            if (player == null || !player.IsValid) return;
            GameHelpers.SetMovementSpeed(player, 1.0f);
        }
        
        public void OnTick(CCSPlayerController player, Buff buff)
        {
            OnApply(player, buff);
        }
    }
}
