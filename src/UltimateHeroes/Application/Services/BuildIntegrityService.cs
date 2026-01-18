using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Builds;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Build Integrity (Diminishing Returns, Anti-Toxic Detection)
    /// </summary>
    public class BuildIntegrityService : IBuildIntegrityService
    {
        private readonly ISkillService _skillService;
        
        // Diminishing Returns Konfiguration
        private readonly Dictionary<SkillTag, DiminishingReturnConfig> _diminishingReturns = new()
        {
            { SkillTag.CrowdControl, new DiminishingReturnConfig { BaseReduction = 0.5f, MaxStacks = 3 } },
            { SkillTag.Stealth, new DiminishingReturnConfig { BaseReduction = 0.3f, MaxStacks = 2 } },
            { SkillTag.Mobility, new DiminishingReturnConfig { BaseReduction = 0.2f, MaxStacks = 3 } }
        };
        
        public BuildIntegrityService(ISkillService skillService)
        {
            _skillService = skillService;
        }
        
        public float ApplyDiminishingReturns(string skillId, float baseValue, int stackCount)
        {
            var skill = _skillService.GetSkill(skillId);
            if (skill == null) return baseValue;
            
            // Finde relevantes Tag
            SkillTag? relevantTag = null;
            foreach (var tag in skill.Tags)
            {
                if (_diminishingReturns.ContainsKey(tag))
                {
                    relevantTag = tag;
                    break;
                }
            }
            
            if (relevantTag == null) return baseValue;
            
            var config = _diminishingReturns[relevantTag.Value];
            if (stackCount <= 1) return baseValue;
            
            // Diminishing Returns: Jeder Stack reduziert die Effektivität
            float reduction = config.BaseReduction * (stackCount - 1);
            reduction = System.Math.Min(reduction, 0.8f); // Max 80% Reduction
            
            return baseValue * (1f - reduction);
        }
        
        public bool HasDiminishingReturns(SkillTag tag)
        {
            return _diminishingReturns.ContainsKey(tag);
        }
        
        public BuildIntegrityResult ValidateBuildIntegrity(Build build)
        {
            var result = new BuildIntegrityResult();
            
            if (build == null || !build.IsValid)
            {
                result.IsValid = false;
                result.Errors.Add("Build is invalid");
                return result;
            }
            
            // Prüfe auf Toxic Builds
            if (IsToxicBuild(build))
            {
                result.Warnings.Add("This build may be considered toxic (too many CC/Stealth skills)");
            }
            
            // Prüfe auf Camping Builds
            if (IsCampingBuild(build))
            {
                result.Warnings.Add("This build may encourage camping (too many defensive skills)");
            }
            
            // Prüfe Diminishing Returns
            var skillTags = new Dictionary<SkillTag, int>();
            foreach (var skillId in build.SkillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null) continue;
                
                foreach (var tag in skill.Tags)
                {
                    if (HasDiminishingReturns(tag))
                    {
                        skillTags[tag] = skillTags.GetValueOrDefault(tag, 0) + 1;
                    }
                }
            }
            
            foreach (var kvp in skillTags)
            {
                if (kvp.Value > 1)
                {
                    var config = _diminishingReturns[kvp.Key];
                    if (kvp.Value > config.MaxStacks)
                    {
                        result.Warnings.Add($"Multiple {kvp.Key} skills will have diminishing returns");
                    }
                }
            }
            
            return result;
        }
        
        public bool IsToxicBuild(Build build)
        {
            if (build == null) return false;
            
            int ccCount = 0;
            int stealthCount = 0;
            
            foreach (var skillId in build.SkillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null) continue;
                
                if (skill.Tags.Contains(SkillTag.CrowdControl)) ccCount++;
                if (skill.Tags.Contains(SkillTag.Stealth)) stealthCount++;
            }
            
            // Toxic: 3+ CC Skills oder 2+ Stealth Skills
            return ccCount >= 3 || stealthCount >= 2;
        }
        
        public bool IsCampingBuild(Build build)
        {
            if (build == null) return false;
            
            int defensiveCount = 0;
            int mobilityCount = 0;
            
            foreach (var skillId in build.SkillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null) continue;
                
                if (skill.Tags.Contains(SkillTag.Defense)) defensiveCount++;
                if (!skill.Tags.Contains(SkillTag.Mobility)) mobilityCount++; // Fehlende Mobility = Camping
            }
            
            // Camping: 3+ Defensive Skills und keine Mobility
            return defensiveCount >= 3 && mobilityCount == build.SkillIds.Count;
        }
        
        private class DiminishingReturnConfig
        {
            public float BaseReduction { get; set; } = 0.5f; // 50% Reduction pro Stack
            public int MaxStacks { get; set; } = 3; // Max 3 Stacks
        }
    }
}
