using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
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
            PluginConfiguration config,
            BasePlugin? plugin = null)
        {
            if (player == null || !player.IsValid) return;
            
            // Skip bots if they don't have a SteamID (but allow bots with SteamID)
            if (player.AuthorizedSteamID == null && !player.IsBot) return;
            
            var settings = config.LeaderboardSettings;
            if (!settings.Enabled)
            {
                Console.WriteLine($"[PlayerNameHelper] Leaderboard disabled for {player.PlayerName}");
                return;
            }
            
            if (!settings.ShowHero && !settings.ShowLevel)
            {
                Console.WriteLine($"[PlayerNameHelper] Both ShowHero and ShowLevel are false for {player.PlayerName}");
                return;
            }
            
            // Get SteamID - for bots, use a fallback
            string steamId;
            if (player.AuthorizedSteamID != null)
            {
                steamId = player.AuthorizedSteamID.SteamId64.ToString();
            }
            else if (player.IsBot)
            {
                // Bots might not have SteamID, use slot-based ID
                steamId = "BOT_" + player.Slot;
            }
            else
            {
                return; // Skip if no SteamID and not a bot
            }
            // Use GetOrCreatePlayer to ensure player exists
            var playerState = playerService.GetOrCreatePlayer(steamId, player);
            if (playerState == null)
            {
                Console.WriteLine($"[PlayerNameHelper] PlayerState is null for {player.PlayerName} (SteamID: {steamId})");
                return;
            }
            
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
            
            Console.WriteLine($"[PlayerNameHelper] Updating name for {player.PlayerName} -> {playerNameWithPrefix} (Level: {playerState.HeroLevel}, Hero: {playerState.CurrentHero?.DisplayName ?? "None"})");
            
            player.PlayerName = playerNameWithPrefix;
            Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
            
            // Update nach 1 Sekunde erneut (für Scoreboard-Refresh)
            if (plugin != null)
            {
                plugin.AddTimer(1.0f, () =>
                {
                    if (player == null || !player.IsValid) return;
                    player.PlayerName = playerNameWithPrefix;
                    Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
                });
            }
            else
            {
                // Fallback wenn kein Plugin übergeben wurde
                Server.NextFrame(() =>
                {
                    if (player == null || !player.IsValid) return;
                    player.PlayerName = playerNameWithPrefix;
                    Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
                });
            }
        }
        
        /// <summary>
        /// Entfernt den Prefix vom Player-Namen (für echten Namen)
        /// </summary>
        public static string GetRealPlayerName(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || string.IsNullOrEmpty(player.PlayerName))
                return string.Empty;
            
            var currentName = player.PlayerName;
            
            // Entferne Pattern wie "5 [HeroName] RealName"
            // Pattern: Zahl Leerzeichen [Text] Leerzeichen
            var playerNameClean = Regex.Replace(currentName, @"\d+\s\[.*\]\s", "");
            var cleaned = playerNameClean.Trim();
            
            // Falls der Name leer wird oder nur noch Zahlen/Leerzeichen enthält, verwende den originalen Namen
            if (string.IsNullOrEmpty(cleaned) || Regex.IsMatch(cleaned, @"^[\d\s]+$"))
            {
                // Versuche es nochmal mit einem anderen Pattern (falls Format anders ist)
                cleaned = Regex.Replace(currentName, @"^\d+\s*\[[^\]]+\]\s*", "");
                cleaned = cleaned.Trim();
                
                if (string.IsNullOrEmpty(cleaned))
                {
                    // Fallback: Entferne nur Zahlen am Anfang
                    cleaned = Regex.Replace(currentName, @"^\d+\s+", "");
                    cleaned = cleaned.Trim();
                    
                    if (string.IsNullOrEmpty(cleaned))
                    {
                        return currentName;
                    }
                }
            }
            
            return cleaned;
        }
    }
}
