using System.Collections.Generic;
using System.Linq;

namespace UltimateHeroes.Domain.Talents
{
    /// <summary>
    /// Ein Talent Node im Talent Tree
    /// </summary>
    public class TalentNode
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Tree & Position
        public TalentTreeType TreeType { get; set; }
        public int Row { get; set; } // 1-5
        public int Column { get; set; } // 1-3
        
        // Progression
        public int MaxLevel { get; set; } = 5;
        public List<string> Prerequisites { get; set; } = new(); // Talent IDs
        
        // Effects
        public TalentEffect Effect { get; set; } = new();
        
        // Helper
        public bool CanUnlock(List<string> unlockedTalents)
        {
            if (Prerequisites.Count == 0) return true;
            return Prerequisites.All(p => unlockedTalents.Contains(p));
        }
    }
}
