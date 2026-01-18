using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Infrastructure.Configuration;

namespace UltimateHeroes.Application.Helpers
{
    /// <summary>
    /// Helper für Player Name Updates (Scoreboard)
    /// </summary>
    public static class PlayerNameHelper
    {
        /// <summary>
        /// Aktualisiert den Player-Namen mit Hero und Level für das Scoreboard
        /// </summary>
        public static void RefreshPlayerName(
            CCSPlayerController player,
            IPlayerService playerService,
            IAccountService? accountService,
            PluginConfiguration config)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var settings = config.LeaderboardSettings;
            if (!settings.Enabled || !settings.ShowHero && !settings.ShowLevel) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = playerService.GetPlayer(steamId);
            if (playerState == null) return;
            
            var playerNameClean = GetRealPlayerName(player);
            var parts = new List<string>();
            
            // Level hinzufügen
            if (settings.ShowLevel)
            {
                int level = 0;
                if (settings.LevelType == "Account" && accountService != null)
                {
                    level = accountService.GetAccountLevelValue(steamId);
                }
                else
                {
                    level = playerState.HeroLevel;
                }
                parts.Add(level.ToString());
            }
            
            // Hero hinzufügen
            if (settings.ShowHero && playerState.CurrentHero != null)
            {
                parts.Add($"[{playerState.CurrentHero.DisplayName}]");
            }
            
            // Player Name zusammenbauen
            var playerNameWithPrefix = parts.Count > 0 
                ? $"{string.Join(" ", parts)} {playerNameClean}"
                : playerNameClean;
            
            player.PlayerName = playerNameWithPrefix;
            Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
            
            // Update nach 1 Sekunde erneut (für Scoreboard-Refresh)
            Server.NextFrame(() =>
            {
                if (player == null || !player.IsValid) return;
                player.PlayerName = playerNameWithPrefix;
                Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
            });
        }
        
        /// <summary>
        /// Entfernt den Prefix vom Player-Namen (für echten Namen)
        /// </summary>
        public static string GetRealPlayerName(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || string.IsNullOrEmpty(player.PlayerName))
                return string.Empty;
            
            // Entferne Pattern wie "5 [HeroName] RealName" oder "5 RealName"
            var playerNameClean = Regex.Replace(player.PlayerName, @"^\d+\s*(\[.*?\]\s*)?", "");
            return playerNameClean.Trim();
        }
    }
}
