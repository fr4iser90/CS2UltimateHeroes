using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;
using UltimateHeroes.Infrastructure.Buffs;

namespace UltimateHeroes.Infrastructure.Buffs.ConcreteHandlers
{
    /// <summary>
    /// Handler für Fire Rate Boost Buffs
    /// (Wird via WeaponModifier System angewendet)
    /// </summary>
    public class FireRateBoostBuffHandler : IBuffHandler
    {
        public BuffType HandlesType => BuffType.FireRateBoost;
        
        public void OnApply(CCSPlayerController player, Buff buff)
        {
            // Fire Rate Boost is handled by WeaponModifier system
            // No immediate action needed
        }
        
        public void OnRemove(CCSPlayerController player, Buff buff)
        {
            // Cleanup handled by WeaponModifier system
        }
        
        public void OnTick(CCSPlayerController player, Buff buff)
        {
            // No tick action needed
        }
    }
}
