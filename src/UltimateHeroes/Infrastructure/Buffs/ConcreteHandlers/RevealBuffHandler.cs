using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;
using UltimateHeroes.Infrastructure.Buffs;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Infrastructure.Buffs.ConcreteHandlers
{
    /// <summary>
    /// Handler für Reveal Buffs
    /// </summary>
    public class RevealBuffHandler : IBuffHandler
    {
        public BuffType HandlesType => BuffType.Reveal;
        
        public void OnApply(CCSPlayerController player, Buff buff)
        {
            if (player == null || !player.IsValid) return;
            
            // Make player visible
            GameHelpers.MakePlayerInvisible(player, false);
        }
        
        public void OnRemove(CCSPlayerController player, Buff buff)
        {
            // Reveal removal doesn't need special handling
            // (player stays visible unless another effect makes them invisible)
        }
        
        public void OnTick(CCSPlayerController player, Buff buff)
        {
            // Keep player visible
            OnApply(player, buff);
        }
    }
}
