using System.Collections.Generic;

namespace UltimateHeroes.Domain.Buffs
{
    /// <summary>
    /// Buff Definition - Definiert einen Buff-Typ (für Skills, um Buffs zu erstellen)
    /// </summary>
    public class BuffDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public BuffType Type { get; set; }
        public BuffStackingType StackingType { get; set; } = BuffStackingType.Refresh;
        public int MaxStacks { get; set; } = 1;
        
        /// <summary>
        /// Creates a Buff instance from this definition with given parameters
        /// </summary>
        public Buff CreateBuff(float duration, Dictionary<string, float>? parameters = null)
        {
            return new Buff
            {
                Id = Id,
                DisplayName = DisplayName,
                Type = Type,
                Duration = duration,
                StackingType = StackingType,
                MaxStacks = MaxStacks,
                Parameters = parameters ?? new Dictionary<string, float>()
            };
        }
    }
}
