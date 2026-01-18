using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Manager für Menu-Operationen
    /// </summary>
    public static class MenuManager
    {
        public static void OpenMainMenu(CCSPlayerController player, Menu menu, int selectedOptionIndex = 0)
        {
            if (player == null || !player.IsValid)
                return;
            MenuAPI.Players[player.Slot].OpenMainMenu(menu, selectedOptionIndex);
        }

        public static void CloseMenu(CCSPlayerController player)
        {
            if (player == null || !player.IsValid)
                return;
            MenuAPI.Players[player.Slot].OpenMainMenu(null);
        }

        public static void CloseSubMenu(CCSPlayerController player)
        {
            if (player == null || !player.IsValid)
                return;
            MenuAPI.Players[player.Slot].CloseSubMenu();
        }

        public static void CloseAllSubMenus(CCSPlayerController player)
        {
            if (player == null || !player.IsValid)
                return;
            MenuAPI.Players[player.Slot].CloseAllSubMenus();
        }

        public static void OpenSubMenu(CCSPlayerController player, Menu menu)
        {
            if (player == null || !player.IsValid)
                return;
            MenuAPI.Players[player.Slot].OpenSubMenu(menu);
        }

        public static Menu CreateMenu(string title = "", int resultsBeforePaging = 5)
        {
            Menu menu = new Menu
            {
                Title = title,
                ResultsBeforePaging = resultsBeforePaging,
            };
            return menu;
        }
    }
}
