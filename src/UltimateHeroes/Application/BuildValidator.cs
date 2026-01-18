using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Application
{
    /// <summary>
    /// Validiert Builds basierend auf Power Budget und Rules Engine
    /// </summary>
    public class BuildValidator
    {
        private const int MaxPowerBudget = 100;
        private const int HeroCorePowerWeight = 30;
        private const int MaxSkillSlots = 3;
        
        /// <summary>
        /// Validiert ob ein Build gültig ist
        /// </summary>
        public ValidationResult ValidateBuild(IHero heroCore, List<ISkill> skills)
        {
            var result = new ValidationResult();
            
            // Power Budget Check
            var totalPower = heroCore.PowerWeight + skills.Sum(s => s.PowerWeight);
            if (totalPower > MaxPowerBudget)
            {
                result.IsValid = false;
                result.Errors.Add($"Power Budget exceeded: {totalPower}/{MaxPowerBudget}");
                return result;
            }
            
            // Skill Slots Check
            if (skills.Count > MaxSkillSlots)
            {
                result.IsValid = false;
                result.Errors.Add($"Too many skills: {skills.Count}/{MaxSkillSlots}");
                return result;
            }
            
            // Rules Engine Checks
            var tags = skills.SelectMany(s => s.Tags).ToList();
            
            // Max 1 Ultimate
            var ultimateCount = skills.Count(s => s.Type == SkillType.Ultimate);
            if (ultimateCount > 1)
            {
                result.IsValid = false;
                result.Errors.Add("Only 1 Ultimate skill allowed");
                return result;
            }
            
            // Max 2 Mobility Skills
            var mobilityCount = skills.Count(s => s.Tags.Contains(SkillTag.Mobility));
            if (mobilityCount > 2)
            {
                result.IsValid = false;
                result.Errors.Add("Only 2 Mobility skills allowed");
                return result;
            }
            
            // CC + Stealth = Cooldown-Malus (warnung, nicht blockieren)
            if (tags.Contains(SkillTag.CrowdControl) && tags.Contains(SkillTag.Stealth))
            {
                result.Warnings.Add("CC + Stealth combination: +25% Cooldown-Malus");
            }
            
            result.IsValid = true;
            return result;
        }
    }
    
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
