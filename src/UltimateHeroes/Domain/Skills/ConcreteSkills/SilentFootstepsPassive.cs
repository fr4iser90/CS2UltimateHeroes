using System.Collections.Generic;
using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Silent Footsteps - Passive Skill: Keine Footstep-Sounds
    /// </summary>
    public class SilentFootstepsPassive : PassiveSkillBase
    {
        public override string Id => "silent_footsteps_passive";
        public override string DisplayName => "Silent Footsteps";
        public override string Description => "Deine Schritte sind lautlos";
        public override int PowerWeight => 0; // Hero Passive, kein Weight
        public override List<SkillTag> Tags => new() { SkillTag.Stealth, SkillTag.Utility };
        public override int MaxLevel => 1;
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // Disable footstep sounds (CS2 API)
            var pawn = player.PlayerPawn.Value;
            // Note: CS2 API may not directly support footstep volume control
            // This is a placeholder - actual implementation may require game-specific mechanics
            // For now, we rely on game mechanics (stealth mode, etc.)
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // Nothing
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // Nothing
        }
    }
}
