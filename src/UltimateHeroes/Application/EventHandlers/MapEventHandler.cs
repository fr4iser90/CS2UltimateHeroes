using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Infrastructure.Configuration;

namespace UltimateHeroes.Application.EventHandlers
{
    /// <summary>
    /// Handler für Map-bezogene Events (MapStart)
    /// </summary>
    public class MapEventHandler
    {
        private readonly IXpService _xpService;
        private readonly IShopService _shopService;
        private readonly ISpawnService _spawnService;
        private readonly EffectManager _effectManager;
        private readonly IBuffService _buffService;
        private readonly HudManager _hudManager;
        private readonly IBotService _botService;
        private readonly IInMatchEvolutionService _inMatchEvolutionService;
        private readonly BasePlugin _plugin;
        private readonly PluginConfiguration? _config;
        
        public MapEventHandler(
            IXpService xpService,
            IShopService shopService,
            ISpawnService spawnService,
            EffectManager effectManager,
            IBuffService buffService,
            HudManager hudManager,
            IBotService botService,
            IInMatchEvolutionService inMatchEvolutionService,
            BasePlugin plugin,
            PluginConfiguration? config = null)
        {
            _xpService = xpService;
            _shopService = shopService;
            _spawnService = spawnService;
            _effectManager = effectManager;
            _buffService = buffService;
            _hudManager = hudManager;
            _botService = botService;
            _inMatchEvolutionService = inMatchEvolutionService;
            _plugin = plugin;
            _config = config;
        }
        
        public void OnMapStart(string mapName)
        {
            System.Console.WriteLine($"[UltimateHeroes] Map started: {mapName}");
            
            // Award Match Completion XP für alle Spieler vom vorherigen Match
            var gameMode = GameModeDetector.DetectCurrentMode();
            var allPlayers = Utilities.GetPlayers();
            foreach (var player in allPlayers)
            {
                if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                // Note: Win/Loss wird hier nicht erkannt, daher false
                _xpService.AwardMatchCompletion(steamId, gameMode, false);
            }
            
            // Reset Shop Items für alle Spieler (neues Match)
            _shopService.ResetAllPlayersForNewMatch();
            
            // Remove all spawned entities
            _spawnService.RemoveAllEntities();
            
            // Start effect tick timer
            _plugin.AddTimer(GameConstants.EffectTickInterval, () =>
            {
                _effectManager.TickEffects();
                _buffService.TickBuffs();
                _spawnService.TickEntities();
            }, TimerFlags.REPEAT);
            
            // Start HUD update timer
            _plugin.AddTimer(GameConstants.HudUpdateInterval, () =>
            {
                _hudManager.UpdateHud();
            }, TimerFlags.REPEAT);
            
            // Start Bot Build Change timer
            var botBuildChangeInterval = _config?.BotSettings?.BuildChangeInterval ?? GameConstants.BotBuildChangeInterval;
            _plugin.AddTimer(botBuildChangeInterval, () =>
            {
                var players = Utilities.GetPlayers();
                foreach (var player in players)
                {
                    if (player == null || !player.IsValid) continue;
                    if (_botService != null && _botService.IsBot(player))
                    {
                        string botSteamId = player.AuthorizedSteamID?.SteamId64.ToString() ?? "BOT_" + player.Slot;
                        _botService.CheckBuildChange(botSteamId);
                    }
                }
            }, TimerFlags.REPEAT);
            
            // Start In-Match Evolution timer (for time-based modes)
            if (GameModeDetector.IsTimeBased(gameMode))
            {
                float matchStartTime = Server.CurrentTime;
                _plugin.AddTimer(GameConstants.InMatchEvolutionTickInterval, () =>
                {
                    var players = Utilities.GetPlayers();
                    foreach (var player in players)
                    {
                        if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                        var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                        float minutesElapsed = (Server.CurrentTime - matchStartTime) / 60f;
                        _inMatchEvolutionService.OnTimeUpdate(steamId, minutesElapsed);
                    }
                }, TimerFlags.REPEAT);
            }
            
            // Reset match progress for all players on map start
            foreach (var player in allPlayers)
            {
                if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                _inMatchEvolutionService.ResetMatchProgress(steamId);
            }
        }
    }
}
