using System.Collections.Generic;
using UltimateHeroes.Domain.Talents;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Talent-Verwaltung
    /// </summary>
    public interface ITalentService
    {
        // Talent Points
        void AwardTalentPoints(string steamId, int points);
        int GetAvailablePoints(string steamId);
        
        // Talent Management
        bool UnlockTalent(string steamId, string talentId);
        bool LevelUpTalent(string steamId, string talentId);
        bool CanUnlockTalent(string steamId, string talentId);
        bool CanLevelUpTalent(string steamId, string talentId);
        List<TalentNode> GetUnlockableTalents(string steamId, TalentTreeType treeType);
        List<TalentNode> GetUnlockedTalents(string steamId, TalentTreeType treeType);
        
        // Talent Trees
        TalentTree GetTalentTree(TalentTreeType treeType);
        List<TalentTree> GetAllTalentTrees();
        
        // Talent Effects
        Dictionary<string, float> GetTalentModifiers(string steamId);
        int GetTalentLevel(string steamId, string talentId);
    }
}
