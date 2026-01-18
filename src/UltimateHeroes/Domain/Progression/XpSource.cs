namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// XP Quellen Enum
    /// </summary>
    public enum XpSource
    {
        Kill,
        Headshot,
        Assist,
        Objective,
        RoundWin,
        FlashAssist,
        ClutchRound,
        FirstBlood,
        MatchCompletion, // Neu: Match Completion XP (50-100)
        WinBonus         // Neu: Win Bonus (Multiplikator, nicht direkt)
    }
}
