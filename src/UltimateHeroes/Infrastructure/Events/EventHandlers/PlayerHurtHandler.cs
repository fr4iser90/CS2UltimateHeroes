using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Infrastructure.Events;

namespace UltimateHeroes.Infrastructure.Events.EventHandlers
{
    /// <summary>
    /// Handler für Player Hurt Events
    /// </summary>
    public class PlayerHurtHandler : IEventHandler<PlayerHurtEvent>
    {
        private readonly IPlayerService _playerService;
        private readonly AssistTracking _assistTracking;
        
        public PlayerHurtHandler(IPlayerService playerService, AssistTracking assistTracking)
        {
            _playerService = playerService;
            _assistTracking = assistTracking;
        }
        
        public void Handle(PlayerHurtEvent eventData)
        {
            var player = _playerService.GetPlayer(eventData.AttackerSteamId);
            if (player == null) return;
            
            // Record damage for assist tracking
            if (!string.IsNullOrEmpty(eventData.VictimSteamId))
            {
                _assistTracking.RecordDamage(eventData.AttackerSteamId, eventData.VictimSteamId, eventData.Damage);
            }
            
            // Trigger Passive Skills
            foreach (var skill in player.ActiveSkills)
            {
                if (skill is IPassiveSkill passiveSkill && eventData.Player != null)
                {
                    passiveSkill.OnPlayerHurt(eventData.Player, eventData.Damage);
                }
            }
        }
    }
}
