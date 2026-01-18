using System;
using System.Collections.Generic;

namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// Account Level - Meta-Progression über alle Heroes hinweg
    /// </summary>
    public class AccountLevel
    {
        public string SteamId { get; set; } = string.Empty;
        public int AccountLevel { get; set; } = 1;
        public float AccountXp { get; set; } = 0f;
        public float XpToNextLevel { get; set; } = 100f;
        
        // Account Unlocks
        public List<string> UnlockedTitles { get; set; } = new();
        public List<string> UnlockedCosmetics { get; set; } = new();
        public List<string> UnlockedArchetypes { get; set; } = new();
        
        /// <summary>
        /// Berechnet benötigtes XP für Account Level (1-∞)
        /// Level 1-50: Schnell (100-200 XP)
        /// Level 51-100: Mittel (200-500 XP)
        /// Level 101+: Langsam (500+ XP)
        /// </summary>
        public static float GetXpForAccountLevel(int level)
        {
            if (level <= 1) return 100f;
            
            if (level <= 50)
            {
                // Level 1-50: 100 + (level - 1) * 2 = 100-198 XP
                return 100f + (level - 1) * 2f;
            }
            else if (level <= 100)
            {
                // Level 51-100: 200 + (level - 50) * 6 = 200-500 XP
                return 200f + (level - 50) * 6f;
            }
            else
            {
                // Level 101+: 500 + (level - 100) * 10 = 500+ XP
                return 500f + (level - 100) * 10f;
            }
        }
        
        /// <summary>
        /// Berechnet Account Level basierend auf totalem Account XP
        /// </summary>
        public static int GetAccountLevelFromXp(float totalXp)
        {
            int level = 1;
            float xp = 0f;
            
            while (xp < totalXp)
            {
                float xpNeeded = GetXpForAccountLevel(level);
                if (xp + xpNeeded > totalXp) break;
                xp += xpNeeded;
                level++;
            }
            
            return level;
        }
        
        /// <summary>
        /// Berechnet XP Progress für aktuelles Account Level (0.0 - 1.0)
        /// </summary>
        public static float GetAccountXpProgress(float currentXp, int currentLevel)
        {
            float xpForCurrentLevel = GetXpForAccountLevel(currentLevel);
            float xpInCurrentLevel = currentXp;
            
            // Subtract XP from previous levels
            for (int i = 1; i < currentLevel; i++)
            {
                xpInCurrentLevel -= GetXpForAccountLevel(i);
            }
            
            return Math.Max(0f, Math.Min(1f, xpInCurrentLevel / xpForCurrentLevel));
        }
        
        /// <summary>
        /// Gibt aktuellen Title basierend auf Account Level zurück
        /// </summary>
        public static string GetTitleForLevel(int level)
        {
            return level switch
            {
                >= 500 => "Legend",
                >= 200 => "Master",
                >= 100 => "Veteran",
                >= 50 => "Experienced",
                >= 25 => "Novice",
                _ => "Rookie"
            };
        }
        
        /// <summary>
        /// Prüft ob Prestige freigeschaltet ist (Account Level 200+)
        /// </summary>
        public static bool IsPrestigeUnlocked(int accountLevel)
        {
            return accountLevel >= 200;
        }
    }
}
