using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Domain.Players;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Role Detection und Role-based Bonuses
    /// </summary>
    public class RoleInfluenceService : IRoleInfluenceService
    {
        private readonly IPlayerService _playerService;
        private readonly Dictionary<string, RoleMetrics> _roleMetrics = new();
        
        public RoleInfluenceService(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        
        public RoleInfluence DetectRole(string steamId)
        {
            var metrics = GetOrCreateMetrics(steamId);
            var player = _playerService.GetPlayer(steamId);
            
            if (player == null) return RoleInfluence.None;
            
            // Berechne Role Scores
            float dpsScore = metrics.DamageDealt + (metrics.Kills * 50f);
            float supportScore = metrics.Heals + (metrics.Assists * 30f) + (metrics.Smokes * 20f);
            float initiatorScore = metrics.EntryKills * 100f;
            float clutchScore = metrics.ClutchRounds * 200f;
            
            // Finde höchsten Score
            var scores = new Dictionary<RoleInfluence, float>
            {
                { RoleInfluence.DPS, dpsScore },
                { RoleInfluence.Support, supportScore },
                { RoleInfluence.Initiator, initiatorScore },
                { RoleInfluence.Clutch, clutchScore }
            };
            
            var maxScore = scores.Values.Max();
            if (maxScore < 100f) return RoleInfluence.None; // Nicht genug Daten
            
            return scores.FirstOrDefault(x => x.Value == maxScore).Key;
        }
        
        public void UpdateRoleMetrics(string steamId, RoleMetric metric, float value)
        {
            var metrics = GetOrCreateMetrics(steamId);
            
            switch (metric)
            {
                case RoleMetric.DamageDealt:
                    metrics.DamageDealt += value;
                    break;
                case RoleMetric.DamageTaken:
                    metrics.DamageTaken += value;
                    break;
                case RoleMetric.Kills:
                    metrics.Kills += (int)value;
                    break;
                case RoleMetric.Assists:
                    metrics.Assists += (int)value;
                    break;
                case RoleMetric.Heals:
                    metrics.Heals += value;
                    break;
                case RoleMetric.Smokes:
                    metrics.Smokes += (int)value;
                    break;
                case RoleMetric.Flashes:
                    metrics.Flashes += (int)value;
                    break;
                case RoleMetric.EntryKills:
                    metrics.EntryKills += (int)value;
                    break;
                case RoleMetric.ClutchRounds:
                    metrics.ClutchRounds += (int)value;
                    break;
            }
            
            // Update player role
            var player = _playerService.GetPlayer(steamId);
            if (player != null)
            {
                player.CurrentRole = DetectRole(steamId);
            }
        }
        
        public float GetRoleXpBonus(string steamId, RoleInfluence role)
        {
            var detectedRole = DetectRole(steamId);
            if (detectedRole == role)
            {
                return 0.10f; // +10% XP Bonus für passende Role
            }
            return 0f;
        }
        
        public Dictionary<string, float> GetRoleRecommendations(string steamId)
        {
            var role = DetectRole(steamId);
            var recommendations = new Dictionary<string, float>();
            
            switch (role)
            {
                case RoleInfluence.DPS:
                    recommendations["damage_bonus"] = 0.15f;
                    recommendations["crit_chance"] = 0.05f;
                    break;
                case RoleInfluence.Support:
                    recommendations["heal_bonus"] = 0.20f;
                    recommendations["cooldown_reduction"] = 0.15f;
                    break;
                case RoleInfluence.Initiator:
                    recommendations["movement_speed"] = 0.10f;
                    recommendations["entry_damage_bonus"] = 0.25f;
                    break;
                case RoleInfluence.Clutch:
                    recommendations["damage_bonus"] = 0.20f;
                    recommendations["health_bonus"] = 30f;
                    break;
            }
            
            return recommendations;
        }
        
        private RoleMetrics GetOrCreateMetrics(string steamId)
        {
            if (!_roleMetrics.TryGetValue(steamId, out var metrics))
            {
                metrics = new RoleMetrics { SteamId = steamId };
                _roleMetrics[steamId] = metrics;
            }
            return metrics;
        }
        
        private class RoleMetrics
        {
            public string SteamId { get; set; } = string.Empty;
            public float DamageDealt { get; set; } = 0f;
            public float DamageTaken { get; set; } = 0f;
            public int Kills { get; set; } = 0;
            public int Assists { get; set; } = 0;
            public float Heals { get; set; } = 0f;
            public int Smokes { get; set; } = 0;
            public int Flashes { get; set; } = 0;
            public int EntryKills { get; set; } = 0;
            public int ClutchRounds { get; set; } = 0;
        }
    }
}
