using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;
using UltimateHeroes.Infrastructure.Buffs;

namespace UltimateHeroes.Infrastructure.Buffs.ConcreteHandlers
{
    /// <summary>
    /// Handler für Taunt Buffs
    /// (Passive - wird in DamagePlayer abgefragt, keine direkte Anwendung nötig)
    /// </summary>
    public class TauntBuffHandler : IBuffHandler
    {
        public BuffType HandlesType => BuffType.Taunt;
        
        public void OnApply(CCSPlayerController player, Buff buff)
        {
            // Taunt is passive - applied in GameHelpers.DamagePlayer
            // Weapon spread is handled by WeaponModifier system
            // No immediate action needed
        }
        
        public void OnRemove(CCSPlayerController player, Buff buff)
        {
            // No cleanup needed
        }
        
        public void OnTick(CCSPlayerController player, Buff buff)
        {
            // No tick action needed
        }
    }
}
