using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Interface für Command Handler (Strategy Pattern)
    /// </summary>
    public interface ICommandHandler
    {
        string CommandName { get; }
        string Description { get; }
        void Handle(CCSPlayerController? player, CommandInfo commandInfo);
    }
}
