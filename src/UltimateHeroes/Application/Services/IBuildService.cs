using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Application;
using UltimateHeroes.Domain.Builds;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für Build Management
    /// </summary>
    public interface IBuildService
    {
        Build CreateBuild(string steamId, int buildSlot, string heroId, 
            List<string> activeSkillIds, string? ultimateSkillId, List<string> passiveSkillIds, string buildName);
        ValidationResult ValidateBuild(string heroId, 
            List<string> activeSkillIds, string? ultimateSkillId, List<string> passiveSkillIds);
        
        Build? GetBuild(string steamId, int buildSlot);
        List<Build> GetPlayerBuilds(string steamId);
        void SaveBuild(Build build);
        void DeleteBuild(string steamId, int buildSlot);
        
        void ActivateBuild(string steamId, int buildSlot, CCSPlayerController player);
        Build? GetActiveBuild(string steamId);
        
        List<int> GetUnlockedSlots(string steamId);
        bool IsSlotUnlocked(string steamId, int slot);
    }
}
