using System.Collections.Generic;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Infrastructure.Configuration
{
    /// <summary>
    /// Plugin Configuration (ausgelagert aus UltimateHeroes.cs)
    /// </summary>
    public class PluginConfiguration : BasePluginConfig
    {
        [JsonPropertyName("ConfigVersion")]
        public override int Version { get; set; } = 1;

        [JsonPropertyName("DefaultHero")]
        public string DefaultHero { get; set; } = GameConstants.DefaultHeroId;

        [JsonPropertyName("MaxSkillSlots")]
        public int MaxSkillSlots { get; set; } = GameConstants.DefaultMaxSkillSlots;

        [JsonPropertyName("MaxPowerBudget")]
        public int MaxPowerBudget { get; set; } = GameConstants.DefaultMaxPowerBudget;
        
        [JsonPropertyName("BotSettings")]
        public BotSettingsConfig BotSettings { get; set; } = new();
        
        [JsonPropertyName("LeaderboardSettings")]
        public LeaderboardSettingsConfig LeaderboardSettings { get; set; } = new();
    }
    
    public class BotSettingsConfig
    {
        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; } = GameConstants.BotEnabledDefault;
        
        [JsonPropertyName("DefaultLevelMode")]
        public string DefaultLevelMode { get; set; } = "MatchPlayerAverage"; // Level0, MaxLevel, Random, Fixed, MatchPlayerAverage
        
        [JsonPropertyName("DefaultBuildMode")]
        public string DefaultBuildMode { get; set; } = "Random"; // Random, Predefined, Pool, Rotate, Balanced
        
        [JsonPropertyName("TrackStats")]
        public bool TrackStats { get; set; } = GameConstants.BotTrackStatsDefault;
        
        [JsonPropertyName("AutoAssignBuild")]
        public bool AutoAssignBuild { get; set; } = GameConstants.BotAutoAssignBuildDefault;
        
        [JsonPropertyName("BuildChangeInterval")]
        public float BuildChangeInterval { get; set; } = GameConstants.BotBuildChangeIntervalDefault;
        
        [JsonPropertyName("PredefinedBuilds")]
        public List<string> PredefinedBuilds { get; set; } = new() { "dps", "mobility", "stealth", "support", "balanced" };
        
        [JsonPropertyName("BuildPool")]
        public List<string> BuildPool { get; set; } = new();

        [JsonPropertyName("XpPerKill")]
        public float XpPerKill { get; set; } = GameConstants.BotXpPerKillDefault;
        
        [JsonPropertyName("XpPersistence")]
        public string XpPersistence { get; set; } = "Persistent"; // "Reset", "Persistent", "Max"
        // Reset: Bots starten jede Runde bei Level 0
        // Persistent: Bots behalten XP aus der Datenbank
        // Max: Bots werden auf Max-Level gesetzt (überschreibt nicht, wenn bereits höher)
    }
    
    public class LeaderboardSettingsConfig
    {
        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; } = true;
        
        [JsonPropertyName("ShowHero")]
        public bool ShowHero { get; set; } = true;
        
        [JsonPropertyName("ShowLevel")]
        public bool ShowLevel { get; set; } = true;
        
        [JsonPropertyName("LevelType")]
        public string LevelType { get; set; } = "Hero"; // "Hero" or "Account"
        
        [JsonPropertyName("MaxPlayers")]
        public int MaxPlayers { get; set; } = 10;
    }
}
