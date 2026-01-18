using System;

namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// XP System - Verwaltet XP Quellen und Level Calculation
    /// </summary>
    public class XpSystem
    {
        // XP Sources
        public const float XpPerKill = 25f; // Base 20-30, hier 25
        public const float XpPerHeadshot = 5f; // Bonus
        public const float XpPerAssist = 3f;
        public const float XpPerObjective = 40f; // 30-60, hier 40
        public const float XpPerRoundWin = 30f;
        public const float XpPerFlashAssist = 5f;
        public const float XpPerMatchCompletion = 75f; // 50-100, hier 75
        public const float WinBonusMultiplier = 0.15f; // +15% XP für Win
        
        // Modus-Multiplikatoren
        public const float CasualMultiplier = 0.8f;
        public const float RankedMultiplier = 1.0f;
        public const float DeathmatchMultiplier = 0.6f;
        public const float ShortMatchMultiplier = 0.7f;
        public const float LongMatchMultiplier = 1.1f;
        
        /// <summary>
        /// Berechnet benötigtes XP für ein Level (Level 1-40)
        /// Soft-exponential: Level 1-10 schnell, 11-25 mittel, 26-40 langsamer
        /// </summary>
        public static float GetXpForLevel(int level)
        {
            if (level <= 1) return 100f;
            if (level > LevelSystem.MaxHeroLevel) return float.MaxValue; // Prestige-Vorbereitung
            
            // Soft-exponential: 100 * (1.15 ^ (level - 1)) für Level 1-40
            // Sanfter als vorher (1.2), damit Level 40 erreichbar bleibt
            return 100f * (float)Math.Pow(1.15, level - 1);
        }
        
        /// <summary>
        /// Berechnet Level basierend auf totalem XP
        /// </summary>
        public static int GetLevelFromXp(float totalXp)
        {
            int level = 1;
            float xp = 0f;
            
            while (xp < totalXp)
            {
                float xpNeeded = GetXpForLevel(level);
                if (xp + xpNeeded > totalXp) break;
                xp += xpNeeded;
                level++;
            }
            
            return level;
        }
        
        /// <summary>
        /// Berechnet XP Progress für aktuelles Level (0.0 - 1.0)
        /// </summary>
        public static float GetXpProgress(float currentXp, int currentLevel)
        {
            float xpForCurrentLevel = GetXpForLevel(currentLevel);
            float xpInCurrentLevel = currentXp;
            
            // Subtract XP from previous levels
            for (int i = 1; i < currentLevel; i++)
            {
                xpInCurrentLevel -= GetXpForLevel(i);
            }
            
            return Math.Max(0f, Math.Min(1f, xpInCurrentLevel / xpForCurrentLevel));
        }
    }
}
