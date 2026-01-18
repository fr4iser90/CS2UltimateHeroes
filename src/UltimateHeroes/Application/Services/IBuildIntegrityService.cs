using System.Collections.Generic;
using UltimateHeroes.Domain.Builds;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Build Integrity (Diminishing Returns, Anti-Toxic Detection)
    /// </summary>
    public interface IBuildIntegrityService
    {
        // Diminishing Returns
        float ApplyDiminishingReturns(string skillId, float baseValue, int stackCount);
        bool HasDiminishingReturns(SkillTag tag);
        
        // Build Validation
        BuildIntegrityResult ValidateBuildIntegrity(Build build);
        bool IsToxicBuild(Build build);
        bool IsCampingBuild(Build build);
    }
    
    public class BuildIntegrityResult
    {
        public bool IsValid { get; set; } = true;
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
