using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Presentation.Menu;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Command Handler für Hero-bezogene Commands
    /// </summary>
    public class HeroCommands : ICommandHandler
    {
        private readonly IHeroService _heroService;
        private readonly IPlayerService _playerService;
        private readonly HeroMenu _heroMenu;
        
        public string CommandName => "hero";
        public string Description => "Open hero selection menu";
        
        public HeroCommands(IHeroService heroService, IPlayerService playerService, HeroMenu heroMenu)
        {
            _heroService = heroService;
            _playerService = playerService;
            _heroMenu = heroMenu;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _heroMenu.ShowMenu(player);
        }
    }
    
    /// <summary>
    /// Command Handler für !selecthero
    /// </summary>
    public class SelectHeroCommand : ICommandHandler
    {
        private readonly IHeroService _heroService;
        private readonly IPlayerService _playerService;
        
        public string CommandName => "selecthero";
        public string Description => "Select a hero";
        
        public SelectHeroCommand(IHeroService heroService, IPlayerService playerService)
        {
            _heroService = heroService;
            _playerService = playerService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var args = commandInfo.GetCommandString.Split(' ');
            
            if (args.Length < 2)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !selecthero <hero_id>");
                return;
            }
            
            var heroId = args[1].ToLower();
            var hero = _heroService.GetHero(heroId);
            
            if (hero == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Hero '{heroId}' not found!");
                return;
            }
            
            try
            {
                _heroService.SetPlayerHero(steamId, heroId);
                var playerState = _playerService.GetPlayer(steamId);
                if (playerState != null)
                {
                    playerState.CurrentHero = hero;
                    _playerService.SavePlayer(playerState);
                }
                
                player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Hero selected: {hero.DisplayName}!");
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
    }
}
