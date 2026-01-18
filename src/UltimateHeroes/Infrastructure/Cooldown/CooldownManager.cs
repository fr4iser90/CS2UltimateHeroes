using System;
using System.Collections.Generic;

namespace UltimateHeroes.Infrastructure.Cooldown
{
    /// <summary>
    /// Verwaltet Skill Cooldowns für Spieler
    /// </summary>
    public class CooldownManager : ICooldownManager
    {
        private readonly Dictionary<string, Dictionary<string, DateTime>> _cooldowns = new(); // steamid -> skillid -> endtime
        
        public void SetCooldown(string steamId, string skillId, float cooldownSeconds)
        {
            if (!_cooldowns.ContainsKey(steamId))
            {
                _cooldowns[steamId] = new Dictionary<string, DateTime>();
            }
            
            var endTime = DateTime.UtcNow.AddSeconds(cooldownSeconds);
            _cooldowns[steamId][skillId] = endTime;
        }
        
        public float GetCooldown(string steamId, string skillId)
        {
            if (!_cooldowns.ContainsKey(steamId)) return 0f;
            if (!_cooldowns[steamId].ContainsKey(skillId)) return 0f;
            
            var endTime = _cooldowns[steamId][skillId];
            var remaining = (float)(endTime - DateTime.UtcNow).TotalSeconds;
            return Math.Max(0f, remaining);
        }
        
        public bool IsReady(string steamId, string skillId)
        {
            return GetCooldown(steamId, skillId) <= 0f;
        }
        
        public void ClearCooldown(string steamId, string skillId)
        {
            if (_cooldowns.ContainsKey(steamId))
            {
                _cooldowns[steamId].Remove(skillId);
            }
        }
        
        public void ClearAllCooldowns(string steamId)
        {
            _cooldowns.Remove(steamId);
        }
    }
}
