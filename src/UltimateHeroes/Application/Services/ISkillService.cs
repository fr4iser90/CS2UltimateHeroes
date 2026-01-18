using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Infrastructure.Cooldown;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für Skill Management
    /// </summary>
    public interface ISkillService
    {
        void RegisterSkill(ISkill skill);
        void RegisterSkills(IEnumerable<ISkill> skills);
        
        ISkill? GetSkill(string skillId);
        List<ISkill> GetAllSkills();
        List<ISkill> GetSkillsByType(SkillType type);
        List<ISkill> GetSkillsByTag(SkillTag tag);
        bool SkillExists(string skillId);
        
        bool CanActivateSkill(string steamId, string skillId);
        void ActivateSkill(string steamId, string skillId, CCSPlayerController player);
        
        float GetSkillCooldown(string steamId, string skillId);
        bool IsSkillReady(string steamId, string skillId);
        void TrackSkillDamage(string steamId, string skillId, float damage);
        void ReduceCooldown(string steamId, string skillId, float reductionPercent);
    }
}
