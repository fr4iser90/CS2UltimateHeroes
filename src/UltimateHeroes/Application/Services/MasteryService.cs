using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Infrastructure.Database.Repositories;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Skill Mastery Management
    /// </summary>
    public class MasteryService : IMasteryService
    {
        private readonly IMasteryRepository _masteryRepository;
        
        public MasteryService(IMasteryRepository masteryRepository)
        {
            _masteryRepository = masteryRepository;
        }
        
        public void TrackSkillUse(string steamId, string skillId)
        {
            var mastery = GetOrCreateMastery(steamId, skillId);
            mastery.AddUse();
            
            var leveledUp = mastery.UpdateMasteryLevel();
            _masteryRepository.SaveSkillMastery(mastery);
            
            // Notify player if leveled up
            if (leveledUp)
            {
                NotifyMasteryLevelUp(steamId, skillId, mastery.MasteryLevel);
            }
        }
        
        public void TrackSkillKill(string steamId, string skillId)
        {
            var mastery = GetOrCreateMastery(steamId, skillId);
            mastery.AddKill();
            
            var leveledUp = mastery.UpdateMasteryLevel();
            _masteryRepository.SaveSkillMastery(mastery);
            
            // Notify player if leveled up
            if (leveledUp)
            {
                NotifyMasteryLevelUp(steamId, skillId, mastery.MasteryLevel);
            }
        }
        
        public void TrackSkillDamage(string steamId, string skillId, float damage)
        {
            var mastery = GetOrCreateMastery(steamId, skillId);
            mastery.AddDamage(damage);
            _masteryRepository.SaveSkillMastery(mastery);
        }
        
        public void TrackSkillEscape(string steamId, string skillId)
        {
            var mastery = GetOrCreateMastery(steamId, skillId);
            mastery.AddEscape();
            
            var leveledUp = mastery.UpdateMasteryLevel();
            _masteryRepository.SaveSkillMastery(mastery);
            
            // Notify player if leveled up
            if (leveledUp)
            {
                NotifyMasteryLevelUp(steamId, skillId, mastery.MasteryLevel);
            }
        }
        
        public SkillMastery? GetSkillMastery(string steamId, string skillId)
        {
            return _masteryRepository.GetSkillMastery(steamId, skillId);
        }
        
        public List<SkillMastery> GetPlayerMasteries(string steamId)
        {
            return _masteryRepository.GetPlayerMasteries(steamId);
        }
        
        public int GetMasteryLevel(string steamId, string skillId)
        {
            var mastery = GetSkillMastery(steamId, skillId);
            return mastery?.MasteryLevel ?? 0;
        }
        
        public List<string> GetUnlockedRewards(string steamId, string skillId)
        {
            var mastery = GetSkillMastery(steamId, skillId);
            return mastery?.UnlockedRewards ?? new List<string>();
        }
        
        public bool HasReward(string steamId, string skillId, string rewardId)
        {
            var rewards = GetUnlockedRewards(steamId, skillId);
            return rewards.Contains(rewardId);
        }
        
        private SkillMastery GetOrCreateMastery(string steamId, string skillId)
        {
            var mastery = _masteryRepository.GetSkillMastery(steamId, skillId);
            if (mastery == null)
            {
                mastery = new SkillMastery
                {
                    SteamId = steamId,
                    SkillId = skillId
                };
            }
            return mastery;
        }
        
        private void NotifyMasteryLevelUp(string steamId, string skillId, int newLevel)
        {
            // Find player and notify
            var players = Utilities.GetPlayers();
            var player = players.FirstOrDefault(p => 
                p != null && 
                p.IsValid && 
                p.AuthorizedSteamID != null && 
                p.AuthorizedSteamID.SteamId64.ToString() == steamId);
            
            if (player != null && player.IsValid)
            {
                player.PrintToChat($" {ChatColors.Gold}[Mastery]{ChatColors.Default} Skill Mastery Level Up! Level {newLevel} reached!");
            }
        }
    }
}
