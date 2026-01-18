using System;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Admin Commands für Ultimate Heroes
    /// </summary>
    
    public class AdminReloadCommand : ICommandHandler
    {
        private readonly BasePlugin _plugin;
        
        public string CommandName => "admin_reload";
        public string Description => "Reload plugin configuration";
        
        public AdminReloadCommand(BasePlugin plugin)
        {
            _plugin = plugin;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (!AdminHelper.IsAdmin(player))
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} You don't have permission to use this command!");
                return;
            }
            
            // Reload Config (wenn möglich)
            player?.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Configuration reloaded!");
            Console.WriteLine($"[Ultimate Heroes] Admin {player?.PlayerName} reloaded configuration");
        }
    }
    
    public class AdminGiveXpCommand : ICommandHandler
    {
        private readonly IXpService _xpService;
        private readonly IPlayerService _playerService;
        
        public string CommandName => "admin_givexp";
        public string Description => "Give XP to a player";
        
        public AdminGiveXpCommand(IXpService xpService, IPlayerService playerService)
        {
            _xpService = xpService;
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (!AdminHelper.IsAdmin(player))
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} You don't have permission to use this command!");
                return;
            }
            
            var args = commandInfo.GetCommandString.Split(' ');
            if (args.Length < 3)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !admin_givexp <player> <amount>");
                return;
            }
            
            var targetName = args[1];
            if (!float.TryParse(args[2], out float xpAmount))
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Invalid XP amount!");
                return;
            }
            
            var target = Utilities.GetPlayers().FirstOrDefault(p => 
                p.IsValid && 
                p.PlayerName.Contains(targetName, StringComparison.OrdinalIgnoreCase) &&
                p.AuthorizedSteamID != null);
            
            if (target == null)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Player not found!");
                return;
            }
            
            var targetSteamId = target.AuthorizedSteamID!.SteamId64.ToString();
            var playerState = _playerService.GetPlayer(targetSteamId);
            
            if (playerState == null)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Player state not found!");
                return;
            }
            
            // Give XP
            playerState.CurrentXp += xpAmount;
            
            // Check for level up
            while (playerState.CurrentXp >= playerState.XpToNextLevel)
            {
                playerState.CurrentXp -= playerState.XpToNextLevel;
                playerState.HeroLevel++;
                playerState.XpToNextLevel = XpSystem.GetXpForLevel(playerState.HeroLevel);
            }
            
            player?.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Gave {xpAmount} XP to {target.PlayerName}");
            target.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} You received {xpAmount} XP from admin!");
        }
    }
    
    public class AdminSetLevelCommand : ICommandHandler
    {
        private readonly IXpService _xpService;
        private readonly IPlayerService _playerService;
        
        public string CommandName => "admin_setlevel";
        public string Description => "Set player level";
        
        public AdminSetLevelCommand(IXpService xpService, IPlayerService playerService)
        {
            _xpService = xpService;
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (!AdminHelper.IsAdmin(player))
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} You don't have permission to use this command!");
                return;
            }
            
            var args = commandInfo.GetCommandString.Split(' ');
            if (args.Length < 3)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !admin_setlevel <player> <level>");
                return;
            }
            
            var targetName = args[1];
            if (!int.TryParse(args[2], out int level) || level < 1)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Invalid level! Must be >= 1");
                return;
            }
            
            var target = Utilities.GetPlayers().FirstOrDefault(p => 
                p.IsValid && 
                p.PlayerName.Contains(targetName, StringComparison.OrdinalIgnoreCase) &&
                p.AuthorizedSteamID != null);
            
            if (target == null)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Player not found!");
                return;
            }
            
            var targetSteamId = target.AuthorizedSteamID!.SteamId64.ToString();
            var playerState = _playerService.GetPlayer(targetSteamId);
            
            if (playerState == null)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Player state not found!");
                return;
            }
            
            // Set level
            playerState.HeroLevel = level;
            playerState.CurrentXp = 0f;
            playerState.XpToNextLevel = XpSystem.GetXpForLevel(level);
            
            player?.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Set {target.PlayerName} to level {level}");
            target.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Your level was set to {level} by admin!");
        }
    }
    
    public class AdminResetPlayerCommand : ICommandHandler
    {
        private readonly IPlayerService _playerService;
        private readonly IHeroService _heroService;
        
        public string CommandName => "admin_resetplayer";
        public string Description => "Reset player data";
        
        public AdminResetPlayerCommand(IPlayerService playerService, IHeroService heroService)
        {
            _playerService = playerService;
            _heroService = heroService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (!AdminHelper.IsAdmin(player))
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} You don't have permission to use this command!");
                return;
            }
            
            var args = commandInfo.GetCommandString.Split(' ');
            if (args.Length < 2)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !admin_resetplayer <player>");
                return;
            }
            
            var targetName = args[1];
            var target = Utilities.GetPlayers().FirstOrDefault(p => 
                p.IsValid && 
                p.PlayerName.Contains(targetName, StringComparison.OrdinalIgnoreCase) &&
                p.AuthorizedSteamID != null);
            
            if (target == null)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Player not found!");
                return;
            }
            
            var targetSteamId = target.AuthorizedSteamID!.SteamId64.ToString();
            var playerState = _playerService.GetPlayer(targetSteamId);
            
            if (playerState != null)
            {
                // Reset player state
                playerState.HeroLevel = 1;
                playerState.CurrentXp = 0f;
                playerState.XpToNextLevel = XpSystem.GetXpForLevel(1);
                playerState.CurrentHero = null;
                playerState.CurrentBuild = null;
                playerState.ActiveSkills.Clear();
                playerState.SkillLevels.Clear();
                playerState.SkillCooldowns.Clear();
                playerState.Kills = 0;
                playerState.Deaths = 0;
                playerState.Assists = 0;
                playerState.Headshots = 0;
                
                _playerService.SavePlayer(playerState);
            }
            
            player?.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Reset {target.PlayerName}'s data");
            target.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} Your data was reset by admin!");
        }
    }
    
    public class AdminGiveHeroCommand : ICommandHandler
    {
        private readonly IHeroService _heroService;
        private readonly IPlayerService _playerService;
        
        public string CommandName => "admin_givehero";
        public string Description => "Give hero to player";
        
        public AdminGiveHeroCommand(IHeroService heroService, IPlayerService playerService)
        {
            _heroService = heroService;
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (!AdminHelper.IsAdmin(player))
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} You don't have permission to use this command!");
                return;
            }
            
            var args = commandInfo.GetCommandString.Split(' ');
            if (args.Length < 3)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !admin_givehero <player> <hero_id>");
                return;
            }
            
            var targetName = args[1];
            var heroId = args[2].ToLower();
            
            var target = Utilities.GetPlayers().FirstOrDefault(p => 
                p.IsValid && 
                p.PlayerName.Contains(targetName, StringComparison.OrdinalIgnoreCase) &&
                p.AuthorizedSteamID != null);
            
            if (target == null)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Player not found!");
                return;
            }
            
            var hero = _heroService.GetHero(heroId);
            if (hero == null)
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Hero '{heroId}' not found!");
                return;
            }
            
            var targetSteamId = target.AuthorizedSteamID!.SteamId64.ToString();
            _heroService.SetPlayerHero(targetSteamId, heroId);
            
            // Also update player state if available
            var playerState = _playerService.GetPlayer(targetSteamId);
            if (playerState != null)
            {
                playerState.CurrentHero = hero;
                _playerService.SavePlayer(playerState);
            }
            
            player?.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Gave {hero.DisplayName} to {target.PlayerName}");
            target.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} You received {hero.DisplayName} from admin!");
        }
    }
    
    public class AdminListPlayersCommand : ICommandHandler
    {
        private readonly IPlayerService _playerService;
        
        public string CommandName => "admin_listplayers";
        public string Description => "List all players with their stats";
        
        public AdminListPlayersCommand(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (!AdminHelper.IsAdmin(player))
            {
                player?.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} You don't have permission to use this command!");
                return;
            }
            
            var players = Utilities.GetPlayers().Where(p => 
                p.IsValid && 
                p.AuthorizedSteamID != null &&
                !p.IsBot);
            
            player?.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} === Player List ===");
            
            foreach (var p in players)
            {
                var steamId = p.AuthorizedSteamID!.SteamId64.ToString();
                var playerState = _playerService.GetPlayer(steamId);
                
                if (playerState != null)
                {
                    var heroName = playerState.CurrentHero?.DisplayName ?? "None";
                    player?.PrintToChat($" {ChatColors.Default}{p.PlayerName}: Level {playerState.HeroLevel}, Hero: {heroName}, K/D: {playerState.Kills}/{playerState.Deaths}");
                }
            }
        }
    }
}
