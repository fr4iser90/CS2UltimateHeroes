using System.Collections.Generic;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Domain.Heroes
{
    /// <summary>
    /// Hero Identity Auras - Modifier für Skills basierend auf Hero Core
    /// </summary>
    public class HeroIdentity
    {
        /// <summary>
        /// Modifier für bestimmte Skill-Tags
        /// z.B. { SkillTag.Defense: 1.1f } = +10% auf Defense-Skills
        /// </summary>
        public Dictionary<SkillTag, float> TagModifiers { get; set; } = new();
        
        /// <summary>
        /// Cooldown-Reduction für bestimmte Skill-Typen
        /// </summary>
        public Dictionary<SkillType, float> CooldownReduction { get; set; } = new();
        
        /// <summary>
        /// Spezielle Bonuses
        /// </summary>
        public Dictionary<string, float> SpecialBonuses { get; set; } = new();
        
        /// <summary>
        /// Berechnet Modifier für einen Skill
        /// </summary>
        public float GetSkillModifier(ISkill skill)
        {
            float modifier = 1.0f;
            
            // Tag-Modifier anwenden
            foreach (var tag in skill.Tags)
            {
                if (TagModifiers.TryGetValue(tag, out var tagMod))
                {
                    modifier *= tagMod;
                }
            }
            
            return modifier;
        }
        
        /// <summary>
        /// Berechnet Cooldown-Reduction für einen Skill
        /// </summary>
        public float GetCooldownReduction(ISkill skill)
        {
            if (CooldownReduction.TryGetValue(skill.Type, out var reduction))
            {
                return reduction;
            }
            return 0f;
        }
    }
}
