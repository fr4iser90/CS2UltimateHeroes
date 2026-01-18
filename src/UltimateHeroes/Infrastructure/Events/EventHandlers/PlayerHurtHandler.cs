using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Infrastructure.Events.EventHandlers
{
    /// <summary>
    /// Handler für Player Hurt Events
    /// </summary>
    public class PlayerHurtHandler : IEventHandler<PlayerHurtEvent>
    {
        private readonly IPlayerService _playerService;
        
        public PlayerHurtHandler(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        
        public void Handle(PlayerHurtEvent eventData)
        {
            var player = _playerService.GetPlayer(eventData.AttackerSteamId);
            if (player == null) return;
            
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
