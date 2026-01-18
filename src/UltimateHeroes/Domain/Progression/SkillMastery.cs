using System.Collections.Generic;

namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// Skill Mastery - Tracking und Rewards für Skill-Nutzung
    /// </summary>
    public class SkillMastery
    {
        public string SteamId { get; set; } = string.Empty;
        public string SkillId { get; set; } = string.Empty;
        
        // Mastery Stats
        public int Kills { get; set; } = 0;
        public int Uses { get; set; } = 0;
        public float TotalDamage { get; set; } = 0f;
        public int Escapes { get; set; } = 0; // Für Mobility Skills
        
        // Mastery Level
        public int MasteryLevel { get; set; } = 0; // 0-5
        
        // Mastery Rewards
        public List<string> UnlockedRewards { get; set; } = new();
        
        /// <summary>
        /// Berechnet den Mastery Level basierend auf Stats
        /// </summary>
        public int CalculateMasteryLevel()
        {
            // Level 1: 100 Kills oder 200 Uses
            // Level 2: 500 Kills oder 1000 Uses
            // Level 3: 1000 Kills oder 2000 Uses
            // Level 4: 2500 Kills oder 5000 Uses
            // Level 5: 5000 Kills oder 10000 Uses
            
            if (Kills >= 5000 || Uses >= 10000) return 5;
            if (Kills >= 2500 || Uses >= 5000) return 4;
            if (Kills >= 1000 || Uses >= 2000) return 3;
            if (Kills >= 500 || Uses >= 1000) return 2;
            if (Kills >= 100 || Uses >= 200) return 1;
            return 0;
        }
        
        /// <summary>
        /// Aktualisiert den Mastery Level und gibt zurück, ob ein Level-Up stattgefunden hat
        /// </summary>
        public bool UpdateMasteryLevel()
        {
            int newLevel = CalculateMasteryLevel();
            if (newLevel > MasteryLevel)
            {
                int oldLevel = MasteryLevel;
                MasteryLevel = newLevel;
                UnlockMasteryRewards(newLevel);
                return true; // Level-Up!
            }
            return false;
        }
        
        /// <summary>
        /// Entsperrt Mastery Rewards für ein bestimmtes Level
        /// </summary>
        private void UnlockMasteryRewards(int level)
        {
            // Level 1: Cosmetic Trail
            if (level >= 1 && !UnlockedRewards.Contains("cosmetic_trail"))
            {
                UnlockedRewards.Add("cosmetic_trail");
            }
            
            // Level 2: Alt Animation
            if (level >= 2 && !UnlockedRewards.Contains("alt_animation"))
            {
                UnlockedRewards.Add("alt_animation");
            }
            
            // Level 3: Talent Modifier (+1 Talent Point)
            if (level >= 3 && !UnlockedRewards.Contains("talent_modifier"))
            {
                UnlockedRewards.Add("talent_modifier");
            }
            
            // Level 4: Mastery Badge
            if (level >= 4 && !UnlockedRewards.Contains("mastery_badge"))
            {
                UnlockedRewards.Add("mastery_badge");
            }
            
            // Level 5: Mastery Title
            if (level >= 5 && !UnlockedRewards.Contains("mastery_title"))
            {
                UnlockedRewards.Add("mastery_title");
            }
        }
        
        /// <summary>
        /// Fügt einen Kill hinzu
        /// </summary>
        public void AddKill()
        {
            Kills++;
        }
        
        /// <summary>
        /// Fügt eine Skill-Nutzung hinzu
        /// </summary>
        public void AddUse()
        {
            Uses++;
        }
        
        /// <summary>
        /// Fügt Damage hinzu
        /// </summary>
        public void AddDamage(float damage)
        {
            TotalDamage += damage;
        }
        
        /// <summary>
        /// Fügt einen Escape hinzu (für Mobility Skills)
        /// </summary>
        public void AddEscape()
        {
            Escapes++;
        }
    }
}
