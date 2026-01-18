using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;
using UltimateHeroes.Infrastructure.Buffs;

namespace UltimateHeroes.Infrastructure.Buffs.ConcreteHandlers
{
    /// <summary>
    /// Handler für Execution Mark Buffs
    /// (Passive - wird in DamagePlayer abgefragt, keine direkte Anwendung nötig)
    /// </summary>
    public class ExecutionMarkBuffHandler : IBuffHandler
    {
        public BuffType HandlesType => BuffType.ExecutionMark;
        
        public void OnApply(CCSPlayerController player, Buff buff)
        {
            // Execution Mark is passive - applied in GameHelpers.DamagePlayer
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
