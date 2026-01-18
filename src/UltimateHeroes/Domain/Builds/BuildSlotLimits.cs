namespace UltimateHeroes.Domain.Builds
{
    /// <summary>
    /// Verwaltet dynamische Slot-Limits für Builds (Base + Bonus)
    /// </summary>
    public class BuildSlotLimits
    {
        // Base Slots (Level-basiert)
        public int BaseActiveSlots { get; set; } = 3;
        public int BaseUltimateSlots { get; set; } = 1;
        public int BasePassiveSlots { get; set; } = 2;
        
        // Bonus Slots (Talent-basiert)
        public int BonusActiveSlots { get; set; } = 0;
        public int BonusUltimateSlots { get; set; } = 0;
        public int BonusPassiveSlots { get; set; } = 0;
        
        // Total Slots
        public int TotalActiveSlots => BaseActiveSlots + BonusActiveSlots;
        public int TotalUltimateSlots => BaseUltimateSlots + BonusUltimateSlots;
        public int TotalPassiveSlots => BasePassiveSlots + BonusPassiveSlots;
        
        // Max Limits (Hard Caps)
        public const int MaxActiveSlots = 5;
        public const int MaxUltimateSlots = 2;
        public const int MaxPassiveSlots = 4;
        
        /// <summary>
        /// Berechnet Base Slots basierend auf Level
        /// </summary>
        public static BuildSlotLimits CalculateBaseSlots(int heroLevel)
        {
            var limits = new BuildSlotLimits
            {
                BaseActiveSlots = 3,
                BaseUltimateSlots = 1,
                BasePassiveSlots = 2
            };
            
            // Level 10: +1 Active Slot
            if (heroLevel >= 10)
            {
                limits.BaseActiveSlots = 4;
            }
            
            // Level 20: +1 Ultimate Slot
            if (heroLevel >= 20)
            {
                limits.BaseUltimateSlots = 2;
            }
            
            // Level 30: +1 Passive Slot
            if (heroLevel >= 30)
            {
                limits.BasePassiveSlots = 3;
            }
            
            return limits;
        }
        
        /// <summary>
        /// Prüft ob Limits innerhalb der Max-Limits sind
        /// </summary>
        public bool IsWithinMaxLimits()
        {
            return TotalActiveSlots <= MaxActiveSlots &&
                   TotalUltimateSlots <= MaxUltimateSlots &&
                   TotalPassiveSlots <= MaxPassiveSlots;
        }
    }
}
