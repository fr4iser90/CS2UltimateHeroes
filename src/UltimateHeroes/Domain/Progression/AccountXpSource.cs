namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// Account XP Quellen (separat von Hero XP)
    /// </summary>
    public enum AccountXpSource
    {
        MatchCompletion,    // +10 Account XP pro Match
        HeroLevelUp,        // +Account XP basierend auf Hero-Level
        DailyQuest,         // Später: Daily Quests
        Achievement,        // Später: Achievements
        Prestige           // Später: Prestige Rewards
    }
}
