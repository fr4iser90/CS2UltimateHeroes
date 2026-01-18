using System.Collections.Generic;
using System.Linq;

namespace UltimateHeroes.Domain.Talents
{
    /// <summary>
    /// Ein Talent Tree (Combat, Utility, Movement)
    /// </summary>
    public class TalentTree
    {
        public TalentTreeType Type { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public List<TalentNode> Nodes { get; set; } = new();
        
        // Helper
        public TalentNode? GetNode(string nodeId)
        {
            return Nodes.FirstOrDefault(n => n.Id == nodeId);
        }
        
        public List<TalentNode> GetUnlockableNodes(List<string> unlockedTalents)
        {
            return Nodes.Where(n => n.CanUnlock(unlockedTalents) && !unlockedTalents.Contains(n.Id)).ToList();
        }
    }
    
    public enum TalentTreeType
    {
        Combat,
        Utility,
        Movement
    }
}
