using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Builds;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Bot Management und Balancing
    /// </summary>
    public interface IBotService
    {
        // Bot Configuration
        void ConfigureBot(string steamId, BotConfiguration config);
        BotConfiguration? GetBotConfiguration(string steamId);
        
        // Bot Build Management
        void AssignRandomBuild(string steamId);
        void AssignPredefinedBuild(string steamId, string buildPreset);
        void AssignBuildFromPool(string steamId, List<string> buildPool);
        
        // Bot Stats
        BotStats GetBotStats(string steamId);
        List<BotStats> GetAllBotStats();
        void ResetBotStats(string steamId);
        
        // Bot Detection
        bool IsBot(CCSPlayerController player);
        bool IsBot(string steamId);
        
        // Bot Events
        void OnBotKill(string steamId, string skillId = "", float damage = 0f);
        void OnBotDeath(string steamId);
        void CheckBuildChange(string steamId);
    }
    
    public class BotConfiguration
    {
        public string SteamId { get; set; } = string.Empty;
        public BotLevelMode LevelMode { get; set; } = BotLevelMode.Level0;
        public int FixedLevel { get; set; } = 0; // Wenn LevelMode = Fixed
        public BotBuildMode BuildMode { get; set; } = BotBuildMode.Random;
        public List<string> PredefinedBuilds { get; set; } = new(); // Für Predefined Mode
        public List<string> BuildPool { get; set; } = new(); // Für Pool Mode
        public bool TrackStats { get; set; } = true;
        public bool AutoAssignBuild { get; set; } = true;
        public float BuildChangeInterval { get; set; } = 300f; // Sekunden zwischen Build-Wechsel
    }
    
    public enum BotLevelMode
    {
        Level0,      // Immer Level 0
        MaxLevel,    // Immer Max Level
        Random,      // Zufälliges Level
        Fixed,       // Festes Level (FixedLevel)
        MatchPlayerAverage // Durchschnittliches Level der Spieler
    }
    
    public enum BotBuildMode
    {
        Random,          // Zufällige Builds
        Predefined,      // Nur Predefined Builds
        Pool,            // Aus Build Pool
        Rotate,          // Rotiert durch alle Builds
        Balanced         // Versucht balanced Builds zu nutzen
    }
    
    public class BotStats
    {
        public string SteamId { get; set; } = string.Empty;
        public string BotName { get; set; } = string.Empty;
        
        // Match Stats
        public int TotalKills { get; set; } = 0;
        public int TotalDeaths { get; set; } = 0;
        public int TotalAssists { get; set; } = 0;
        public float TotalDamage { get; set; } = 0f;
        public int RoundsWon { get; set; } = 0;
        public int RoundsLost { get; set; } = 0;
        
        // Build Stats
        public Dictionary<string, BuildPerformance> BuildPerformances { get; set; } = new(); // build_id -> stats
        public Dictionary<string, HeroPerformance> HeroPerformances { get; set; } = new(); // hero_id -> stats
        public Dictionary<string, SkillPerformance> SkillPerformances { get; set; } = new(); // skill_id -> stats
        
        // Performance Metrics
        public float KDRatio => TotalDeaths > 0 ? (float)TotalKills / TotalDeaths : TotalKills;
        public float WinRate => (RoundsWon + RoundsLost) > 0 ? (float)RoundsWon / (RoundsWon + RoundsLost) : 0f;
        public float AverageDamage => (TotalKills + TotalDeaths) > 0 ? TotalDamage / (TotalKills + TotalDeaths) : 0f;
    }
    
    public class BuildPerformance
    {
        public string BuildId { get; set; } = string.Empty;
        public int Uses { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public float Damage { get; set; } = 0f;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public float KDRatio => Deaths > 0 ? (float)Kills / Deaths : Kills;
        public float WinRate => (Wins + Losses) > 0 ? (float)Wins / (Wins + Losses) : 0f;
    }
    
    public class HeroPerformance
    {
        public string HeroId { get; set; } = string.Empty;
        public int Uses { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public float Damage { get; set; } = 0f;
        public float KDRatio => Deaths > 0 ? (float)Kills / Deaths : Kills;
    }
    
    public class SkillPerformance
    {
        public string SkillId { get; set; } = string.Empty;
        public int Uses { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public float Damage { get; set; } = 0f;
        public float Effectiveness => Uses > 0 ? (float)Kills / Uses : 0f;
    }
}
