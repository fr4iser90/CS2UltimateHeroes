using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Fireball - Active Skill: Wirft einen Feuerball
    /// </summary>
    public class Fireball : ActiveSkillBase
    {
        public override string Id => "fireball";
        public override string DisplayName => "Fireball";
        public override string Description => "Wirft einen Feuerball in Blickrichtung";
        public override int PowerWeight => 25;
        public override List<SkillTag> Tags => new() { SkillTag.Damage, SkillTag.Area };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 10f;
        
        private const int BaseDamage = 30;
        private const int BaseRadius = 150;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // Calculate damage and radius based on level
            var damage = BaseDamage + (CurrentLevel * 5);
            var radius = BaseRadius + (CurrentLevel * 20);
            
            // TODO: Implement actual fireball logic
            // - Get player position and view angle
            // - Spawn fireball projectile
            // - Apply damage in radius
            // - Apply burn effect (Damage Over Time)
            
            // Placeholder: Just notify player
            player.PrintToChat($" {ChatColors.Orange}[Fireball]{ChatColors.Default} Damage: {damage}, Radius: {radius}");
        }
    }
}
