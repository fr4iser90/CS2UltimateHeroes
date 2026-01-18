using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Builds;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Build Selection/Editor Menu (Interaktiv mit HTML)
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
            
            var buildMenu = MenuManager.CreateMenu($"<font color='lightgrey' class='fontSize-m'>Ultimate Heroes - Build Management</font>", 5);
            
            if (activeBuild != null)
            {
                var activeDisplay = $"<font color='green'>[ACTIVE]</font> <font color='lightblue'>{activeBuild.BuildName}</font> (Slot {activeBuild.BuildSlot})";
                var skillsDisplay = $"Active: {activeBuild.ActiveSkillIds.Count}/3";
                if (!string.IsNullOrEmpty(activeBuild.UltimateSkillId))
                    skillsDisplay += $" | Ultimate: {activeBuild.UltimateSkillId}";
                skillsDisplay += $" | Passive: {activeBuild.PassiveSkillIds.Count}/2";
                var activeSubDisplay = $"<font color='grey' class='fontSize-sm'>Hero: <font color='lightblue'>{activeBuild.HeroCoreId}</font><br>{skillsDisplay}</font>";
                buildMenu.Add(activeDisplay, activeSubDisplay, (p, opt) =>
                {
                    p.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} This build is already active!");
                });
            }
            
            for (int slot = 1; slot <= 5; slot++)
            {
                var build = builds.FirstOrDefault(b => b.BuildSlot == slot);
                var isUnlocked = unlockedSlots.Contains(slot);
                var isActive = build?.IsActive ?? false;
                
                if (!isUnlocked)
                {
                    buildMenu.Add($"<font color='grey'>Slot {slot}: <font color='darkgrey'>[LOCKED]</font></font>", null, (p, opt) =>
                    {
                        p.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} This slot is locked!");
                    });
                }
                else if (build != null)
                {
                    var status = isActive ? "<font color='green'>[ACTIVE]</font>" : "";
                    var display = $"<font color='lightblue'>Slot {slot}: {build.BuildName}</font> {status}";
                    var skillsDisplay = $"Active: {build.ActiveSkillIds.Count}/3";
                    if (!string.IsNullOrEmpty(build.UltimateSkillId))
                        skillsDisplay += $" | Ultimate: {build.UltimateSkillId}";
                    skillsDisplay += $" | Passive: {build.PassiveSkillIds.Count}/2";
                    var subDisplay = $"<font color='grey' class='fontSize-sm'>Hero: <font color='lightblue'>{build.HeroCoreId}</font><br>{skillsDisplay}</font>";
                    
                    var buildSlot = slot;
                    buildMenu.Add(display, subDisplay, (p, opt) =>
                    {
                        if (!isActive)
                        {
                            _buildService.ActivateBuild(steamId, buildSlot, p);
                            MenuManager.CloseMenu(p);
                            p.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Build activated: {build.BuildName}");
                        }
                        else
                        {
                            p.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} This build is already active!");
                        }
                    });
                }
                else
                {
                    buildMenu.Add($"<font color='grey'>Slot {slot}: <font color='white'>[EMPTY]</font></font>", 
                        "<font color='grey' class='fontSize-sm'>Use !createbuild to create a build</font>", 
                        (p, opt) =>
                    {
                        p.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} Use: !createbuild {slot} <hero> <skill1> [skill2] [skill3] <name>");
                    });
                }
            }
            
            MenuManager.OpenMainMenu(player, buildMenu);
        }
    }
}
