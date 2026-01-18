using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Passive Skill: Gibt +5 Armor nach jedem Kill
    /// </summary>
    public class ArmorPerKillPassive : PassiveSkillBase
    {
        public override string Id => "armor_per_kill_passive";
        public override string DisplayName => "Armor per Kill";
        public override string Description => "Gibt +5 Armor nach jedem Kill";
        public override int PowerWeight => 0; // Hero Passive, kein Weight
        public override List<SkillTag> Tags => new() { SkillTag.Defense };
        public override int MaxLevel => 1;
        
        private const int ArmorPerKill = 5;
        private const int MaxArmor = 100;
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Nichts beim Spawn
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // Nichts bei Damage
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.ArmorValue < MaxArmor)
            {
                var newArmor = pawn.ArmorValue + ArmorPerKill;
                pawn.ArmorValue = newArmor > MaxArmor ? MaxArmor : newArmor;
            }
        }
    }
}
