using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Domain.Builds.Validation
{
    /// <summary>
    /// Validiert Builds basierend auf Power Budget und Rules Engine (verschoben von Application nach Domain)
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
        public ValidationResult ValidateBuild(
            IHero heroCore,
            List<ISkill> activeSkills,
            ISkill? ultimateSkill,
            List<ISkill> passiveSkills,
            BuildSlotLimits slotLimits)
        {
            var errors = new List<string>();
            var warnings = new List<string>();
            
            // Check Hero Core
            if (heroCore == null)
            {
                errors.Add("Hero Core is required");
                return new ValidationResult { IsValid = false, Errors = errors, Warnings = warnings };
            }
            
            // Check Active Skills
            if (activeSkills.Count > slotLimits.TotalActiveSlots)
            {
                errors.Add($"Too many active skills: {activeSkills.Count} (max {slotLimits.TotalActiveSlots})");
            }
            
            // Check Ultimate Skills
            if (ultimateSkill != null && slotLimits.TotalUltimateSlots == 0)
            {
                errors.Add("Ultimate skill slot not unlocked");
            }
            
            if (ultimateSkill != null && slotLimits.TotalUltimateSlots > 0 && !(ultimateSkill is IUltimateSkill))
            {
                errors.Add("Ultimate skill must be of type Ultimate");
            }
            
            // Check Passive Skills
            if (passiveSkills.Count > slotLimits.TotalPassiveSlots)
            {
                errors.Add($"Too many passive skills: {passiveSkills.Count} (max {slotLimits.TotalPassiveSlots})");
            }
            
            // Check Power Budget
            var totalPowerWeight = heroCore.PowerWeight +
                                  activeSkills.Sum(s => s?.PowerWeight ?? 0) +
                                  (ultimateSkill?.PowerWeight ?? 0) +
                                  passiveSkills.Sum(s => s?.PowerWeight ?? 0);
            
            var maxPowerBudget = BasePowerBudget + (slotLimits.TotalActiveSlots - 3) * 10 + 
                                 (slotLimits.TotalUltimateSlots - 1) * 15 + 
                                 (slotLimits.TotalPassiveSlots - 2) * 5;
            
            if (totalPowerWeight > maxPowerBudget)
            {
                errors.Add($"Power budget exceeded: {totalPowerWeight}/{maxPowerBudget}");
            }
            
            // Check Tag-based Limits
            var allSkills = activeSkills.Cast<ISkill>().ToList();
            if (ultimateSkill != null) allSkills.Add(ultimateSkill);
            allSkills.AddRange(passiveSkills);
            
            foreach (var tagLimit in _tagLimits)
            {
                var tagCount = allSkills.Count(s => s.Tags.Contains(tagLimit.Key));
                if (tagCount > tagLimit.Value)
                {
                    warnings.Add($"Too many {tagLimit.Key} skills: {tagCount} (recommended max {tagLimit.Value})");
                }
            }
            
            // Check Combination Rules
            var hasCrowdControl = allSkills.Any(s => s.Tags.Contains(SkillTag.CrowdControl));
            var hasStealth = allSkills.Any(s => s.Tags.Contains(SkillTag.Stealth));
            
            if (hasCrowdControl && hasStealth)
            {
                warnings.Add("CC + Stealth combination may have increased cooldowns");
            }
            
            var damageSkillCount = allSkills.Count(s => s.Tags.Contains(SkillTag.Damage));
            if (damageSkillCount >= 3)
            {
                warnings.Add("High damage skill count may consume excessive power");
            }
            
            var hasMobility = allSkills.Any(s => s.Tags.Contains(SkillTag.Mobility));
            if (hasMobility && hasStealth)
            {
                warnings.Add("Mobility + Stealth: Synergy bonus detected");
            }
            
            var hasSupport = allSkills.Any(s => s.Tags.Contains(SkillTag.Support));
            var hasDefense = allSkills.Any(s => s.Tags.Contains(SkillTag.Defense));
            if (hasSupport && hasDefense)
            {
                warnings.Add("Support + Defense: Tank build detected");
            }
            
            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors,
                Warnings = warnings
            };
        }
    }
    
    /// <summary>
    /// Validation Result DTO
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
