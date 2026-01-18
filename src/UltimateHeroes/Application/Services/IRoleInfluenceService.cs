using UltimateHeroes.Domain.Players;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Role Detection und Role-based Bonuses
    /// </summary>
    public interface IRoleInfluenceService
    {
        // Role Detection
        RoleInfluence DetectRole(string steamId);
        void UpdateRoleMetrics(string steamId, RoleMetric metric, float value);
        
        // Role-based Bonuses
        float GetRoleXpBonus(string steamId, RoleInfluence role);
        Dictionary<string, float> GetRoleRecommendations(string steamId);
    }
    
    public enum RoleMetric
    {
        DamageDealt,
        DamageTaken,
        Kills,
        Assists,
        Heals,
        Smokes,
        Flashes,
        EntryKills,
        ClutchRounds
    }
}
