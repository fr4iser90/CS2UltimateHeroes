using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für In-Match Evolution
    /// </summary>
    public interface IInMatchEvolutionService
    {
        // Round-based Events
        void OnRoundStart(string steamId, int roundNumber);
        void OnRoundEnd(string steamId, bool won);
        
        // Time-based Events
        void OnTimeUpdate(string steamId, float minutesElapsed);
        
        // Kill Streak
        void OnKill(string steamId);
        void OnDeath(string steamId);
        
        // Objectives
        void OnObjective(string steamId, ObjectiveType type);
        
        // Queries
        MatchProgress? GetMatchProgress(string steamId);
        Dictionary<string, float> GetActiveUpgrades(string steamId);
        
        // Reset
        void ResetMatchProgress(string steamId);
    }
    
    public enum ObjectiveType
    {
        BombPlant,
        BombDefuse,
        HostageRescue,
        RoundWin,
        Clutch
    }
}
