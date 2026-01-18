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
        private readonly Application.Services.IInMatchEvolutionService? _inMatchEvolutionService;
        private readonly Application.Services.IBotService? _botService;
        
        public PlayerKillHandler(IXpService xpService, IPlayerService playerService, IMasteryService? masteryService = null, Application.Services.IInMatchEvolutionService? inMatchEvolutionService = null, Application.Services.IBotService? botService = null)
        {
            _xpService = xpService;
            _playerService = playerService;
            _masteryService = masteryService;
            _inMatchEvolutionService = inMatchEvolutionService;
            _botService = botService;
        }
        
        public void Handle(PlayerKillEvent eventData)
        {
            var player = _playerService.GetPlayer(eventData.KillerSteamId);
            if (player == null) return;
            
            // Anti-Exploit: Kill-Diminishing (gemäß LEVELING.md)
            var victimSteamId = eventData.VictimSteamId;
            player.KillTracking.RecordKill(victimSteamId);
            var killMultiplier = player.KillTracking.GetKillXpMultiplier(victimSteamId);
            
            // Award XP mit Kill-Diminishing
            var baseXp = Domain.Progression.XpSystem.XpPerKill;
            var adjustedXp = baseXp * killMultiplier;
            _xpService.AwardXp(eventData.KillerSteamId, XpSource.Kill, adjustedXp);
            
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
            
            // Track kill for In-Match Evolution
            _inMatchEvolutionService?.OnKill(eventData.KillerSteamId);
            
            // Track death for victim
            _inMatchEvolutionService?.OnDeath(eventData.VictimSteamId);
            
            // Track bot stats
            if (_botService != null && _botService.IsBot(eventData.KillerSteamId))
            {
                // Find which skill was used (simplified - track all active skills)
                string? skillId = null;
                foreach (var skill in player.ActiveSkills)
                {
                    if (skill.Type != SkillType.Passive)
                    {
                        skillId = skill.Id;
                        break; // Use first active skill
                    }
                }
                _botService.OnBotKill(eventData.KillerSteamId, skillId ?? "", 0f);
            }
            
            if (_botService != null && _botService.IsBot(eventData.VictimSteamId))
            {
                _botService.OnBotDeath(eventData.VictimSteamId);
            }
        }
    }
}
