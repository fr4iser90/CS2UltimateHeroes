using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Teleport - Ultimate Skill: Teleportiert zu beliebiger Position
    /// </summary>
    public class Teleport : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "teleport";
        public override string DisplayName => "Teleport";
        public override string Description => "Teleportiert dich zu einer beliebigen Position auf der Map";
        public override int PowerWeight => 40;
        public override List<SkillTag> Tags => new() { SkillTag.Mobility, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 60f;
        
        private const float CastTime = 2f;
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            // TODO: Implement teleport logic
            // - Show crosshair/aim indicator
            // - Cast time (2s)
            // - Check if position is valid
            // - Teleport player
            // - Show effects
            
            // Placeholder: Just notify player
            player.PrintToChat($" {ChatColors.Gold}[Teleport]{ChatColors.Default} Ultimate ready! (Cast time: {CastTime}s)");
        }
    }
}
