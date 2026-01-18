namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// Prestige System - Vorbereitung für horizontale Progression (gemäß LEVELING.md)
    /// </summary>
    public class Prestige
    {
        public string SteamId { get; set; } = string.Empty;
        public int PrestigeLevel { get; set; } = 0; // 0 = No Prestige
        public int TotalPrestigePoints { get; set; } = 0;
        
        // Prestige-Unlocks (horizontal, nicht vertikal)
        public List<string> UnlockedArchetypes { get; set; } = new(); // Neue Talent-Archetypen
        public List<string> UnlockedMutators { get; set; } = new(); // Mutators (z.B. andere Econ-Regeln)
        public List<string> UnlockedCosmetics { get; set; } = new(); // Kosmetische Auren / UI / Titles
        
        // Prestige-Talent-Slots (separat von normalen Talent Points)
        public int PrestigeTalentPoints { get; set; } = 0;
        
        /// <summary>
        /// Prüft ob Spieler Prestige machen kann (Level 40 erreicht)
        /// </summary>
        public bool CanPrestige(int currentLevel)
        {
            return currentLevel >= LevelSystem.MaxHeroLevel && PrestigeLevel >= 0;
        }
        
        /// <summary>
        /// Führt Prestige durch (Reset auf Level 1, behält Prestige-Unlocks)
        /// </summary>
        public void PerformPrestige()
        {
            PrestigeLevel++;
            TotalPrestigePoints += PrestigeLevel; // Mehr Points für höhere Prestige-Level
        }
    }
}
