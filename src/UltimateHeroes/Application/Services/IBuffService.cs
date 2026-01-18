using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Buffs;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für Buff Management
    /// </summary>
    public interface IBuffService
    {
        // Buff Management
        void ApplyBuff(string steamId, Buff buff);
        void RemoveBuff(string steamId, string buffId);
        void RemoveAllBuffs(string steamId);
        bool HasBuff(string steamId, string buffId);
        List<Buff> GetPlayerBuffs(string steamId);
        List<Buff> GetPlayerBuffs(string steamId, BuffType type);
        
        // Buff Queries
        float GetBuffValue(string steamId, BuffType type, string parameterKey, float defaultValue = 0f);
        float GetTotalBuffValue(string steamId, BuffType type, string parameterKey, float defaultValue = 0f);
        
        // Helper Methods (for convenience, but Skills should create Buff objects directly)
        void ApplyDamageBoost(string steamId, float multiplier, float duration);
        void ApplySpeedBoost(string steamId, float multiplier, float duration);
        void ApplyWeaponSpread(string steamId, float spreadMultiplier, float duration);
        void ApplyTaunt(string steamId, string taunterSteamId, float duration);
        void ApplyReveal(string steamId, float duration);
        void ApplyExecutionMark(string steamId, float damageMultiplier, float duration);
        void ApplyShield(string steamId, float damageReduction, float duration);
        void ApplyInfiniteAmmo(string steamId, float duration);
        
        // Tick (should be called regularly)
        void TickBuffs();
    }
}
