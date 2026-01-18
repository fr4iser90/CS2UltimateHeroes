using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Infrastructure.Cooldown;
using UltimateHeroes.Presentation.Menu;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Command Handler für Skill-bezogene Commands
    /// </summary>
    public class SkillCommands : ICommandHandler
    {
        private readonly ISkillService _skillService;
        private readonly IPlayerService _playerService;
        private readonly ICooldownManager _cooldownManager;
        private readonly SkillMenu _skillMenu;
        
        public string CommandName => "skills";
        public string Description => "Open skills menu";
        
        public SkillCommands(ISkillService skillService, IPlayerService playerService, ICooldownManager cooldownManager, SkillMenu skillMenu)
        {
            _skillService = skillService;
            _playerService = playerService;
            _cooldownManager = cooldownManager;
            _skillMenu = skillMenu;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _skillMenu.ShowMenu(player);
        }
    }
    
    /// <summary>
    /// Command Handler für !use
    /// </summary>
    public class UseSkillCommand : ICommandHandler
    {
        private readonly ISkillService _skillService;
        private readonly IPlayerService _playerService;
        
        public string CommandName => "use";
        public string Description => "Use a skill";
        
        public UseSkillCommand(ISkillService skillService, IPlayerService playerService)
        {
            _skillService = skillService;
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var args = commandInfo.GetCommandString.Split(' ');
            
            if (args.Length < 2)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !use <skill_id>");
                return;
            }
            
            var skillId = args[1].ToLower();
            
            try
            {
                if (_skillService.CanActivateSkill(steamId, skillId))
                {
                    _skillService.ActivateSkill(steamId, skillId, player);
                }
                else
                {
                    var cooldown = _skillService.GetSkillCooldown(steamId, skillId);
                    if (cooldown > 0)
                    {
                        player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill on cooldown: {cooldown:F1}s");
                    }
                }
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Command Handler für !skill1, !skill2, !skill3
    /// </summary>
    public class SkillSlotCommand : ICommandHandler
    {
        private readonly ISkillService _skillService;
        private readonly IPlayerService _playerService;
        private readonly int _slotIndex;
        
        public string CommandName { get; }
        public string Description => $"Activate skill slot {_slotIndex + 1}";
        
        public SkillSlotCommand(ISkillService skillService, IPlayerService playerService, int slotIndex)
        {
            _skillService = skillService;
            _playerService = playerService;
            _slotIndex = slotIndex;
            CommandName = $"skill{slotIndex + 1}";
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = _playerService.GetPlayer(steamId);
            
            if (playerState == null || playerState.ActiveSkills.Count == 0)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} No active skills!");
                return;
            }
            
            if (_slotIndex < 0 || _slotIndex >= playerState.ActiveSkills.Count)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill slot {_slotIndex + 1} is empty!");
                return;
            }
            
            var skill = playerState.ActiveSkills[_slotIndex];
            
            if (skill.Type == Domain.Skills.SkillType.Passive)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill {skill.DisplayName} is passive!");
                return;
            }
            
            try
            {
                if (_skillService.CanActivateSkill(steamId, skill.Id))
                {
                    _skillService.ActivateSkill(steamId, skill.Id, player);
                }
                else
                {
                    var cooldown = _skillService.GetSkillCooldown(steamId, skill.Id);
                    if (cooldown > 0)
                    {
                        player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} {skill.DisplayName} on cooldown: {cooldown:F1}s");
                    }
                }
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Command Handler für !ultimate
    /// </summary>
    public class UltimateCommand : ICommandHandler
    {
        private readonly ISkillService _skillService;
        private readonly IPlayerService _playerService;
        
        public string CommandName => "ultimate";
        public string Description => "Activate ultimate skill";
        
        public UltimateCommand(ISkillService skillService, IPlayerService playerService)
        {
            _skillService = skillService;
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = _playerService.GetPlayer(steamId);
            
            if (playerState?.UltimateSkill == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} No ultimate skill equipped!");
                return;
            }
            
            var ultimateSkill = playerState.UltimateSkill;
            
            try
            {
                if (_skillService.CanActivateSkill(steamId, ultimateSkill.Id))
                {
                    _skillService.ActivateSkill(steamId, ultimateSkill.Id, player);
                }
                else
                {
                    var cooldown = _skillService.GetSkillCooldown(steamId, ultimateSkill.Id);
                    if (cooldown > 0)
                    {
                        player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} {ultimateSkill.DisplayName} on cooldown: {cooldown:F1}s");
                    }
                }
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
    }
}
