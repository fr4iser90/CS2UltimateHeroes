using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Infrastructure.Events;

namespace UltimateHeroes.Application.EventHandlers
{
    /// <summary>
    /// Handler für Round-bezogene Events (RoundStart, RoundEnd)
    /// </summary>
    public class RoundEventHandler
    {
        private readonly IInMatchEvolutionService _inMatchEvolutionService;
        private readonly IXpService _xpService;
        private readonly IPlayerService _playerService;
        private readonly EventSystem _eventSystem;
        
        // Round Number Tracking (statisch, da pro Match)
        private static int _currentRoundNumber = 0;
        
        public RoundEventHandler(
            IInMatchEvolutionService inMatchEvolutionService,
            IXpService xpService,
            IPlayerService playerService,
            EventSystem eventSystem)
        {
            _inMatchEvolutionService = inMatchEvolutionService;
            _xpService = xpService;
            _playerService = playerService;
            _eventSystem = eventSystem;
        }
        
        /// <summary>
        /// Setzt Round Counter zurück (wird bei Map Start aufgerufen)
        /// </summary>
        public static void ResetRoundCounter()
        {
            _currentRoundNumber = 0;
        }
        
        /// <summary>
        /// Gibt aktuelle Round Number zurück
        /// </summary>
        public static int GetCurrentRoundNumber()
        {
            return _currentRoundNumber;
        }
        
        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            // Erhöhe Round Counter
            _currentRoundNumber++;
            var roundNumber = _currentRoundNumber;
            
            var players = Utilities.GetPlayers();
            
            foreach (var player in players)
            {
                if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                _inMatchEvolutionService.OnRoundStart(steamId, roundNumber);
            }
            
            return HookResult.Continue;
        }
        
        public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            var winner = @event.Winner;
            var players = Utilities.GetPlayers();
            var gameMode = GameModeDetector.DetectCurrentMode();
            
            foreach (var player in players)
            {
                if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                bool won = player.TeamNum == winner;
                
                // In-Match Evolution
                _inMatchEvolutionService.OnRoundEnd(steamId, won);
                
                // Award Round Win XP (wenn gewonnen)
                if (won)
                {
                    _xpService.AwardXp(steamId, XpSource.RoundWin);
                }
            }
            
            return HookResult.Continue;
        }
    }
}
