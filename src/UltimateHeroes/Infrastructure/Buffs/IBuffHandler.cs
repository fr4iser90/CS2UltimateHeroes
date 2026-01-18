using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;

namespace UltimateHeroes.Infrastructure.Buffs
{
    /// <summary>
    /// Interface für Buff Handler (Strategy Pattern)
    /// Jeder Buff-Type hat einen Handler für Apply/Remove/Tick
    /// </summary>
    public interface IBuffHandler
    {
        BuffType HandlesType { get; }
        
        void OnApply(CCSPlayerController player, Buff buff);
        void OnRemove(CCSPlayerController player, Buff buff);
        void OnTick(CCSPlayerController player, Buff buff);
    }
}
