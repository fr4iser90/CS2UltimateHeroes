using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Blink - Active Skill: Teleportiert Spieler in Blickrichtung
    /// </summary>
    public class Blink : ActiveSkillBase
    {
        public override string Id => "blink";
        public override string DisplayName => "Blink";
        public override string Description => "Teleportiert dich in Blickrichtung";
        public override int PowerWeight => 20;
        public override List<SkillTag> Tags => new() { SkillTag.Mobility };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 5f;
        
        private const int BaseRange = 300;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate range based on level
            var range = BaseRange + (CurrentLevel * 50);
            
            // TODO: Implement actual blink logic
            // - Get player position and view angle
            // - Calculate destination
            // - Check for collisions
            // - Teleport player
            // - Apply invulnerability for 0.2s
            
            // Placeholder: Just notify player
            player.PrintToChat($" {ChatColors.Cyan}[Blink]{ChatColors.Default} Teleported {range} units forward!");
        }
    }
}
