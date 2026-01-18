using System;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Infrastructure.Configuration;
using UltimateHeroes.Presentation.UI;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Command Handler für !stats
    /// </summary>
    public class StatsCommand : ICommandHandler
    {
        private readonly IPlayerService _playerService;
        private readonly IXpService _xpService;
        
        public string CommandName => "stats";
        public string Description => "Show player stats";
        
        public StatsCommand(IPlayerService playerService, IXpService xpService)
        {
            _playerService = playerService;
            _xpService = xpService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = _playerService.GetPlayer(steamId);
            
            if (playerState == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Player not found!");
                return;
            }
            
            var xpProgress = _xpService.GetXpProgress(steamId);
            var xpPercent = (int)(xpProgress * 100);
            
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.Default}   Ultimate Heroes - Stats      {ChatColors.Green}║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════╝");
            player.PrintToChat($" {ChatColors.Yellow}Level: {ChatColors.LightBlue}{playerState.HeroLevel}{ChatColors.Default} | XP: {ChatColors.LightBlue}{playerState.CurrentXp:F0}{ChatColors.Default} ({xpPercent}%)");
            player.PrintToChat($" {ChatColors.Yellow}Current Hero: {ChatColors.LightBlue}{playerState.CurrentHero?.DisplayName ?? "None"}");
            player.PrintToChat($" {ChatColors.Yellow}Active Build: {ChatColors.LightBlue}{playerState.CurrentBuild?.BuildName ?? "None"}");
            player.PrintToChat($"");
            
            if (playerState.ActiveSkills.Count > 0)
            {
                player.PrintToChat($" {ChatColors.Yellow}Active Skills:");
                foreach (var skill in playerState.ActiveSkills)
                {
                    player.PrintToChat($" {ChatColors.Default}  - {skill.DisplayName}");
                }
            }
            
            if (playerState.UltimateSkill != null)
            {
                player.PrintToChat($" {ChatColors.Yellow}Ultimate: {ChatColors.LightBlue}{playerState.UltimateSkill.DisplayName}");
            }
        }
    }
    
    /// <summary>
    /// Command Handler für !botstats
    /// </summary>
    public class BotStatsCommand : ICommandHandler
    {
        private readonly IBotService _botService;
        
        public string CommandName => "botstats";
        public string Description => "Show bot statistics for balancing";
        
        public BotStatsCommand(IBotService botService)
        {
            _botService = botService;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || _botService == null) return;
            
            var allBotStats = _botService.GetAllBotStats();
            
            if (allBotStats.Count == 0)
            {
                player.PrintToChat($" {ChatColors.Yellow}[Bot Stats]{ChatColors.Default} No bot statistics available yet.");
                return;
            }
            
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.Default}   Bot Statistics (Balancing)  {ChatColors.Green}║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════╝");
            
            int totalKills = 0;
            int totalDeaths = 0;
            float totalDamage = 0f;
            int totalRoundsWon = 0;
            int totalRoundsLost = 0;
            
            foreach (var stats in allBotStats)
            {
                totalKills += stats.TotalKills;
                totalDeaths += stats.TotalDeaths;
                totalDamage += stats.TotalDamage;
                totalRoundsWon += stats.RoundsWon;
                totalRoundsLost += stats.RoundsLost;
            }
            
            float avgKDRatio = totalDeaths > 0 ? (float)totalKills / totalDeaths : totalKills;
            float avgWinRate = (totalRoundsWon + totalRoundsLost) > 0 ? (float)totalRoundsWon / (totalRoundsWon + totalRoundsLost) : 0f;
            
            player.PrintToChat($" {ChatColors.Yellow}Total Bots: {ChatColors.LightBlue}{allBotStats.Count}");
            player.PrintToChat($" {ChatColors.Yellow}K/D Ratio: {ChatColors.LightBlue}{avgKDRatio:F2}");
            player.PrintToChat($" {ChatColors.Yellow}Win Rate: {ChatColors.LightBlue}{avgWinRate * 100:F1}%");
            player.PrintToChat($" {ChatColors.Yellow}Total Kills: {ChatColors.LightBlue}{totalKills}");
            player.PrintToChat($" {ChatColors.Yellow}Total Deaths: {ChatColors.LightBlue}{totalDeaths}");
            player.PrintToChat($" {ChatColors.Yellow}Total Damage: {ChatColors.LightBlue}{totalDamage:F0}");
            
            var buildStats = new System.Collections.Generic.Dictionary<string, (int kills, int deaths, int wins, int losses)>();
            foreach (var botStat in allBotStats)
            {
                foreach (var buildPerf in botStat.BuildPerformances)
                {
                    if (!buildStats.ContainsKey(buildPerf.Key))
                    {
                        buildStats[buildPerf.Key] = (0, 0, 0, 0);
                    }
                    var current = buildStats[buildPerf.Key];
                    buildStats[buildPerf.Key] = (
                        current.kills + buildPerf.Value.Kills,
                        current.deaths + buildPerf.Value.Deaths,
                        current.wins + buildPerf.Value.Wins,
                        current.losses + buildPerf.Value.Losses
                    );
                }
            }
            
            if (buildStats.Count > 0)
            {
                player.PrintToChat($"");
                player.PrintToChat($" {ChatColors.Yellow}Build Performance:");
                foreach (var kvp in buildStats.OrderByDescending(x => x.Value.kills))
                {
                    var kd = kvp.Value.deaths > 0 ? (float)kvp.Value.kills / kvp.Value.deaths : kvp.Value.kills;
                    player.PrintToChat($" {ChatColors.Default}Build {kvp.Key}: K/D {kd:F2} ({kvp.Value.kills}K/{kvp.Value.deaths}D)");
                }
            }
        }
    }
    
    /// <summary>
    /// Command Handler für !hud
    /// </summary>
    public class HudCommand : ICommandHandler
    {
        private readonly HudManager _hudManager;
        
        public string CommandName => "hud";
        public string Description => "Toggle HUD display";
        
        public HudCommand(HudManager hudManager)
        {
            _hudManager = hudManager;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            
            // Toggle HUD
            var isEnabled = _hudManager.IsHudEnabled(player);
            if (isEnabled)
            {
                _hudManager.DisableHud(player);
                player.PrintToChat($" {ChatColors.Yellow}[HUD]{ChatColors.Default} HUD disabled!");
            }
            else
            {
                _hudManager.EnableHud(player);
                player.PrintToChat($" {ChatColors.Green}[HUD]{ChatColors.Default} HUD enabled!");
            }
        }
    }
    
    /// <summary>
    /// Command Handler für !leaderboard
    /// </summary>
    public class LeaderboardCommand : ICommandHandler
    {
        private readonly IPlayerService _playerService;
        private readonly IAccountService? _accountService;
        private readonly PluginConfiguration _config;
        
        public string CommandName => "leaderboard";
        public string Description => "Show player leaderboard with heroes and levels";
        
        public LeaderboardCommand(IPlayerService playerService, IAccountService? accountService, PluginConfiguration config)
        {
            _playerService = playerService;
            _accountService = accountService;
            _config = config;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            
            var settings = _config.LeaderboardSettings;
            
            if (!settings.Enabled)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Leaderboard is disabled!");
                return;
            }
            
            var players = Utilities.GetPlayers()
                .Where(p => p != null && p.IsValid && p.AuthorizedSteamID != null && !p.IsBot)
                .ToList();
            
            if (players.Count == 0)
            {
                player.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} No players found!");
                return;
            }
            
            // Get player data
            var playerData = players
                .Select(p =>
                {
                    var steamId = p.AuthorizedSteamID!.SteamId64.ToString();
                    var playerState = _playerService.GetPlayer(steamId);
                    if (playerState == null)
                    {
                        Console.WriteLine($"[LeaderboardCommand] PlayerState is null for {p.PlayerName} (SteamID: {steamId})");
                        return null;
                    }
                    
                    int level = 0;
                    if (settings.ShowLevel)
                    {
                        if (settings.LevelType == "Account" && _accountService != null)
                        {
                            level = _accountService.GetAccountLevelValue(steamId);
                        }
                        else
                        {
                            level = playerState.HeroLevel;
                        }
                    }
                    
                    return new
                    {
                        Player = p,
                        PlayerState = playerState,
                        Level = level,
                        HeroName = playerState.CurrentHero?.DisplayName ?? "None"
                    };
                })
                .Where(p => p != null)
                .OrderByDescending(p => p!.Level)
                .ThenBy(p => p!.Player.PlayerName)
                .Take(settings.MaxPlayers)
                .ToList();
            
            if (playerData.Count == 0)
            {
                player.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} No player data available!");
                return;
            }
            
            // Display leaderboard
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.Default}   Ultimate Heroes - Leaderboard      {ChatColors.Green}║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════════════╝");
            
            int rank = 1;
            foreach (var data in playerData)
            {
                var name = data!.Player.PlayerName;
                var heroInfo = settings.ShowHero ? $" | {data.HeroName}" : "";
                var levelInfo = settings.ShowLevel ? $"Lv.{data.Level}" : "";
                
                var display = $"{rank}. {name}";
                if (settings.ShowLevel && settings.ShowHero)
                {
                    display += $" - {levelInfo}{heroInfo}";
                }
                else if (settings.ShowLevel)
                {
                    display += $" - {levelInfo}";
                }
                else if (settings.ShowHero)
                {
                    display += $" - {data.HeroName}";
                }
                
                player.PrintToChat($" {ChatColors.Yellow}{display}");
                rank++;
            }
        }
    }
}
