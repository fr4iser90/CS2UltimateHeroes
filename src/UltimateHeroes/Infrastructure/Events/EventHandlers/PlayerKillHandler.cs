using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Infrastructure.Events.EventHandlers
{
    /// <summary>
    /// Handler für Player Kill Events
    /// </summary>
    public class PlayerKillHandler : IEventHandler<PlayerKillEvent>
    {
        private readonly IXpService _xpService;
        private readonly IPlayerService _playerService;
        
        public PlayerKillHandler(IXpService xpService, IPlayerService playerService)
        {
            _xpService = xpService;
            _playerService = playerService;
        }
        
        public void Handle(PlayerKillEvent eventData)
        {
            var player = _playerService.GetPlayer(eventData.KillerSteamId);
            if (player == null) return;
            
            // Award XP
            _xpService.AwardXp(eventData.KillerSteamId, XpSource.Kill);
            
            if (eventData.IsHeadshot)
            {
                _xpService.AwardXp(eventData.KillerSteamId, XpSource.Headshot);
            }
            
            // Update player stats
            if (eventData.Victim != null)
            {
                player.OnKill(eventData.Victim);
            }
        }
    }
}
