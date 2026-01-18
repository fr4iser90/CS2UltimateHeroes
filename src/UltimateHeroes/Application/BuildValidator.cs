using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Builds;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Application
{
    /// <summary>
    /// Validiert Builds basierend auf Power Budget und Rules Engine (erweitert mit Tag-based Rules)
    /// </summary>
    public class BuildValidator
    {
        private const int BasePowerBudget = 150; // Base für 6 Skills (3 Active + 1 Ultimate + 2 Passive)
        private const int HeroCorePowerWeight = 30;
        
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
        /// Validiert ob ein Build gültig ist (separate Slots mit dynamischen Limits)
        /// </summary>
        public ValidationResult ValidateBuild(IHero heroCore, List<ISkill> activeSkills, ISkill? ultimateSkill, List<ISkill> passiveSkills, BuildSlotLimits? slotLimits = null)
        {
            var result = new ValidationResult();
            
            // Use provided slot limits or default
            var limits = slotLimits ?? new BuildSlotLimits();
            
            // Active Slots Check
            if (activeSkills.Count > limits.TotalActiveSlots)
            {
                result.IsValid = false;
                result.Errors.Add($"Too many active skills: {activeSkills.Count}/{limits.TotalActiveSlots}");
                return result;
            }
            
            // Ultimate Slot Check
            var ultimateCount = ultimateSkill != null ? 1 : 0;
            if (ultimateCount > limits.TotalUltimateSlots)
            {
                result.IsValid = false;
                result.Errors.Add($"Too many ultimate skills: {ultimateCount}/{limits.TotalUltimateSlots}");
                return result;
            }
            
            if (ultimateSkill != null && ultimateSkill.Type != SkillType.Ultimate)
            {
                result.IsValid = false;
                result.Errors.Add("Ultimate slot can only contain Ultimate skills");
                return result;
            }
            
            // Passive Slots Check
            if (passiveSkills.Count > limits.TotalPassiveSlots)
            {
                result.IsValid = false;
                result.Errors.Add($"Too many passive skills: {passiveSkills.Count}/{limits.TotalPassiveSlots}");
                return result;
            }
            
            // Prüfe dass Active Skills wirklich Active sind
            foreach (var skill in activeSkills)
            {
                if (skill.Type != SkillType.Active)
                {
                    result.IsValid = false;
                    result.Errors.Add($"Active slot can only contain Active skills, found: {skill.Type}");
                    return result;
                }
            }
            
            // Prüfe dass Passive Skills wirklich Passive sind
            foreach (var skill in passiveSkills)
            {
                if (skill.Type != SkillType.Passive)
                {
                    result.IsValid = false;
                    result.Errors.Add($"Passive slot can only contain Passive skills, found: {skill.Type}");
                    return result;
                }
            }
            
            // Power Budget Check (dynamisch basierend auf Slots)
            var totalPower = heroCore.PowerWeight + 
                           activeSkills.Sum(s => s.PowerWeight) + 
                           (ultimateSkill?.PowerWeight ?? 0) + 
                           passiveSkills.Sum(s => s.PowerWeight);
            
            // Calculate dynamic power budget based on slots
            var powerBudget = CalculatePowerBudget(limits);
            if (totalPower > powerBudget)
            {
                result.IsValid = false;
                result.Errors.Add($"Power Budget exceeded: {totalPower}/{powerBudget}");
                return result;
            }
            
            // Alle Skills für Tag-based Limits
            var allSkills = new List<ISkill>(activeSkills);
            if (ultimateSkill != null) allSkills.Add(ultimateSkill);
            allSkills.AddRange(passiveSkills);
            
            // Tag-based Limits (für alle Skills zusammen)
            foreach (var kvp in _tagLimits)
            {
                var tag = kvp.Key;
                var limit = kvp.Value;
                var count = allSkills.Count(s => s.Tags.Contains(tag));
                
                if (count > limit)
                {
                    result.IsValid = false;
                    result.Errors.Add($"Too many {tag} skills: {count}/{limit}");
                    return result;
                }
            }
            
            // Combination Rules
            ValidateCombinations(allSkills, result);
            
            result.IsValid = true;
            return result;
        }
        
        /// <summary>
        /// Berechnet dynamisches Power Budget basierend auf Slot-Limits
        /// </summary>
        private int CalculatePowerBudget(BuildSlotLimits limits)
        {
            var budget = BasePowerBudget;
            
            // +25 pro zusätzlichem Active Slot
            if (limits.TotalActiveSlots > 3)
            {
                budget += (limits.TotalActiveSlots - 3) * 25;
            }
            
            // +40 pro zusätzlichem Ultimate Slot
            if (limits.TotalUltimateSlots > 1)
            {
                budget += (limits.TotalUltimateSlots - 1) * 40;
            }
            
            // +15 pro zusätzlichem Passive Slot
            if (limits.TotalPassiveSlots > 2)
            {
                budget += (limits.TotalPassiveSlots - 2) * 15;
            }
            
            return budget;
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
