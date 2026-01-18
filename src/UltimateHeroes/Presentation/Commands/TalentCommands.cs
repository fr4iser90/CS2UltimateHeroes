using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Presentation.Menu;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Command Handler für Talent-bezogene Commands
    /// </summary>
    public class TalentCommands : ICommandHandler
    {
        private readonly ITalentService _talentService;
        private readonly IPlayerService _playerService;
        private readonly TalentMenu _talentMenu;
        
        public string CommandName => "talents";
        public string Description => "Open talents menu";
        
        public TalentCommands(ITalentService talentService, IPlayerService playerService, TalentMenu talentMenu)
        {
            _talentService = talentService;
            _playerService = playerService;
            _talentMenu = talentMenu;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _talentMenu.ShowMenu(player);
        }
    }
}
