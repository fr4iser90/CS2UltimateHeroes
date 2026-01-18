using System;
using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Builds.Validation;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Domain.Builds
{
    /// <summary>
    /// Repräsentiert eine gespeicherte Build-Konfiguration eines Spielers
    /// </summary>
    public class Build
    {
        // Primary Key
        public string SteamId { get; set; } = string.Empty;
        public int BuildSlot { get; set; } // 1-5
        
        // Build Configuration
        public string HeroCoreId { get; set; } = string.Empty;
        
        // Separate Skill Slots
        public List<string> ActiveSkillIds { get; set; } = new(); // Max 3 Active Skills
        public string? UltimateSkillId { get; set; } = null; // Max 1 Ultimate Skill (optional)
        public List<string> PassiveSkillIds { get; set; } = new(); // Max 2 Passive Skills
        
        /// <summary>
        /// Helper Property: Alle Skill IDs kombiniert (für Kompatibilität)
        /// </summary>
        public List<string> SkillIds
        {
            get
            {
                var allIds = new List<string>(ActiveSkillIds);
                if (!string.IsNullOrEmpty(UltimateSkillId))
                {
                    allIds.Add(UltimateSkillId);
                }
                allIds.AddRange(PassiveSkillIds);
                return allIds;
            }
        }
        
        // Metadata
        public string BuildName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUsedAt { get; set; }
        
        // Validation
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
        
        /// <summary>
        /// Validiert den Build mit Hero Core und Skills (separate Slots)
        /// </summary>
        public ValidationResult Validate(IHero heroCore, List<ISkill> activeSkills, ISkill? ultimateSkill, List<ISkill> passiveSkills, BuildSlotLimits slotLimits)
        {
            var validator = new BuildValidator();
            var result = validator.ValidateBuild(heroCore, activeSkills, ultimateSkill, passiveSkills, slotLimits);
            
            IsValid = result.IsValid;
            ValidationErrors = result.Errors;
            
            return result;
        }
        
        /// <summary>
        /// Prüft ob der Build leer ist
        /// </summary>
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(HeroCoreId);
        }
        
        /// <summary>
        /// Prüft ob der Build aktiviert werden kann
        /// </summary>
        public bool CanActivate()
        {
            return IsValid && !IsEmpty();
        }
        
        /// <summary>
        /// Berechnet den totalen Power Weight des Builds (separate Slots)
        /// </summary>
        public int GetPowerWeight(IHero heroCore, List<ISkill> activeSkills, ISkill? ultimateSkill, List<ISkill> passiveSkills)
        {
            if (heroCore == null) return 0;
            return heroCore.PowerWeight + 
                   activeSkills.Sum(s => s?.PowerWeight ?? 0) + 
                   (ultimateSkill?.PowerWeight ?? 0) + 
                   passiveSkills.Sum(s => s?.PowerWeight ?? 0);
        }
        
        /// <summary>
        /// Markiert den Build als aktiv und updated LastUsedAt
        /// </summary>
        public void MarkAsActive()
        {
            IsActive = true;
            LastUsedAt = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Markiert den Build als inaktiv
        /// </summary>
        public void MarkAsInactive()
        {
            IsActive = false;
        }
    }
}
