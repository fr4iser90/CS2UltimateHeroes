using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Talents;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Talent Tree Menu (Interaktiv mit HTML)
    /// </summary>
    public class TalentMenu
    {
        private readonly ITalentService _talentService;
        private readonly IPlayerService _playerService;
        
        public TalentMenu(ITalentService talentService, IPlayerService playerService)
        {
            _talentService = talentService;
            _playerService = playerService;
        }
        
        public void ShowMenu(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var availablePoints = _talentService.GetAvailablePoints(steamId);
            
            var mainMenu = MenuManager.CreateMenu($"<font color='lightgrey' class='fontSize-m'>Ultimate Heroes - Talent Trees</font><br><font color='gold' class='fontSize-sm'>Available Points: {availablePoints}</font>", 5);
            
            // Add Talent Trees
            var combatTree = _talentService.GetTalentTree(TalentTreeType.Combat);
            var utilityTree = _talentService.GetTalentTree(TalentTreeType.Utility);
            var movementTree = _talentService.GetTalentTree(TalentTreeType.Movement);
            
            mainMenu.Add($"<font color='red'>Combat Tree</font>", 
                $"<font color='grey' class='fontSize-sm'>{combatTree.Nodes.Count} Talents</font>", 
                (p, opt) => ShowTalentTree(p, TalentTreeType.Combat));
            
            mainMenu.Add($"<font color='blue'>Utility Tree</font>", 
                $"<font color='grey' class='fontSize-sm'>{utilityTree.Nodes.Count} Talents</font>", 
                (p, opt) => ShowTalentTree(p, TalentTreeType.Utility));
            
            mainMenu.Add($"<font color='green'>Movement Tree</font>", 
                $"<font color='grey' class='fontSize-sm'>{movementTree.Nodes.Count} Talents</font>", 
                (p, opt) => ShowTalentTree(p, TalentTreeType.Movement));
            
            MenuManager.OpenMainMenu(player, mainMenu);
        }
        
        private void ShowTalentTree(CCSPlayerController player, TalentTreeType treeType)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var tree = _talentService.GetTalentTree(treeType);
            var unlockedTalents = _talentService.GetUnlockedTalents(steamId, treeType);
            var unlockableTalents = _talentService.GetUnlockableTalents(steamId, treeType);
            var availablePoints = _talentService.GetAvailablePoints(steamId);
            
            var treeMenu = MenuManager.CreateMenu($"<font color='lightgrey' class='fontSize-m'>{tree.DisplayName} Tree</font><br><font color='gold' class='fontSize-sm'>Points: {availablePoints} | Unlocked: {unlockedTalents.Count}/{tree.Nodes.Count}</font>", 5);
            
            // Group talents by row
            var talentsByRow = tree.Nodes.OrderBy(t => t.Row).ThenBy(t => t.Column).GroupBy(t => t.Row);
            
            foreach (var rowGroup in talentsByRow)
            {
                foreach (var talent in rowGroup)
                {
                    var isUnlocked = unlockedTalents.Any(t => t.Id == talent.Id);
                    var canUnlock = _talentService.CanUnlockTalent(steamId, talent.Id);
                    
                    string display;
                    string subDisplay;
                    
                    if (isUnlocked)
                    {
                        display = $"<font color='green'>[UNLOCKED]</font> <font color='lightblue'>{talent.DisplayName}</font> (Lv.{talent.MaxLevel})";
                        subDisplay = $"<font color='grey' class='fontSize-sm'>{talent.Description}</font>";
                    }
                    else if (canUnlock)
                    {
                        display = $"<font color='yellow'>[AVAILABLE]</font> <font color='lightblue'>{talent.DisplayName}</font>";
                        subDisplay = $"<font color='grey' class='fontSize-sm'>{talent.Description}<br>Row {talent.Row}, Column {talent.Column}</font>";
                    }
                    else
                    {
                        var missingPrereq = talent.Prerequisites.FirstOrDefault(p => !unlockedTalents.Any(u => u.Id == p));
                        var prereqText = missingPrereq != null ? $"<br>Requires: {missingPrereq}" : "";
                        var pointsText = availablePoints <= 0 ? "<br><font color='red'>No talent points available</font>" : "";
                        
                        display = $"<font color='grey'>[LOCKED]</font> <font color='darkgrey'>{talent.DisplayName}</font>";
                        subDisplay = $"<font color='grey' class='fontSize-sm'>{talent.Description}{prereqText}{pointsText}</font>";
                    }
                    
                    var talentId = talent.Id;
                    treeMenu.Add(display, subDisplay, (p, opt) =>
                    {
                        if (canUnlock && !isUnlocked)
                        {
                            if (_talentService.UnlockTalent(steamId, talentId))
                            {
                                MenuManager.CloseMenu(p);
                                p.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Talent unlocked: {talent.DisplayName}");
                            }
                            else
                            {
                                p.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Failed to unlock talent!");
                            }
                        }
                        else if (isUnlocked)
                        {
                            p.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} This talent is already unlocked!");
                        }
                        else
                        {
                            p.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Cannot unlock this talent yet!");
                        }
                    });
                }
            }
            
            MenuManager.OpenMainMenu(player, treeMenu);
        }
    }
}
