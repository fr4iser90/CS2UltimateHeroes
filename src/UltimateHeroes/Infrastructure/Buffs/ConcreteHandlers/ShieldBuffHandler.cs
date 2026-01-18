using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;
using UltimateHeroes.Infrastructure.Buffs;

namespace UltimateHeroes.Infrastructure.Buffs.ConcreteHandlers
{
    /// <summary>
    /// Handler für Shield Buffs
    /// (Passive - wird in DamagePlayer abgefragt, keine direkte Anwendung nötig)
    /// </summary>
    public class ShieldBuffHandler : IBuffHandler
    {
        public BuffType HandlesType => BuffType.Shield;
        
        public void OnApply(CCSPlayerController player, Buff buff)
        {
            // Shield is passive - applied in GameHelpers.DamagePlayer
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
