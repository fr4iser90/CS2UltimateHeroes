using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// HealingAura - Passive Skill: Heilt Spieler und Teammates
    /// </summary>
    public class HealingAura : PassiveSkillBase
    {
        public override string Id => "healing_aura";
        public override string DisplayName => "Healing Aura";
        public override string Description => "Heilt dich und Teammates in der Nähe";
        public override int PowerWeight => 15;
        public override List<SkillTag> Tags => new() { SkillTag.Support, SkillTag.Area };
        public override int MaxLevel => 5;
        
        private const float BaseHealPerSecond = 2f;
        private const int BaseRadius = 200;
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Start healing aura effect
            // TODO: Implement periodic healing
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // Nothing special on hurt
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // Nothing special on kill
        }
        
        // TODO: Add periodic healing method that gets called every second
        public void HealInRadius(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var healPerSecond = BaseHealPerSecond + (CurrentLevel * 0.5f);
            var radius = BaseRadius + (CurrentLevel * 30);
            
            // TODO: Find all players in radius
            // TODO: Heal them
            // TODO: Show particles/effects
        }
    }
}
