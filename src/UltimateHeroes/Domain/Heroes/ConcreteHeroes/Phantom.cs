using System.Collections.Generic;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Domain.Skills.ConcreteSkills;

namespace UltimateHeroes.Domain.Heroes.ConcreteHeroes
{
    /// <summary>
    /// Phantom - Stealth Hero mit Mobility-Fokus
    /// </summary>
    public class Phantom : HeroCore
    {
        public override string Id => "phantom";
        public override string DisplayName => "Phantom";
        public override string Description => "Ein geschickter Stealth-Hero mit starken Mobility-Fähigkeiten";
        public override int PowerWeight => 30;
        
        private readonly List<IPassiveSkill> _passiveSkills = new();
        
        public override List<IPassiveSkill> PassiveSkills => _passiveSkills;
        
        public override HeroIdentity Identity { get; }
        
        public Phantom()
        {
            // Initialize Passive Skills
            _passiveSkills.Add(new SilentFootstepsPassive());
            
            // Initialize Hero Identity
            Identity = new HeroIdentity
            {
                TagModifiers = new Dictionary<SkillTag, float>
                {
                    { SkillTag.Mobility, 0.9f },  // -10% Cooldown
                    { SkillTag.Stealth, 1.1f }     // +10% Stealth
                },
                CooldownReduction = new Dictionary<SkillType, float>
                {
                    { SkillType.Active, 0.1f }     // -10% Cooldown für Active Skills
                },
                SpecialBonuses = new Dictionary<string, float>
                {
                    { "BackstabDamage", 1.15f }    // +15% Backstab Damage
                }
            };
        }
    }
}
