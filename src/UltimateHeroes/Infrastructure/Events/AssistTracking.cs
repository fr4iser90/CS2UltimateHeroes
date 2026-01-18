using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimateHeroes.Infrastructure.Events
{
    /// <summary>
    /// Assist Tracking System - Trackt Damage vor einem Kill für Assist-Detection
    /// </summary>
    public class AssistTracking
    {
        private const float AssistTimeWindow = 5f; // 5 Sekunden
        private const float AssistDamageThreshold = 50f; // Mindestens 50 Damage
        
        private readonly Dictionary<string, Dictionary<string, AssistDamage>> _damageTracking = new();
        
        /// <summary>
        /// Recorded damage dealt to a victim by an attacker
        /// </summary>
        private class AssistDamage
        {
            public float TotalDamage { get; set; }
            public DateTime LastDamageTime { get; set; }
        }
        
        /// <summary>
        /// Records damage dealt to a victim by an attacker
        /// </summary>
        public void RecordDamage(string attackerSteamId, string victimSteamId, float damage)
        {
            if (!_damageTracking.ContainsKey(victimSteamId))
            {
                _damageTracking[victimSteamId] = new Dictionary<string, AssistDamage>();
            }
            
            var victimDamage = _damageTracking[victimSteamId];
            if (!victimDamage.ContainsKey(attackerSteamId))
            {
                victimDamage[attackerSteamId] = new AssistDamage
                {
                    TotalDamage = 0f,
                    LastDamageTime = DateTime.UtcNow
                };
            }
            
            var assistDamage = victimDamage[attackerSteamId];
            assistDamage.TotalDamage += damage;
            assistDamage.LastDamageTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Gets all assists for a kill (attacker != killer, damage within time window, above threshold)
        /// </summary>
        public List<string> GetAssists(string killerSteamId, string victimSteamId)
        {
            if (!_damageTracking.ContainsKey(victimSteamId))
            {
                return new List<string>();
            }
            
            var victimDamage = _damageTracking[victimSteamId];
            var now = DateTime.UtcNow;
            var assists = new List<string>();
            
            foreach (var kvp in victimDamage)
            {
                var attackerSteamId = kvp.Key;
                var assistDamage = kvp.Value;
                
                // Skip killer (they get the kill, not assist)
                if (attackerSteamId == killerSteamId)
                {
                    continue;
                }
                
                // Check time window
                var timeSinceDamage = (now - assistDamage.LastDamageTime).TotalSeconds;
                if (timeSinceDamage > AssistTimeWindow)
                {
                    continue;
                }
                
                // Check damage threshold
                if (assistDamage.TotalDamage >= AssistDamageThreshold)
                {
                    assists.Add(attackerSteamId);
                }
            }
            
            return assists;
        }
        
        /// <summary>
        /// Clears damage tracking for a victim (after kill)
        /// </summary>
        public void ClearVictimTracking(string victimSteamId)
        {
            _damageTracking.Remove(victimSteamId);
        }
        
        /// <summary>
        /// Clears all tracking (on map change, etc.)
        /// </summary>
        public void ClearAll()
        {
            _damageTracking.Clear();
        }
    }
}
