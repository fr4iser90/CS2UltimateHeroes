using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Menu State für einen einzelnen Spieler
    /// </summary>
    internal class MenuPlayer
    {
        internal CCSPlayerController Player { get; set; }
        internal Menu MainMenu = null;
        internal LinkedListNode<MenuOption> CurrentChoice = null;
        internal LinkedListNode<MenuOption> MenuStart = null;
        internal string CenterHtml = "";
        internal int VisibleOptions = 5;
        internal PlayerButtons Buttons { get; set; }

        internal void OpenMainMenu(Menu menu, int selectedOptionIndex = 0)
        {
            if (Player == null || !Player.IsValid) return;
            
            Player.DisableMovement();

            if (menu == null)
            {
                Player.EnableMovement();
                MainMenu = null;
                CurrentChoice = null;
                CenterHtml = "";
                return;
            }
            
            MainMenu = menu;
            VisibleOptions = menu.ResultsBeforePaging;
            MenuStart = MainMenu.Options?.First;
            CurrentChoice = MenuStart;

            // Set the selected option based on index
            for (int i = 0; i < selectedOptionIndex && CurrentChoice != null; i++)
            {
                CurrentChoice = CurrentChoice.Next;
            }

            CurrentChoice?.Value.OnSelect?.Invoke(Player, CurrentChoice.Value);
            UpdateCenterHtml();
        }

        internal void OpenSubMenu(Menu menu)
        {
            if (menu == null)
            {
                CurrentChoice = MainMenu?.Options?.First;
                MenuStart = CurrentChoice;
                UpdateCenterHtml();
                return;
            }

            VisibleOptions = menu.ResultsBeforePaging;
            CurrentChoice = menu.Options?.First;
            MenuStart = CurrentChoice;
            UpdateCenterHtml();
        }

        internal void CloseSubMenu()
        {
            if (CurrentChoice?.Value.Parent?.Prev == null)
            {
                if (Player != null && Player.IsValid && Player.PlayerPawn.Value != null && Player.PlayerPawn.Value.IsValid)
                {
                    Player.EnableMovement();
                }
                return;
            }
            GoBackToPrev(CurrentChoice?.Value.Parent.Prev);
        }

        internal void CloseAllSubMenus()
        {
            OpenSubMenu(null);
        }

        internal void Choose()
        {
            CurrentChoice?.Value.OnChoose?.Invoke(Player, CurrentChoice.Value);
        }

        internal void ScrollDown()
        {
            if (CurrentChoice == null || MainMenu == null)
                return;
            CurrentChoice = CurrentChoice.Next ?? CurrentChoice.List?.First;
            MenuStart = CurrentChoice!.Value.Index >= VisibleOptions ? MenuStart!.Next : CurrentChoice.List?.First;

            CurrentChoice?.Value.OnSelect?.Invoke(Player, CurrentChoice.Value);
            UpdateCenterHtml();
        }

        internal void ScrollUp()
        {
            if (CurrentChoice == null || MainMenu == null)
                return;
            CurrentChoice = CurrentChoice.Previous ?? CurrentChoice.List?.Last;
            if (CurrentChoice == CurrentChoice?.List?.Last && CurrentChoice?.Value.Index >= VisibleOptions)
            {
                MenuStart = CurrentChoice;
                for (int i = 0; i < VisibleOptions - 1; i++)
                    MenuStart = MenuStart?.Previous;
            }
            else
                MenuStart = CurrentChoice!.Value.Index >= VisibleOptions ? MenuStart!.Previous : CurrentChoice.List?.First;

            CurrentChoice?.Value.OnSelect?.Invoke(Player, CurrentChoice.Value);
            UpdateCenterHtml();
        }

        private void GoBackToPrev(LinkedListNode<MenuOption> menu)
        {
            if (menu == null)
            {
                CurrentChoice = MainMenu?.Options?.First;
                MenuStart = CurrentChoice;
                UpdateCenterHtml();
                return;
            }

            VisibleOptions = menu.Value.Parent?.ResultsBeforePaging ?? 4;
            CurrentChoice = menu;
            if (CurrentChoice.Value.Index >= 5)
            {
                MenuStart = CurrentChoice;
                for (int i = 0; i < 4; i++)
                {
                    MenuStart = MenuStart?.Previous;
                }
            }
            else
                MenuStart = CurrentChoice.List?.First;
            UpdateCenterHtml();
        }

        private void UpdateCenterHtml()
        {
            if (CurrentChoice == null || MainMenu == null)
                return;

            StringBuilder builder = new StringBuilder();
            int i = 0;
            LinkedListNode<MenuOption> option = MenuStart!;
            builder.AppendLine($"{option.Value.Parent?.Title}<br>");

            while (i < VisibleOptions && option != null)
            {
                if (option == CurrentChoice)
                {
                    builder.AppendLine($"<font color='orange'>></font> {option.Value.OptionDisplay} <font color='orange'><</font><br>");
                    if (!string.IsNullOrEmpty(option.Value.SubOptionDisplay))
                        builder.AppendLine($"{option.Value.SubOptionDisplay}<br>");
                }
                else
                {
                    builder.AppendLine($"{option.Value.OptionDisplay}<br>");
                }
                option = option.Next;
                i++;
            }

            if (option != null)
            {
                builder.AppendLine($"<font color='grey' class='fontSize-sm'>More options below...</font>");
            }
            if (option == null && MenuStart.List.Count > VisibleOptions)
            {
                builder.AppendLine($"<center><img src='https://dummyimage.com/1x16/000/fff'></center><br>");
            }

            if (!string.IsNullOrEmpty(CurrentChoice?.Value?.SubOptionDisplay))
            {
                var subOptionTextSpace = CalculateTextSpace(CurrentChoice?.Value?.SubOptionDisplay);
                if (subOptionTextSpace < 56)
                {
                    builder.AppendLine($"<font class='fontSize-m'>ㅤ</font><br>");
                }
                else
                {
                    builder.AppendLine($"<font class='fontSize-xs'>ㅤ</font><br>");
                }
            }

            builder.AppendLine($"<center><font color='red' class='fontSize-sm'>Navigate:</font><font color='orange' class='fontSize-s'> W / S</font><font color='white' class='fontSize-sm'> | </font><font color='red' class='fontSize-sm'>Select: </font><font color='orange' class='fontSize-sm'>Space / E</font><font color='white' class='fontSize-sm'> | </font><font color='red' class='fontSize-sm'>Exit: </font><font color='orange' class='fontSize-sm'>M</font></center>");
            builder.AppendLine("<br>");
            CenterHtml = builder.ToString();
        }

        private static int CalculateTextSpace(string subOptionDisplay)
        {
            if (string.IsNullOrEmpty(subOptionDisplay))
                return 0;
                
            // Remove HTML tags
            string pattern = @"<[^>]+>";
            string cleanedString = Regex.Replace(subOptionDisplay, pattern, string.Empty);
            return cleanedString.Length;
        }
    }
}
