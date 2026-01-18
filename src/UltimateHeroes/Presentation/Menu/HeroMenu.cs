using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Heroes;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Hero Selection Menu (Interaktiv mit HTML)
    /// </summary>
    public class HeroMenu
    {
        private readonly IHeroService _heroService;
        private readonly IPlayerService _playerService;
        
        public HeroMenu(IHeroService heroService, IPlayerService playerService)
        {
            _heroService = heroService;
            _playerService = playerService;
        }
        
        public void ShowMenu(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var heroes = _heroService.GetAllHeroes();
            
            if (heroes.Count == 0)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} No heroes available! Please contact an admin.");
                Console.WriteLine($"[HeroMenu] WARNING: No heroes found for player {player.PlayerName}");
                return;
            }
            
            var currentHero = _heroService.GetPlayerHero(steamId);
            
            var heroMenu = MenuManager.CreateMenu($"<font color='lightgrey' class='fontSize-m'>Ultimate Heroes - Hero Selection</font>", 5);
            
            foreach (var hero in heroes)
            {
                var isCurrent = currentHero?.Id == hero.Id;
                var status = isCurrent ? "<font color='green'>[ACTIVE]</font>" : "";
                var tags = string.Join(", ", hero.Identity.TagModifiers.Keys);
                
                var display = $"<font color='lightblue'>{hero.DisplayName}</font> {status}";
                var subDisplay = $"<font color='grey' class='fontSize-sm'>{hero.Description}<br>Power: <font color='yellow'>{hero.PowerWeight}</font> | Tags: {tags}</font>";
                
                var heroId = hero.Id;
                heroMenu.Add(display, subDisplay, (p, opt) =>
                {
                    if (!isCurrent)
                    {
                        _heroService.SetPlayerHero(steamId, heroId);
                        var playerState = _playerService.GetPlayer(steamId);
                        if (playerState != null)
                        {
                            playerState.CurrentHero = hero;
                            _playerService.SavePlayer(playerState);
                        }
                        MenuManager.CloseMenu(p);
                        p.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Hero selected: {hero.DisplayName}");
                    }
                    else
                    {
                        p.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} This hero is already active!");
                    }
                });
            }
            
            MenuManager.OpenMainMenu(player, heroMenu);
        }
    }
}
