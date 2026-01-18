using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Skill Mastery Management
    /// </summary>
    public interface IMasteryService
    {
        // Mastery Tracking
        void TrackSkillUse(string steamId, string skillId);
        void TrackSkillKill(string steamId, string skillId);
        void TrackSkillDamage(string steamId, string skillId, float damage);
        void TrackSkillEscape(string steamId, string skillId);
        
        // Mastery Queries
        SkillMastery? GetSkillMastery(string steamId, string skillId);
        List<SkillMastery> GetPlayerMasteries(string steamId);
        int GetMasteryLevel(string steamId, string skillId);
        
        // Mastery Rewards
        List<string> GetUnlockedRewards(string steamId, string skillId);
        bool HasReward(string steamId, string skillId, string rewardId);
    }
}
