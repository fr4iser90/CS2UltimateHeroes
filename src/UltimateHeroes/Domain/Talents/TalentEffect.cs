using System.Collections.Generic;

namespace UltimateHeroes.Domain.Talents
{
    /// <summary>
    /// Effect eines Talents
    /// </summary>
    public class TalentEffect
    {
        public TalentEffectType Type { get; set; }
        public Dictionary<string, float> Parameters { get; set; } = new();
        
        // z.B. { "damage_bonus": 0.05f } = +5% Damage
        // z.B. { "recoil_reduction": 0.1f } = -10% Recoil
    }
    
    public enum TalentEffectType
    {
        DamageBonus,
        RecoilReduction,
        ArmorPenetration,
        ExtraNade,
        PlantSpeed,
        DefuseSpeed,
        AirControl,
        LadderSpeed,
        SilentDrop,
        HeadshotDamage,
        ReloadSpeed,
        WeaponAccuracy,
        MovementSpeed,
        JumpHeight,
        FallDamageReduction,
        // Slot Expansion Talents
        ExtraActiveSlot,    // +1 Active Slot
        ExtraUltimateSlot,  // +1 Ultimate Slot
        ExtraPassiveSlot    // +1 Passive Slot
    }
}
