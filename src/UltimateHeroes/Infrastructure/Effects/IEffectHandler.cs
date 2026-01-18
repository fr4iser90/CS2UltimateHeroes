using CounterStrikeSharp.API.Core;
using UltimateHeroes.Infrastructure.Effects;

namespace UltimateHeroes.Infrastructure.Effects
{
    /// <summary>
    /// Interface für Effect Handler (Strategy Pattern)
    /// Jeder Effect-Type hat einen Handler für Apply/Remove/Tick
    /// </summary>
    public interface IEffectHandler
    {
        string HandlesEffectId { get; } // Effect ID this handler manages
        
        void OnApply(CCSPlayerController player, IEffect effect);
        void OnRemove(CCSPlayerController player, IEffect effect);
        void OnTick(CCSPlayerController player, IEffect effect);
    }
}
