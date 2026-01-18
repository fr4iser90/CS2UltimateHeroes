using System.Collections.Generic;

namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// Match Progress - Tracking für In-Match Evolution
    /// </summary>
    public class MatchProgress
    {
        public string SteamId { get; set; } = string.Empty;
        
        // Round/Time Tracking
        public int CurrentRound { get; set; } = 0;
        public float TimeElapsed { get; set; } = 0f; // in Minuten
        public int LastUpgradeInterval { get; set; } = 0; // für Time-based
        
        // Kill Streaks
        public int CurrentKillStreak { get; set; } = 0;
        public int HighestKillStreak { get; set; } = 0;
        public int LastKillStreakReward { get; set; } = 0; // Letzter vergebener Streak
        
        // Objectives
        public int ObjectivesCompleted { get; set; } = 0;
        
        // Mini-Upgrades (temporäre Buffs)
        public Dictionary<string, float> ActiveUpgrades { get; set; } = new(); // upgrade_id -> value
        
        // Match Stats
        public int MatchKills { get; set; } = 0;
        public int MatchDeaths { get; set; } = 0;
        public int MatchAssists { get; set; } = 0;
    }
}
