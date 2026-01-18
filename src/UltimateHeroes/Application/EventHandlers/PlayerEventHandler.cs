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
        private readonly BasePlugin? _plugin;
        
        public PlayerEventHandler(
            IPlayerService playerService,
            IHeroService heroService,
            HudManager hudManager,
            EventSystem eventSystem,
            string defaultHero,
            PluginConfiguration config,
            IAccountService? accountService = null,
            BasePlugin? plugin = null)
        {
            _playerService = playerService;
            _heroService = heroService;
            _hudManager = hudManager;
            _eventSystem = eventSystem;
            _defaultHero = defaultHero;
            _config = config;
            _accountService = accountService;
            _plugin = plugin;
        }
        
        public void OnClientConnect(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            if (player == null || !player.IsValid) return;
            
            // Get SteamID - for bots, use a fallback
            string steamId;
            if (player.AuthorizedSteamID != null)
            {
                steamId = player.AuthorizedSteamID.SteamId64.ToString();
            }
            else if (player.IsBot)
            {
                // Generate bot ID based on name
                steamId = GetBotId(player);
            }
            else
            {
                return; // Skip if no SteamID and not a bot
            }
            
            // Check if bot and get XP persistence setting
            bool isBot = player.IsBot;
            string? botXpPersistence = isBot ? _config.BotSettings.XpPersistence : null;
            
            // Get or create player with bot XP persistence
            _playerService.GetOrCreatePlayer(steamId, player, isBot, botXpPersistence);
            _playerService.OnPlayerConnect(steamId, player);
            
            // Update player name after connect
            Server.NextFrame(() =>
            {
                if (player != null && player.IsValid)
                {
                    PlayerNameHelper.RefreshPlayerName(player, _playerService, _accountService, _config, _plugin);
                }
            });
        }
        
        /// <summary>
        /// Generates a unique ID for bots based on their name
        /// </summary>
        private string GetBotId(CCSPlayerController player)
        {
            string botName = player.PlayerName ?? $"Bot_{player.Slot}";
            // Use simple hash for bot ID (consistent for same bot name)
            return "BOT_" + botName.GetHashCode().ToString("X");
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
            if (player == null || !player.IsValid) return;
            
            // Get SteamID - for bots, use a fallback
            string steamId;
            if (player.AuthorizedSteamID != null)
            {
                steamId = player.AuthorizedSteamID.SteamId64.ToString();
            }
            else if (player.IsBot)
            {
                steamId = GetBotId(player);
            }
            else
            {
                return; // Skip if no SteamID and not a bot
            }
            
            // Use GetOrCreatePlayer to ensure bot exists
            // Apply bot XP persistence if it's a bot
            bool isBot = player.IsBot;
            string? botXpPersistence = isBot ? _config.BotSettings.XpPersistence : null;
            var playerState = _playerService.GetOrCreatePlayer(steamId, player, isBot, botXpPersistence);
            
            // Set default hero if no hero is selected (for both players and bots)
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
            
            // For bots: ensure they have a hero immediately
            if (player.IsBot && playerState != null && playerState.CurrentHero == null)
            {
                var allHeroes = _heroService.GetAllHeroes();
                if (allHeroes.Count > 0)
                {
                    // Assign random hero to bot
                    var randomHero = allHeroes[new System.Random().Next(allHeroes.Count)];
                    _heroService.SetPlayerHero(steamId, randomHero.Id);
                    playerState.CurrentHero = randomHero;
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
                    PlayerNameHelper.RefreshPlayerName(player, _playerService, _accountService, _config, _plugin);
                }
            });
        }
        
        public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;
            
            // Handle kill event (for both players and bots)
            if (attacker != null && attacker.IsValid && victim != null && victim.IsValid)
            {
                string? killerSteamId = null;
                if (attacker.AuthorizedSteamID != null)
                {
                    killerSteamId = attacker.AuthorizedSteamID.SteamId64.ToString();
                }
                else if (attacker.IsBot)
                {
                    killerSteamId = GetBotId(attacker);
                }
                
                string? victimSteamId = null;
                if (victim.AuthorizedSteamID != null)
                {
                    victimSteamId = victim.AuthorizedSteamID.SteamId64.ToString();
                }
                else if (victim.IsBot)
                {
                    victimSteamId = GetBotId(victim);
                }
                
                if (killerSteamId != null && victimSteamId != null)
                {
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
            }
            
            // Update player stats and disable HUD when player dies (for both players and bots)
            if (victim != null && victim.IsValid)
            {
                string? victimSteamId = null;
                if (victim.AuthorizedSteamID != null)
                {
                    victimSteamId = victim.AuthorizedSteamID.SteamId64.ToString();
                }
                else if (victim.IsBot)
                {
                    victimSteamId = GetBotId(victim);
                }
                
                if (victimSteamId != null)
                {
                    var playerState = _playerService.GetPlayer(victimSteamId);
                    
                    if (playerState != null)
                    {
                        playerState.Deaths++;
                        _playerService.SavePlayer(playerState);
                    }
                    
                    _hudManager.DisableHud(victim);
                }
            }
            
            return HookResult.Continue;
        }
    }
}
