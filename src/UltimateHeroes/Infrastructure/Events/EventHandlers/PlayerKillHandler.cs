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
        private readonly IMasteryService? _masteryService;
        
        public PlayerKillHandler(IXpService xpService, IPlayerService playerService, IMasteryService? masteryService = null)
        {
            _xpService = xpService;
            _playerService = playerService;
            _masteryService = masteryService;
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
            
            // Track Skill Mastery (if kill was with a skill)
            // Note: We need to detect which skill was used for the kill
            // For now, we'll track kills for all active skills (simplified)
            foreach (var skill in player.ActiveSkills)
            {
                _masteryService?.TrackSkillKill(eventData.KillerSteamId, skill.Id);
            }
            
            // Update player stats
            if (eventData.Victim != null)
            {
                player.OnKill(eventData.Victim);
            }
        }
    }
}
