using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für In-Match Evolution
    /// </summary>
    public class InMatchEvolutionService : IInMatchEvolutionService
    {
        private readonly Dictionary<string, MatchProgress> _matchProgress = new();
        private readonly IPlayerService _playerService;
        
        // Upgrade Intervals
        private const int RoundUpgradeInterval = 3; // Alle 3 Runden
        private const float TimeUpgradeInterval = 2f; // Alle 2 Minuten (für Deathmatch)
        
        public InMatchEvolutionService(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        
        public void OnRoundStart(string steamId, int roundNumber)
        {
            var progress = GetOrCreateProgress(steamId);
            progress.CurrentRound = roundNumber;
            
            // Mini-Upgrade alle 3 Runden
            if (roundNumber > 0 && roundNumber % RoundUpgradeInterval == 0)
            {
                AwardMiniUpgrade(steamId, $"round_{roundNumber}");
            }
        }
        
        public void OnRoundEnd(string steamId, bool won)
        {
            var progress = GetOrCreateProgress(steamId);
            
            if (won)
            {
                // Bonus für Round Win
                AwardObjectiveReward(steamId, ObjectiveType.RoundWin);
            }
        }
        
        public void OnTimeUpdate(string steamId, float minutesElapsed)
        {
            var progress = GetOrCreateProgress(steamId);
            progress.TimeElapsed = minutesElapsed;
            
            // Mini-Upgrade alle X Minuten
            int intervals = (int)(minutesElapsed / TimeUpgradeInterval);
            
            if (intervals > progress.LastUpgradeInterval)
            {
                AwardMiniUpgrade(steamId, $"time_{intervals}");
                progress.LastUpgradeInterval = intervals;
            }
        }
        
        public void OnKill(string steamId)
        {
            var progress = GetOrCreateProgress(steamId);
            progress.CurrentKillStreak++;
            progress.MatchKills++;
            
            if (progress.CurrentKillStreak > progress.HighestKillStreak)
            {
                progress.HighestKillStreak = progress.CurrentKillStreak;
            }
            
            // Kill Streak Rewards
            if (progress.CurrentKillStreak >= 3 && progress.CurrentKillStreak > progress.LastKillStreakReward)
            {
                AwardKillStreakReward(steamId, progress.CurrentKillStreak);
                progress.LastKillStreakReward = progress.CurrentKillStreak;
            }
        }
        
        public void OnDeath(string steamId)
        {
            var progress = GetOrCreateProgress(steamId);
            progress.CurrentKillStreak = 0;
            progress.MatchDeaths++;
        }
        
        public void OnObjective(string steamId, ObjectiveType type)
        {
            var progress = GetOrCreateProgress(steamId);
            progress.ObjectivesCompleted++;
            
            AwardObjectiveReward(steamId, type);
        }
        
        public MatchProgress? GetMatchProgress(string steamId)
        {
            return _matchProgress.GetValueOrDefault(steamId);
        }
        
        public Dictionary<string, float> GetActiveUpgrades(string steamId)
        {
            var progress = GetMatchProgress(steamId);
            return progress?.ActiveUpgrades ?? new Dictionary<string, float>();
        }
        
        public void ResetMatchProgress(string steamId)
        {
            _matchProgress.Remove(steamId);
        }
        
        private MatchProgress GetOrCreateProgress(string steamId)
        {
            if (!_matchProgress.TryGetValue(steamId, out var progress))
            {
                progress = new MatchProgress { SteamId = steamId };
                _matchProgress[steamId] = progress;
            }
            return progress;
        }
        
        private void AwardMiniUpgrade(string steamId, string source)
        {
            var progress = GetOrCreateProgress(steamId);
            var player = _playerService.GetPlayer(steamId);
            
            if (player == null) return;
            
            // Zufälliges Mini-Upgrade
            var upgrades = new[]
            {
                ("damage_bonus", 0.05f),      // +5% Damage
                ("movement_speed", 0.10f),    // +10% Movement Speed
                ("cooldown_reduction", 0.10f), // -10% Cooldown
                ("health_bonus", 20f),        // +20 HP
                ("armor_bonus", 10f)          // +10 Armor
            };
            
            var random = new System.Random();
            var upgrade = upgrades[random.Next(upgrades.Length)];
            
            // Stack upgrades (additiv)
            if (progress.ActiveUpgrades.ContainsKey(upgrade.Item1))
            {
                progress.ActiveUpgrades[upgrade.Item1] += upgrade.Item2;
            }
            else
            {
                progress.ActiveUpgrades[upgrade.Item1] = upgrade.Item2;
            }
            
            // Notify player
            var playerController = player.PlayerController;
            if (playerController != null && playerController.IsValid)
            {
                var upgradeName = upgrade.Item1.Replace("_", " ").ToUpper();
                playerController.PrintToChat($" {ChatColors.Gold}[Match Evolution]{ChatColors.Default} Mini-Upgrade: {upgradeName} +{upgrade.Item2 * 100}%!");
            }
        }
        
        private void AwardKillStreakReward(string steamId, int streak)
        {
            var progress = GetOrCreateProgress(steamId);
            var player = _playerService.GetPlayer(steamId);
            
            if (player == null) return;
            
            // Streak-basierte Rewards
            float bonus = streak switch
            {
                3 => 0.05f,   // 3 Kills: +5% Damage
                5 => 0.10f,   // 5 Kills: +10% Damage
                7 => 0.15f,   // 7 Kills: +15% Damage
                10 => 0.20f,  // 10 Kills: +20% Damage
                _ => 0f
            };
            
            if (bonus > 0)
            {
                if (progress.ActiveUpgrades.ContainsKey("damage_bonus"))
                {
                    progress.ActiveUpgrades["damage_bonus"] += bonus;
                }
                else
                {
                    progress.ActiveUpgrades["damage_bonus"] = bonus;
                }
                
                var playerController = player.PlayerController;
                if (playerController != null && playerController.IsValid)
                {
                    playerController.PrintToChat($" {ChatColors.Red}[Kill Streak]{ChatColors.Default} {streak} Kills! +{bonus * 100}% Damage Bonus!");
                }
            }
        }
        
        private void AwardObjectiveReward(string steamId, ObjectiveType type)
        {
            var progress = GetOrCreateProgress(steamId);
            var player = _playerService.GetPlayer(steamId);
            
            if (player == null) return;
            
            // Objective-basierte Rewards
            var reward = type switch
            {
                ObjectiveType.BombPlant => ("cooldown_reduction", 0.15f),
                ObjectiveType.BombDefuse => ("health_bonus", 30f),
                ObjectiveType.HostageRescue => ("movement_speed", 0.15f),
                ObjectiveType.RoundWin => ("damage_bonus", 0.10f),
                ObjectiveType.Clutch => ("damage_bonus", 0.20f),
                _ => (null, 0f)
            };
            
            if (reward.Item1 != null)
            {
                if (progress.ActiveUpgrades.ContainsKey(reward.Item1))
                {
                    progress.ActiveUpgrades[reward.Item1] += reward.Item2;
                }
                else
                {
                    progress.ActiveUpgrades[reward.Item1] = reward.Item2;
                }
                
                var playerController = player.PlayerController;
                if (playerController != null && playerController.IsValid)
                {
                    var typeName = type.ToString().Replace("_", " ").ToUpper();
                    playerController.PrintToChat($" {ChatColors.Green}[Objective]{ChatColors.Default} {typeName} Reward!");
                }
            }
        }
    }
}
