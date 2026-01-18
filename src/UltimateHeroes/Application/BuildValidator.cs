using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Application
{
    /// <summary>
    /// Validiert Builds basierend auf Power Budget und Rules Engine (erweitert mit Tag-based Rules)
    /// </summary>
    public class BuildValidator
    {
        private const int MaxPowerBudget = 100;
        private const int HeroCorePowerWeight = 30;
        private const int MaxSkillSlots = 3;
        
        // Tag-based Limits
        private readonly Dictionary<SkillTag, int> _tagLimits = new()
        {
            { SkillTag.Mobility, 2 },
            { SkillTag.CrowdControl, 2 },
            { SkillTag.Stealth, 1 },
            { SkillTag.Damage, 3 },
            { SkillTag.Support, 3 },
            { SkillTag.Defense, 2 }
        };
        
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
            
            // Tag-based Limits
            foreach (var kvp in _tagLimits)
            {
                var tag = kvp.Key;
                var limit = kvp.Value;
                var count = skills.Count(s => s.Tags.Contains(tag));
                
                if (count > limit)
                {
                    result.IsValid = false;
                    result.Errors.Add($"Too many {tag} skills: {count}/{limit}");
                    return result;
                }
            }
            
            // Combination Rules
            ValidateCombinations(skills, result);
            
            result.IsValid = true;
            return result;
        }
        
        /// <summary>
        /// Validiert Skill-Kombinationen
        /// </summary>
        private void ValidateCombinations(List<ISkill> skills, ValidationResult result)
        {
            var tags = skills.SelectMany(s => s.Tags).Distinct().ToList();
            
            // CC + Stealth = Cooldown-Malus (Warning)
            if (tags.Contains(SkillTag.CrowdControl) && tags.Contains(SkillTag.Stealth))
            {
                result.Warnings.Add("CC + Stealth combination: +25% Cooldown penalty");
            }
            
            // 3+ Damage Skills = Power Budget Warning
            var damageCount = skills.Count(s => s.Tags.Contains(SkillTag.Damage));
            if (damageCount >= 3)
            {
                result.Warnings.Add("3+ Damage skills: High power consumption");
            }
            
            // Mobility + Stealth = Synergy Bonus (Info)
            if (tags.Contains(SkillTag.Mobility) && tags.Contains(SkillTag.Stealth))
            {
                result.Warnings.Add("Mobility + Stealth: Synergy bonus active");
            }
            
            // Support + Defense = Tank Build (Info)
            if (tags.Contains(SkillTag.Support) && tags.Contains(SkillTag.Defense))
            {
                result.Warnings.Add("Support + Defense: Tank build detected");
            }
        }
    }
    
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
