using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Infrastructure.Weapons
{
    /// <summary>
    /// Interface für Weapon Modifier (Spread, Fire Rate, Ammo, etc.)
    /// </summary>
    public interface IWeaponModifier
    {
        // Weapon Properties
        float? GetSpreadMultiplier(string steamId);
        float? GetFireRateMultiplier(string steamId);
        bool HasInfiniteAmmo(string steamId);
        float? GetDamageMultiplier(string steamId);
        float? GetRecoilMultiplier(string steamId);
        
        // Apply Modifiers (called on weapon fire/reload)
        void OnWeaponFire(CCSPlayerController player);
        void OnWeaponReload(CCSPlayerController player);
    }
}
