namespace UltimateHeroes.Infrastructure.Configuration
{
    /// <summary>
    /// Game Constants - Magic Numbers und konfigurierbare Werte
    /// </summary>
    public static class GameConstants
    {
        // Timer Intervals
        public const float EffectTickInterval = 0.5f;
        public const float HudUpdateInterval = 0.5f;
        public const float BotBuildChangeInterval = 30f;
        public const float InMatchEvolutionTickInterval = 1f;
        
        // Default Values
        public const string DefaultHeroId = "vanguard";
        public const int DefaultMaxSkillSlots = 3;
        public const int DefaultMaxPowerBudget = 100;
        
        // Bot Defaults
        public const bool BotEnabledDefault = true;
        public const bool BotTrackStatsDefault = true;
        public const bool BotAutoAssignBuildDefault = true;
        public const float BotBuildChangeIntervalDefault = 300f;
        public const float BotXpPerKillDefault = 10f;
        
        // Command Prefixes
        public const string CommandPrefix = "css_";
        
        // Chat Colors (für Konsistenz)
        public const string ChatColorDefault = "default";
        public const string ChatColorRed = "red";
        public const string ChatColorGreen = "green";
        public const string ChatColorBlue = "blue";
        public const string ChatColorYellow = "yellow";
        public const string ChatColorGold = "gold";
    }
}
