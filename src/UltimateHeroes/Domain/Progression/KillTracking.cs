using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// Tracking für Kill-Diminishing Returns (Anti-Exploit gemäß LEVELING.md)
    /// </summary>
    public class KillTracking
    {
        public string SteamId { get; set; } = string.Empty;
        public Dictionary<string, KillRecord> VictimKills { get; set; } = new(); // victim_steamid -> KillRecord
        public int TotalKills { get; set; } = 0;
        public DateTime LastKillTime { get; set; } = DateTime.MinValue;
        
        /// <summary>
        /// Berechnet XP-Multiplikator basierend auf Kill-Diminishing
        /// 1-5 Kill: 100%
        /// 6-10 Kill: -10%
        /// 10+ gleicher Gegner: -25%
        /// </summary>
        public float GetKillXpMultiplier(string victimSteamId)
        {
            float multiplier = 1.0f;
            
            // Kill 6-10: -10%
            if (TotalKills >= 6 && TotalKills <= 10)
            {
                multiplier = 0.9f;
            }
            
            // Gleicher Gegner mehrfach: -25%
            if (VictimKills.TryGetValue(victimSteamId, out var record))
            {
                if (record.KillCount >= 10)
                {
                    multiplier = 0.75f; // -25%
                }
            }
            
            return multiplier;
        }
        
        /// <summary>
        /// Registriert einen Kill
        /// </summary>
        public void RecordKill(string victimSteamId)
        {
            TotalKills++;
            LastKillTime = DateTime.UtcNow;
            
            if (!VictimKills.TryGetValue(victimSteamId, out var record))
            {
                record = new KillRecord { VictimSteamId = victimSteamId };
                VictimKills[victimSteamId] = record;
            }
            
            record.KillCount++;
            record.LastKillTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Bereinigt alte Kill-Records (älter als 5 Minuten)
        /// </summary>
        public void CleanupOldKills(TimeSpan maxAge)
        {
            var cutoff = DateTime.UtcNow - maxAge;
            var toRemove = VictimKills
                .Where(kvp => kvp.Value.LastKillTime < cutoff)
                .Select(kvp => kvp.Key)
                .ToList();
            
            foreach (var key in toRemove)
            {
                VictimKills.Remove(key);
            }
        }
        
        /// <summary>
        /// Reset für neues Match
        /// </summary>
        public void ResetForNewMatch()
        {
            VictimKills.Clear();
            TotalKills = 0;
            LastKillTime = DateTime.MinValue;
        }
    }
    
    public class KillRecord
    {
        public string VictimSteamId { get; set; } = string.Empty;
        public int KillCount { get; set; } = 0;
        public DateTime LastKillTime { get; set; } = DateTime.UtcNow;
    }
}
