using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Scanner Drone - Active Skill: Revealt Gegner im Radius
    /// </summary>
    public class ScannerDrone : ActiveSkillBase
    {
        public override string Id => "scanner_drone";
        public override string DisplayName => "Scanner Drone";
        public override string Description => "Revealt Gegner im Radius";
        public override int PowerWeight => 20;
        public override List<SkillTag> Tags => new() { SkillTag.Utility, SkillTag.Area };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 15f;
        
        private const float BaseRadius = 500f;
        private const float BaseDuration = 8f;
        
        // SpawnService wird über Helper gesetzt
        public static Application.Services.ISpawnService? SpawnService { get; set; }
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null || player.AuthorizedSteamID == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null) return;
            
            // Calculate stats based on level
            var radius = BaseRadius + (CurrentLevel * 100);
            var duration = BaseDuration + (CurrentLevel * 2f);
            
            // Spawn drone via SpawnService (persistent reveal)
            if (SpawnService != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                SpawnService.SpawnDrone(steamId, pawn.AbsOrigin, radius, duration);
            }
            
            player.PrintToChat($" {ChatColors.Green}[Scanner Drone]{ChatColors.Default} Drone deployed! Reveals enemies in {radius:F0} range for {duration:F1}s");
        }
    }
}
