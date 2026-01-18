using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Presentation.Menu
{
    /// <summary>
    /// Menu API - Verwaltet interaktive Menus für alle Spieler
    /// </summary>
    internal static class MenuAPI
    {
        internal static readonly Dictionary<int, MenuPlayer> Players = new();

        internal static void Load(BasePlugin plugin)
        {
            Server.NextFrame(() =>
            {
                foreach (var pl in Utilities.GetPlayers())
                {
                    if (pl.IsValid)
                    {
                        Players[pl.Slot] = new MenuPlayer
                        {
                            Player = pl,
                            Buttons = pl.Buttons
                        };
                    }
                }
            });

            plugin.RegisterEventHandler<EventPlayerActivate>((@event, info) =>
            {
                if (@event.Userid != null && @event.Userid.IsValid)
                {
                    Players[@event.Userid.Slot] = new MenuPlayer
                    {
                        Player = @event.Userid,
                        Buttons = 0
                    };
                }
                return HookResult.Continue;
            });

            plugin.RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
            {
                if (@event.Userid != null)
                {
                    Players.Remove(@event.Userid.Slot);
                }
                return HookResult.Continue;
            });

            plugin.RegisterListener<Listeners.OnTick>(OnTick);
        }

        internal static void OnTick()
        {
            foreach (var player in Players.Values.Where(p => p.MainMenu != null && p.Player != null && p.Player.IsValid))
            {
                // Forward (W) - Scroll Up
                if ((player.Buttons & PlayerButtons.Forward) == 0 && (player.Player.Buttons & PlayerButtons.Forward) != 0)
                {
                    player.ScrollUp();
                }
                // Back (S) - Scroll Down
                else if ((player.Buttons & PlayerButtons.Back) == 0 && (player.Player.Buttons & PlayerButtons.Back) != 0)
                {
                    player.ScrollDown();
                }
                // Jump (Space) - Choose
                else if ((player.Buttons & PlayerButtons.Jump) == 0 && (player.Player.Buttons & PlayerButtons.Jump) != 0)
                {
                    player.Choose();
                }
                // Use (E) - Choose
                else if ((player.Buttons & PlayerButtons.Use) == 0 && (player.Player.Buttons & PlayerButtons.Use) != 0)
                {
                    player.Choose();
                }

                // M (8589934592) - Open Main Menu
                if (((long)player.Player.Buttons & 8589934592) == 8589934592)
                {
                    // Could open main menu here if needed
                }

                player.Buttons = player.Player.Buttons;
                if (!string.IsNullOrEmpty(player.CenterHtml))
                {
                    Server.NextFrame(() =>
                    {
                        if (player.Player != null && player.Player.IsValid)
                        {
                            player.Player.PrintToCenterHtml(player.CenterHtml);
                        }
                    });
                }
            }
        }
    }
}
