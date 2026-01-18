using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Builds;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Build Selection/Editor Menu
    /// </summary>
    public class BuildMenu
    {
        private readonly IBuildService _buildService;
        private readonly IPlayerService _playerService;
        
        public BuildMenu(IBuildService buildService, IPlayerService playerService)
        {
            _buildService = buildService;
            _playerService = playerService;
        }
        
        public void ShowMenu(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var builds = _buildService.GetPlayerBuilds(steamId);
            var activeBuild = _buildService.GetActiveBuild(steamId);
            var unlockedSlots = _buildService.GetUnlockedSlots(steamId);
            
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.Default}  Ultimate Heroes - Builds      {ChatColors.Green}║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════╝");
            
            if (activeBuild != null)
            {
                player.PrintToChat($" {ChatColors.Yellow}Active Build: {ChatColors.LightBlue}{activeBuild.BuildName} {ChatColors.Default}(Slot {activeBuild.BuildSlot})");
                player.PrintToChat($"   Hero: {ChatColors.LightBlue}{activeBuild.HeroCoreId}{ChatColors.Default} | Skills: {string.Join(", ", activeBuild.SkillIds)}");
            }
            
            player.PrintToChat($"");
            player.PrintToChat($" {ChatColors.Default}Your Builds:");
            
            for (int slot = 1; slot <= 5; slot++)
            {
                var build = builds.FirstOrDefault(b => b.BuildSlot == slot);
                var isUnlocked = unlockedSlots.Contains(slot);
                
                if (!isUnlocked)
                {
                    player.PrintToChat($" {ChatColors.Gray}Slot {slot}: {ChatColors.DarkGray}[LOCKED]");
                }
                else if (build != null)
                {
                    var isActive = build.IsActive;
                    var status = isActive ? $"{ChatColors.Green}[ACTIVE]{ChatColors.Default}" : "";
                    player.PrintToChat($" {ChatColors.LightBlue}Slot {slot}: {build.BuildName} {status}");
                    player.PrintToChat($"   Hero: {build.HeroCoreId} | Skills: {string.Join(", ", build.SkillIds)}");
                }
                else
                {
                    player.PrintToChat($" {ChatColors.Gray}Slot {slot}: {ChatColors.Default}[EMPTY]");
                }
            }
            
            player.PrintToChat($"");
            player.PrintToChat($" {ChatColors.Gray}Use: !createbuild <slot> <hero> <skill1> [skill2] [skill3] <name>");
            player.PrintToChat($" {ChatColors.Gray}Use: !activatebuild <slot> to activate a build");
        }
    }
}
