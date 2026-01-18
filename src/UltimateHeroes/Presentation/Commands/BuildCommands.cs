using System;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Presentation.Menu;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Command Handler für Build-bezogene Commands
    /// </summary>
    public class BuildCommands : ICommandHandler
    {
        private readonly IBuildService _buildService;
        private readonly IPlayerService _playerService;
        private readonly BuildMenu _buildMenu;
        
        public string CommandName => "build";
        public string Description => "Open build menu";
        
        public BuildCommands(IBuildService buildService, IPlayerService playerService, BuildMenu buildMenu)
        {
            _buildService = buildService;
            _playerService = playerService;
            _buildMenu = buildMenu;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _buildMenu.ShowMenu(player);
        }
    }
    
    /// <summary>
    /// Command Handler für !createbuild
    /// </summary>
    public class CreateBuildCommand : ICommandHandler
    {
        private readonly IBuildService _buildService;
        private readonly IPlayerService _playerService;
        
        public string CommandName => "createbuild";
        public string Description => "Create a build";
        
        public CreateBuildCommand(IBuildService buildService, IPlayerService playerService)
        {
            _buildService = buildService;
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var args = commandInfo.GetCommandString.Split(' ');
            
            if (args.Length < 2)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !createbuild <build_name>");
                return;
            }
            
            var buildName = string.Join(" ", args.Skip(1));
            var playerState = _playerService.GetPlayer(steamId);
            
            if (playerState?.CurrentHero == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Please select a hero first!");
                return;
            }
            
            try
            {
                // Find next available slot
                var builds = _buildService.GetPlayerBuilds(steamId);
                var nextSlot = builds.Count > 0 ? builds.Max(b => b.BuildSlot) + 1 : 1;
                
                var build = _buildService.CreateBuild(
                    steamId,
                    nextSlot,
                    playerState.CurrentHero.Id,
                    playerState.ActiveSkills.Where(s => s.Type == SkillType.Active).Select(s => s.Id).ToList(),
                    playerState.UltimateSkill?.Id,
                    playerState.PassiveSkills.Select(s => s.Id).ToList(),
                    buildName
                );
                
                player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Build '{buildName}' created!");
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Command Handler für !activatebuild
    /// </summary>
    public class ActivateBuildCommand : ICommandHandler
    {
        private readonly IBuildService _buildService;
        private readonly IPlayerService _playerService;
        
        public string CommandName => "activatebuild";
        public string Description => "Activate a build";
        
        public ActivateBuildCommand(IBuildService buildService, IPlayerService playerService)
        {
            _buildService = buildService;
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var args = commandInfo.GetCommandString.Split(' ');
            
            if (args.Length < 2)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !activatebuild <build_name>");
                return;
            }
            
            var buildName = string.Join(" ", args.Skip(1));
            
            try
            {
                // Find build by name
                var builds = _buildService.GetPlayerBuilds(steamId);
                var build = builds.FirstOrDefault(b => b.BuildName.Equals(buildName, StringComparison.OrdinalIgnoreCase));
                
                if (build == null)
                {
                    player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Build '{buildName}' not found!");
                    return;
                }
                
                _buildService.ActivateBuild(steamId, build.BuildSlot, player);
                player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Build '{buildName}' activated!");
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
    }
}
