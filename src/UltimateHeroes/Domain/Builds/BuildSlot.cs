using System.Collections.Generic;

namespace UltimateHeroes.Domain.Builds
{
    /// <summary>
    /// Verwaltet Build Slots und deren Unlock-Anforderungen
    /// </summary>
    public class BuildSlot
    {
        public int SlotNumber { get; set; } // 1-5
        public bool IsUnlocked { get; set; }
        public int UnlockLevel { get; set; } // z.B. Slot 4 bei Level 10
        
        /// <summary>
        /// Unlock-Anforderungen für jeden Slot (Slot Number -> Required Level)
        /// </summary>
        public static Dictionary<int, int> UnlockRequirements = new()
        {
            { 1, 1 },   // Slot 1: Level 1 (immer)
            { 2, 1 },   // Slot 2: Level 1 (immer)
            { 3, 1 },   // Slot 3: Level 1 (immer)
            { 4, 10 },  // Slot 4: Level 10
            { 5, 20 }   // Slot 5: Level 20
        };
        
        /// <summary>
        /// Prüft ob ein Slot für einen bestimmten Level freigeschaltet ist
        /// </summary>
        public static bool IsSlotUnlocked(int slotNumber, int playerLevel)
        {
            if (!UnlockRequirements.ContainsKey(slotNumber))
            {
                return false;
            }
            
            return playerLevel >= UnlockRequirements[slotNumber];
        }
        
        /// <summary>
        /// Gibt alle freigeschalteten Slots für einen Level zurück
        /// </summary>
        public static List<int> GetUnlockedSlots(int playerLevel)
        {
            var unlockedSlots = new List<int>();
            
            foreach (var requirement in UnlockRequirements)
            {
                if (playerLevel >= requirement.Value)
                {
                    unlockedSlots.Add(requirement.Key);
                }
            }
            
            return unlockedSlots;
        }
        
        /// <summary>
        /// Gibt das benötigte Level für einen Slot zurück
        /// </summary>
        public static int GetRequiredLevel(int slotNumber)
        {
            return UnlockRequirements.GetValueOrDefault(slotNumber, int.MaxValue);
        }
    }
}
