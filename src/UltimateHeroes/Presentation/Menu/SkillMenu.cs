using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Infrastructure.Cooldown;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Skill Browser Menu (Interaktiv mit HTML)
    /// </summary>
    public class SkillMenu
    {
        private readonly ISkillService _skillService;
        private readonly IPlayerService _playerService;
        private readonly ICooldownManager _cooldownManager;
        
        public SkillMenu(ISkillService skillService, IPlayerService playerService, ICooldownManager cooldownManager)
        {
            _skillService = skillService;
            _playerService = playerService;
            _cooldownManager = cooldownManager;
        }
        
        public void ShowMenu(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = _playerService.GetPlayer(steamId);
            var allSkills = _skillService.GetAllSkills();
            
            var skillMenu = MenuManager.CreateMenu($"<font color='lightgrey' class='fontSize-m'>Ultimate Heroes - Skills</font>", 5);
            
            // Active Skills Section
            if (playerState != null && playerState.ActiveSkills.Count > 0)
            {
                foreach (var skill in playerState.ActiveSkills)
                {
                    var cooldown = _cooldownManager.GetCooldown(steamId, skill.Id);
                    var cooldownText = cooldown > 0 
                        ? $"<font color='red'>[CD: {cooldown:F1}s]</font>" 
                        : "<font color='green'>[READY]</font>";
                    var level = playerState.SkillLevels.GetValueOrDefault(skill.Id, 1);
                    
                    var display = $"<font color='green'>[ACTIVE]</font> <font color='lightblue'>{skill.DisplayName}</font> (Lv.{level}) {cooldownText}";
                    var subDisplay = $"<font color='grey' class='fontSize-sm'>{skill.Description}</font>";
                    
                    var skillId = skill.Id;
                    skillMenu.Add(display, subDisplay, (p, opt) =>
                    {
                        if (cooldown <= 0)
                        {
                            _skillService.ActivateSkill(steamId, skillId, p);
                            MenuManager.CloseMenu(p);
                        }
                        else
                        {
                            p.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill is on cooldown: {cooldown:F1}s");
                        }
                    });
                }
            }
            
            // Available Skills by Type
            var skillsByType = allSkills.GroupBy(s => s.Type);
            foreach (var group in skillsByType)
            {
                foreach (var skill in group)
                {
                    var tags = string.Join(", ", skill.Tags);
                    var display = $"<font color='lightblue'>{skill.DisplayName}</font> <font color='grey'>({group.Key})</font>";
                    var subDisplay = $"<font color='grey' class='fontSize-sm'>{skill.Description}<br>Power: <font color='yellow'>{skill.PowerWeight}</font> | Tags: {tags}</font>";
                    
                    var skillId = skill.Id;
                    skillMenu.Add(display, subDisplay, (p, opt) =>
                    {
                        if (skill.Type == SkillType.Active || skill.Type == SkillType.Ultimate)
                        {
                            var cd = _cooldownManager.GetCooldown(steamId, skillId);
                            if (cd <= 0)
                            {
                                _skillService.ActivateSkill(steamId, skillId, p);
                                MenuManager.CloseMenu(p);
                            }
                            else
                            {
                                p.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill is on cooldown: {cd:F1}s");
                            }
                        }
                        else
                        {
                            p.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} This is a passive skill!");
                        }
                    });
                }
            }
            
            MenuManager.OpenMainMenu(player, skillMenu);
        }
    }
}
