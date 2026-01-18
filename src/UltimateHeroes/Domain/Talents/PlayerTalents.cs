using System.Collections.Generic;

namespace UltimateHeroes.Domain.Talents
{
    /// <summary>
    /// Talent Progress eines Spielers
    /// </summary>
    public class PlayerTalents
    {
        public string SteamId { get; set; } = string.Empty;
        public List<string> UnlockedTalents { get; set; } = new(); // Talent IDs
        public Dictionary<string, int> TalentLevels { get; set; } = new(); // talent_id -> level
        public int AvailableTalentPoints { get; set; } = 0;
        
        // Helper
        public bool IsUnlocked(string talentId)
        {
            return UnlockedTalents.Contains(talentId);
        }
        
        public int GetTalentLevel(string talentId)
        {
            return TalentLevels.GetValueOrDefault(talentId, 0);
        }
        
        public bool CanUnlock(TalentNode node)
        {
            if (AvailableTalentPoints <= 0) return false;
            if (IsUnlocked(node.Id)) return false;
            return node.CanUnlock(UnlockedTalents);
        }
        
        public bool CanLevelUp(TalentNode node)
        {
            if (AvailableTalentPoints <= 0) return false;
            if (!IsUnlocked(node.Id)) return false;
            int currentLevel = GetTalentLevel(node.Id);
            if (currentLevel >= node.MaxLevel) return false;
            return true;
        }
        
        public void UnlockTalent(string talentId, int level = 1)
        {
            if (!UnlockedTalents.Contains(talentId))
            {
                UnlockedTalents.Add(talentId);
            }
            TalentLevels[talentId] = level;
            AvailableTalentPoints--;
        }
        
        public void LevelUpTalent(string talentId)
        {
            if (!IsUnlocked(talentId)) return;
            int currentLevel = GetTalentLevel(talentId);
            TalentLevels[talentId] = currentLevel + 1;
            AvailableTalentPoints--;
        }
    }
}
