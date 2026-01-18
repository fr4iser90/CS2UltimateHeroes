using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Base class für verbesserte Menus mit besserem Formatting
    /// </summary>
    public abstract class MenuBase
    {
        protected void PrintHeader(CCSPlayerController player, string title)
        {
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.White} {title.PadRight(38)}{ChatColors.Green} ║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════════════╝");
        }
        
        protected void PrintSection(CCSPlayerController player, string sectionName)
        {
            player.PrintToChat($"");
            player.PrintToChat($" {ChatColors.Yellow}▶ {sectionName}");
        }
        
        protected void PrintItem(CCSPlayerController player, string text, bool isActive = false)
        {
            var marker = isActive ? $"{ChatColors.Green}● {ChatColors.Default}" : "  ";
            player.PrintToChat($" {marker}{text}");
        }
        
        protected void PrintInfo(CCSPlayerController player, string text)
        {
            player.PrintToChat($" {ChatColors.Gray}ℹ {text}");
        }
        
        protected void PrintSuccess(CCSPlayerController player, string text)
        {
            player.PrintToChat($" {ChatColors.Green}✓ {text}");
        }
        
        protected void PrintError(CCSPlayerController player, string text)
        {
            player.PrintToChat($" {ChatColors.Red}✗ {text}");
        }
        
        protected void PrintSeparator(CCSPlayerController player)
        {
            player.PrintToChat($" {ChatColors.Gray}────────────────────────────────────────");
        }
    }
}
