using System.Collections.Generic;

namespace UltimateHeroes.Domain.Skills
{
    /// <summary>
    /// Basis-Klasse für alle Skills
    /// </summary>
    public abstract class SkillBase : ISkill
    {
        public abstract string Id { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract SkillType Type { get; }
        public abstract int PowerWeight { get; }
        public abstract List<SkillTag> Tags { get; }
        public abstract int MaxLevel { get; }
        
        /// <summary>
        /// Aktuelles Level des Skills (1-MaxLevel)
        /// </summary>
        public int CurrentLevel { get; set; } = 1;
        
        /// <summary>
        /// Berechnet Level-Multiplier (+10% pro Level)
        /// </summary>
        protected float GetLevelMultiplier()
        {
            return 1.0f + (CurrentLevel - 1) * 0.1f; // +10% pro Level
        }
    }
}
