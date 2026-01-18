using System;

namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// XP System - Verwaltet XP Quellen und Level Calculation
    /// </summary>
    public class XpSystem
    {
        // XP Sources
        public const float XpPerKill = 10f;
        public const float XpPerHeadshot = 5f;
        public const float XpPerAssist = 3f;
        public const float XpPerObjective = 20f;
        public const float XpPerRoundWin = 30f;
        public const float XpPerFlashAssist = 5f;
        
        /// <summary>
        /// Berechnet benötigtes XP für ein Level
        /// Exponential: 100 * (1.2 ^ (level - 1))
        /// </summary>
        public static float GetXpForLevel(int level)
        {
            if (level <= 1) return 100f;
            return 100f * (float)Math.Pow(1.2, level - 1);
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
