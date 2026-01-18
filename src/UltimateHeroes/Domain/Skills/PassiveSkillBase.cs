using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Domain.Skills
{
    /// <summary>
    /// Basis-Klasse für Passive Skills
    /// </summary>
    public abstract class PassiveSkillBase : SkillBase, IPassiveSkill
    {
        public override SkillType Type => SkillType.Passive;
        
        public abstract void OnPlayerSpawn(CCSPlayerController player);
        public abstract void OnPlayerHurt(CCSPlayerController player, int damage);
        public abstract void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim);
    }
}
