using System.Collections.Generic;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository für Skill Mastery Daten
    /// </summary>
    public interface IMasteryRepository
    {
        SkillMastery? GetSkillMastery(string steamId, string skillId);
        List<SkillMastery> GetPlayerMasteries(string steamId);
        void SaveSkillMastery(SkillMastery mastery);
    }
}
