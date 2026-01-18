using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Life on Kill - Passive Skill: +HP pro Kill
    /// </summary>
    public class LifeOnKillPassive : PassiveSkillBase
    {
        public override string Id => "life_on_kill_passive";
        public override string DisplayName => "Life on Kill";
        public override string Description => "Gibt +HP pro Kill";
        public override int PowerWeight => 10;
        public override List<SkillTag> Tags => new() { SkillTag.Support };
        public override int MaxLevel => 5;
        
        private const int BaseHealAmount = 15;
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Passive is active, no spawn action needed
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // No action on hurt
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            if (player == null || !player.IsValid) return;
            
            // Calculate heal amount based on level
            var healAmount = BaseHealAmount + (CurrentLevel * 5); // 15-35 HP
            
            // Heal player
            GameHelpers.HealPlayer(player, healAmount);
            
            player.PrintToChat($" {ChatColors.Green}[Life on Kill]{ChatColors.Default} Healed {healAmount} HP!");
        }
    }
}
