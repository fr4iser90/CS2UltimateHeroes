using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Buffs;

namespace UltimateHeroes.Infrastructure.Weapons
{
    /// <summary>
    /// Weapon Modifier - Modifiziert Weapon Properties basierend auf Buffs
    /// </summary>
    public class WeaponModifier : IWeaponModifier
    {
        private readonly IBuffService _buffService;
        
        public WeaponModifier(IBuffService buffService)
        {
            _buffService = buffService;
        }
        
        public float? GetSpreadMultiplier(string steamId)
        {
            // Check for Taunt (increased spread)
            var tauntSpread = _buffService.GetBuffValue(steamId, BuffType.Taunt, "spread_multiplier", 1f);
            if (tauntSpread > 1f)
            {
                return tauntSpread;
            }
            
            // Check for Accuracy Boost (reduced spread)
            var accuracyBoost = _buffService.GetBuffValue(steamId, BuffType.AccuracyBoost, "multiplier", 0f);
            if (accuracyBoost > 0f)
            {
                return 1f - accuracyBoost; // Reduce spread
            }
            
            return null; // No modifier
        }
        
        public float? GetFireRateMultiplier(string steamId)
        {
            // Check for Bullet Storm (increased fire rate)
            var fireRateBoost = _buffService.GetBuffValue(steamId, BuffType.FireRateBoost, "multiplier", 0f);
            if (fireRateBoost > 0f)
            {
                return 1f + fireRateBoost;
            }
            
            return null; // No modifier
        }
        
        public bool HasInfiniteAmmo(string steamId)
        {
            return _buffService.HasBuff(steamId, "infinite_ammo");
        }
        
        public float? GetDamageMultiplier(string steamId)
        {
            // Check for Damage Boost
            var damageBoost = _buffService.GetTotalBuffValue(steamId, BuffType.DamageBoost, "multiplier", 0f);
            if (damageBoost > 0f)
            {
                return 1f + damageBoost;
            }
            
            return null; // No modifier
        }
        
        public float? GetRecoilMultiplier(string steamId)
        {
            // Check for Recoil Reduction (via Accuracy Boost or similar)
            var recoilReduction = _buffService.GetBuffValue(steamId, BuffType.AccuracyBoost, "recoil_reduction", 0f);
            if (recoilReduction > 0f)
            {
                return 1f - recoilReduction; // Reduce recoil
            }
            
            return null; // No modifier
        }
        
        public void OnWeaponFire(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var pawn = player.PlayerPawn.Value;
            if (pawn?.WeaponServices?.ActiveWeapon.Value == null) return;
            
            var weapon = pawn.WeaponServices.ActiveWeapon.Value;
            
            // Apply Infinite Ammo (refill ammo after shot)
            if (HasInfiniteAmmo(steamId))
            {
                // TODO: Refill ammo to max
                // This might require CS2-specific API calls
            }
            
            // Apply Spread Modifier
            var spreadMultiplier = GetSpreadMultiplier(steamId);
            if (spreadMultiplier.HasValue && spreadMultiplier.Value != 1f)
            {
                // TODO: Modify weapon spread
                // This might require CS2-specific API calls or ConVar manipulation
            }
        }
        
        public void OnWeaponReload(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            
            // Apply Infinite Ammo (refill ammo on reload)
            if (HasInfiniteAmmo(steamId))
            {
                // TODO: Set ammo to max
            }
        }
    }
}
