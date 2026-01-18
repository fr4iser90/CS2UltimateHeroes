using System;
using System.Collections.Generic;

namespace UltimateHeroes.Domain.Buffs
{
    /// <summary>
    /// Buff/Debuff Domain Model
    /// </summary>
    public class Buff
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public BuffType Type { get; set; }
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        // Buff Parameters (flexible dictionary)
        public Dictionary<string, float> Parameters { get; set; } = new();
        
        // Stacking
        public int MaxStacks { get; set; } = 1;
        public int CurrentStacks { get; set; } = 1;
        public BuffStackingType StackingType { get; set; } = BuffStackingType.Refresh;
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
        
        public float GetRemainingDuration()
        {
            var elapsed = (DateTime.UtcNow - AppliedAt).TotalSeconds;
            return Math.Max(0f, Duration - (float)elapsed);
        }
    }
    
    public enum BuffType
    {
        // Positive Buffs
        DamageBoost,
        SpeedBoost,
        ArmorBoost,
        HealthRegen,
        CooldownReduction,
        FireRateBoost,
        AccuracyBoost,
        
        // Negative Debuffs
        DamageReduction,
        SpeedReduction,
        WeaponSpreadIncrease,
        CooldownIncrease,
        Taunt, // Special: Reduced damage if not attacking taunter
        
        // Special
        Reveal,
        Shield,
        ExecutionMark,
        InfiniteAmmo,
        CCImmunity
    }
    
    public enum BuffStackingType
    {
        Refresh,      // Refresh duration, keep stacks
        Additive,    // Add stacks, refresh duration
        Multiplicative, // Multiply values
        Max          // Take maximum value
    }
}
