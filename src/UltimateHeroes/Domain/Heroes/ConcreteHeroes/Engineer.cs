using System.Collections.Generic;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Domain.Skills.ConcreteSkills;

namespace UltimateHeroes.Domain.Heroes.ConcreteHeroes
{
    /// <summary>
    /// Engineer - Tech Hero mit Utility-Fokus
    /// </summary>
    public class Engineer : HeroCore
    {
        public override string Id => "engineer";
        public override string DisplayName => "Engineer";
        public override string Description => "Ein technischer Hero mit starken Utility-Fähigkeiten";
        public override int PowerWeight => 30;
        
        private readonly List<IPassiveSkill> _passiveSkills = new();
        
        public override List<IPassiveSkill> PassiveSkills => _passiveSkills;
        
        public override HeroIdentity Identity { get; }
        
        public Engineer()
        {
            // Initialize Passive Skills
            _passiveSkills.Add(new MiniSentryPassive());
            _passiveSkills.Add(new UtilityCooldownReductionPassive());
            
            // Initialize Hero Identity
            Identity = new HeroIdentity
            {
                TagModifiers = new Dictionary<SkillTag, float>
                {
                    { SkillTag.Utility, 1.25f },    // +25% Range
                    { SkillTag.Area, 1.1f }        // +10% Area
                },
                CooldownReduction = new Dictionary<SkillType, float>(),
                SpecialBonuses = new Dictionary<string, float>
                {
                    { "SentryDamage", 1.3f }       // +30% Sentry Damage
                }
            };
        }
    }
}
