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
                // Refill ammo to max (CS2 API)
                if (weapon.Clip1 != null)
                {
                    var maxClip = weapon.GetMaxClip1();
                    if (maxClip > 0)
                    {
                        weapon.Clip1.Value = maxClip;
                    }
                }
            }
            
            // Apply Spread Modifier
            var spreadMultiplier = GetSpreadMultiplier(steamId);
            if (spreadMultiplier.HasValue && spreadMultiplier.Value != 1f)
            {
                // Modify weapon spread via ConVar (CS2 API)
                // Note: This requires weapon-specific ConVars
                // For now, we track the modifier and apply it via game mechanics
                // The actual spread modification would need to be done via weapon properties
                // This is a placeholder for when CS2 API supports direct spread modification
            }
        }
        
        public void OnWeaponReload(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            
            // Apply Infinite Ammo (refill ammo on reload)
            if (HasInfiniteAmmo(steamId))
            {
                var weapon = pawn.WeaponServices?.ActiveWeapon.Value;
                if (weapon != null && weapon.IsValid)
                {
                    // Set ammo to max (CS2 API)
                    if (weapon.Clip1 != null)
                    {
                        var maxClip = weapon.GetMaxClip1();
                        if (maxClip > 0)
                        {
                            weapon.Clip1.Value = maxClip;
                        }
                    }
                    
                    if (weapon.ReserveAmmo != null)
                    {
                        var maxReserve = weapon.GetMaxReserveAmmo();
                        if (maxReserve > 0)
                        {
                            weapon.ReserveAmmo.Value = maxReserve;
                        }
                    }
                }
            }
        }
    }
}
