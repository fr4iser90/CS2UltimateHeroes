using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Heroes;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Hero Selection Menu
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
            var currentHero = _heroService.GetPlayerHero(steamId);
            
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.Default}     Ultimate Heroes - Heroes     {ChatColors.Green}║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════╝");
            
            if (currentHero != null)
            {
                player.PrintToChat($" {ChatColors.Yellow}Current Hero: {ChatColors.LightBlue}{currentHero.DisplayName}");
            }
            
            player.PrintToChat($"");
            player.PrintToChat($" {ChatColors.Default}Available Heroes:");
            
            int index = 1;
            foreach (var hero in heroes)
            {
                var isCurrent = currentHero?.Id == hero.Id;
                var marker = isCurrent ? $"{ChatColors.Green}[ACTIVE]{ChatColors.Default}" : "";
                player.PrintToChat($" {ChatColors.LightBlue}{index}. {hero.DisplayName}{ChatColors.Default} - {hero.Description} {marker}");
                player.PrintToChat($"    Power Weight: {ChatColors.Yellow}{hero.PowerWeight}{ChatColors.Default} | Tags: {string.Join(", ", hero.Identity.TagModifiers.Keys)}");
                index++;
            }
            
            player.PrintToChat($"");
            player.PrintToChat($" {ChatColors.Gray}Use: !selecthero <number> to select a hero");
        }
    }
}
