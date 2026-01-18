using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Skill Browser Menu
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
            
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.Default}  Ultimate Heroes - Skills      {ChatColors.Green}║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════╝");
            
            if (playerState != null && playerState.ActiveSkills.Count > 0)
            {
                player.PrintToChat($" {ChatColors.Yellow}Active Skills:");
                foreach (var skill in playerState.ActiveSkills)
                {
                    var cooldown = _cooldownManager.GetCooldown(steamId, skill.Id);
                    var cooldownText = cooldown > 0 ? $"{ChatColors.Red}[CD: {cooldown:F1}s]{ChatColors.Default}" : $"{ChatColors.Green}[READY]{ChatColors.Default}";
                    var level = playerState.SkillLevels.GetValueOrDefault(skill.Id, 1);
                    player.PrintToChat($"  {ChatColors.LightBlue}{skill.DisplayName}{ChatColors.Default} (Lv.{level}) - {skill.Description} {cooldownText}");
                }
                player.PrintToChat($"");
            }
            
            player.PrintToChat($" {ChatColors.Default}Available Skills:");
            
            var skillsByType = allSkills.GroupBy(s => s.Type);
            foreach (var group in skillsByType)
            {
                player.PrintToChat($" {ChatColors.Yellow}{group.Key}:");
                foreach (var skill in group)
                {
                    var tags = string.Join(", ", skill.Tags);
                    player.PrintToChat($"  {ChatColors.LightBlue}{skill.DisplayName}{ChatColors.Default} - Power: {ChatColors.Yellow}{skill.PowerWeight}{ChatColors.Default} | Tags: {tags}");
                }
            }
        }
    }
}
