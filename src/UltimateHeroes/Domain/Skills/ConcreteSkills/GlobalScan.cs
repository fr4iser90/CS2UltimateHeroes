using System.Collections.Generic;
using UltimateHeroes.Infrastructure.Helpers;
using CounterStrikeSharp.API;
using UltimateHeroes.Infrastructure.Helpers;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Infrastructure.Helpers;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;
using UltimateHeroes.Infrastructure.Helpers;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Global Scan - Ultimate Skill: Revealt alle Gegner kurzzeitig
    /// </summary>
    public class GlobalScan : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "global_scan";
        public override string DisplayName => "Global Scan";
        public override string Description => "Revealt alle Gegner kurzzeitig";
        public override int PowerWeight => 40;
        public override List<SkillTag> Tags => new() { SkillTag.Utility, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 90f;
        
        private const float BaseDuration = 5f;
        
        // Buff Definition (wird einmal erstellt, kann wiederverwendet werden)
        private static readonly Domain.Buffs.BuffDefinition RevealBuffDefinition = new()
        {
            Id = "global_scan_reveal", // Will be made unique per player
            DisplayName = "Revealed",
            Type = Domain.Buffs.BuffType.Reveal,
            StackingType = Domain.Buffs.BuffStackingType.Refresh
        };
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Calculate duration based on level
            var duration = BaseDuration + (CurrentLevel * 2f);
            
            // Create Reveal Buffs in Skill (not in Service)
            var buffService = Infrastructure.Helpers.BuffServiceHelper.GetBuffService();
            int revealedCount = 0;
            
            foreach (var target in Utilities.GetPlayers())
            {
                if (target == player) continue;
                if (!target.IsValid || target.AuthorizedSteamID == null) continue;
                
                var targetSteamId = target.AuthorizedSteamID.SteamId64.ToString();
                
                // Create Reveal Buff from Definition (generisch)
                if (buffService != null)
                {
                    var revealBuff = RevealBuffDefinition.CreateBuff(duration);
                    revealBuff.Id = $"global_scan_reveal_{targetSteamId}"; // Make unique per player
                    buffService.ApplyBuff(targetSteamId, revealBuff);
                }
                
                // Make enemy visible (remove invisibility if active)
                GameHelper.MakePlayerInvisible(target, false);
                
                revealedCount++;
                
                // Notify both players
                player.PrintToChat($" {ChatColors.Green}[Global Scan]{ChatColors.Default} Revealed {target.PlayerName}!");
                target.PrintToChat($" {ChatColors.Red}[Global Scan]{ChatColors.Default} You have been revealed!");
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Global Scan]{ChatColors.Default} Ultimate! Revealed {revealedCount} enemies for {duration:F1}s!");
        }
    }
}
