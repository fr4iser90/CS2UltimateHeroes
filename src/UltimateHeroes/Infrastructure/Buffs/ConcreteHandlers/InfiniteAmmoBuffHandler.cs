using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;
using UltimateHeroes.Infrastructure.Buffs;

namespace UltimateHeroes.Infrastructure.Buffs.ConcreteHandlers
{
    /// <summary>
    /// Handler für Infinite Ammo Buffs
    /// </summary>
    public class InfiniteAmmoBuffHandler : IBuffHandler
    {
        public BuffType HandlesType => BuffType.InfiniteAmmo;
        
        public void OnApply(CCSPlayerController player, Buff buff)
        {
            // Infinite Ammo is handled on weapon fire/reload
            // No immediate action needed
        }
        
        public void OnRemove(CCSPlayerController player, Buff buff)
        {
            // No cleanup needed
        }
        
        public void OnTick(CCSPlayerController player, Buff buff)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // Refill ammo every tick
            var weapon = player.PlayerPawn.Value.WeaponServices?.ActiveWeapon.Value;
            if (weapon != null && weapon.IsValid)
            {
                // TODO: Set ammo to max (requires CS2 API)
                // This would be handled by WeaponModifier system
            }
        }
    }
}
