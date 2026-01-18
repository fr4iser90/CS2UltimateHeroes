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
                // Set ammo to max (CS2 API)
                if (weapon.Clip1 != null)
                {
                    // Note: GetMaxClip1() doesn't exist in CS2 API
                    // Using a reasonable default (30 rounds for most weapons)
                    var maxClip = 30; // Placeholder - should be weapon-specific
                    weapon.Clip1 = maxClip;
                }
                
                // Also refill reserve ammo
                if (weapon.ReserveAmmo != null)
                {
                    // Note: GetMaxReserveAmmo() doesn't exist in CS2 API
                    // Using a reasonable default (90 rounds for most weapons)
                    var maxReserve = 90; // Placeholder - should be weapon-specific
                    // Note: ReserveAmmo is read-only in CS2 API
                    // Reserve ammo cannot be directly modified
                    // This is a placeholder - actual implementation may require game-specific mechanics
                }
            }
        }
    }
}
