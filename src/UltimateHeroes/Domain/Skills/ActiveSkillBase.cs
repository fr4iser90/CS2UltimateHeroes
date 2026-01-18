using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Domain.Skills
{
    /// <summary>
    /// Basis-Klasse für Active Skills
    /// </summary>
    public abstract class ActiveSkillBase : SkillBase, IActiveSkill
    {
        public override SkillType Type => SkillType.Active;
        
        public abstract float Cooldown { get; }
        public abstract void Activate(CCSPlayerController player);
    }
}
