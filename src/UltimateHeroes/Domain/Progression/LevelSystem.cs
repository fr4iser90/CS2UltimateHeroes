namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// Level System - Verwaltet Level Limits und Unlocks
    /// </summary>
    public class LevelSystem
    {
        // Hero Level (1-40) - Max Level für Prestige-Vorbereitung
        public const int MaxHeroLevel = 40;
        
        // Skill Level (1-5)
        public const int MaxSkillLevel = 5;
        
        /// <summary>
        /// Gibt Talent Points für ein Level zurück (1 Talent Point pro Level)
        /// </summary>
        public static int GetTalentPointsForLevel(int heroLevel)
        {
            return heroLevel;
        }
        
        /// <summary>
        /// Gibt Anzahl freigeschalteter Skill Slots für ein Level zurück
        /// </summary>
        public static int GetSkillSlotsForLevel(int heroLevel)
        {
            if (heroLevel >= 20) return 5;
            if (heroLevel >= 10) return 4;
            return 3;
        }
    }
}
