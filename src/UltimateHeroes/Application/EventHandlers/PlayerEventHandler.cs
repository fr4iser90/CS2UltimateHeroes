using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Infrastructure.Configuration;
using UltimateHeroes.Infrastructure.Events;
using UltimateHeroes.Infrastructure.Events.EventHandlers;
using UltimateHeroes.Presentation.UI;

namespace UltimateHeroes.Application.EventHandlers
{
    /// <summary>
    /// Handler für Player-bezogene Events (Connect, Disconnect, Spawn, Death)
    /// </summary>
    public class PlayerEventHandler
    {
        private readonly IPlayerService _playerService;
        private readonly IHeroService _heroService;
        private readonly HudManager _hudManager;
        private readonly EventSystem _eventSystem;
        private readonly string _defaultHero;
        private readonly PluginConfiguration _config;
        private readonly IAccountService? _accountService;
        
        public PlayerEventHandler(
            IPlayerService playerService,
            IHeroService heroService,
            HudManager hudManager,
            EventSystem eventSystem,
            string defaultHero,
            PluginConfiguration config,
            IAccountService? accountService = null)
        {
            _playerService = playerService;
            _heroService = heroService;
            _hudManager = hudManager;
            _eventSystem = eventSystem;
            _defaultHero = defaultHero;
            _config = config;
            _accountService = accountService;
        }
        
        public void OnClientConnect(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            _playerService.OnPlayerConnect(steamId, player);
        }
        
        public void OnClientDisconnect(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            _playerService.OnPlayerDisconnect(steamId);
            
            // Disable HUD for player
            _hudManager.DisableHud(player);
        }
        
        public void OnPlayerSpawn(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = _playerService.GetPlayer(steamId);
            
            // Set default hero if no hero is selected
            if (playerState != null && playerState.CurrentHero == null)
            {
                var defaultHero = _heroService.GetHero(_defaultHero);
                if (defaultHero != null)
                {
                    _heroService.SetPlayerHero(steamId, _defaultHero);
                    playerState.CurrentHero = defaultHero;
                    _playerService.SavePlayer(playerState);
                }
            }
            
            // Enable HUD for player
            _hudManager.EnableHud(player);
            
            _playerService.OnPlayerSpawn(steamId, player);
            
            // Update player name for scoreboard
            Server.NextFrame(() =>
            {
                if (player != null && player.IsValid)
                {
                    PlayerNameHelper.RefreshPlayerName(player, _playerService, _accountService, _config);
                }
            });
        }
        
        public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;
            
            if (attacker != null && attacker.IsValid && attacker.AuthorizedSteamID != null &&
                victim != null && victim.IsValid && victim.AuthorizedSteamID != null)
            {
                var killerSteamId = attacker.AuthorizedSteamID.SteamId64.ToString();
                var victimSteamId = victim.AuthorizedSteamID.SteamId64.ToString();
                var isHeadshot = @event.Headshot;
                
                var killEvent = new PlayerKillEvent
                {
                    KillerSteamId = killerSteamId,
                    VictimSteamId = victimSteamId,
                    Killer = attacker,
                    Victim = victim,
                    IsHeadshot = isHeadshot
                };
                
                // Dispatch event via EventSystem
                _eventSystem.Dispatch(killEvent);
            }
            
            // Update player stats and disable HUD when player dies
            if (victim != null && victim.IsValid && victim.AuthorizedSteamID != null)
            {
                var victimSteamId = victim.AuthorizedSteamID.SteamId64.ToString();
                var playerState = _playerService.GetPlayer(victimSteamId);
                
                if (playerState != null)
                {
                    playerState.Deaths++;
                    _playerService.SavePlayer(playerState);
                }
                
                _hudManager.DisableHud(victim);
            }
            
            return HookResult.Continue;
        }
    }
}
