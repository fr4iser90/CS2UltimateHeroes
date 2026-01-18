using System.Collections.Generic;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Domain.Skills.ConcreteSkills;

namespace UltimateHeroes.Domain.Heroes.ConcreteHeroes
{
    /// <summary>
    /// Vanguard - Tank Hero mit Defense-Fokus
    /// </summary>
    public class Vanguard : HeroCore
    {
        public override string Id => "vanguard";
        public override string DisplayName => "Vanguard";
        public override string Description => "Ein robuster Tank-Hero mit starken Defense-Fähigkeiten";
        public override int PowerWeight => 30;
        
        private readonly List<IPassiveSkill> _passiveSkills = new();
        
        public override List<IPassiveSkill> PassiveSkills => _passiveSkills;
        
        public override HeroIdentity Identity { get; }
        
        public Vanguard()
        {
            // Initialize Passive Skills
            _passiveSkills.Add(new ArmorPerKillPassive());
            
            // Initialize Hero Identity
            Identity = new HeroIdentity
            {
                TagModifiers = new Dictionary<SkillTag, float>
                {
                    { SkillTag.Defense, 1.1f },  // +10% Defense Skills
                    { SkillTag.Support, 1.05f }  // +5% Support Skills
                },
                CooldownReduction = new Dictionary<SkillType, float>(),
                SpecialBonuses = new Dictionary<string, float>
                {
                    { "ShieldDuration", 1.2f }  // +20% Shield Duration
                }
            };
        }
    }
}
