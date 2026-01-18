using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Domain.Builds;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Bot Management und Balancing
    /// </summary>
    public class BotService : IBotService
    {
        private readonly IPlayerService _playerService;
        private readonly IHeroService _heroService;
        private readonly ISkillService _skillService;
        private readonly IBuildService _buildService;
        private readonly IXpService _xpService;
        
        private readonly Dictionary<string, BotConfiguration> _botConfigs = new();
        private readonly Dictionary<string, BotStats> _botStats = new();
        private readonly Dictionary<string, DateTime> _lastBuildChange = new();
        
        // Predefined Build Presets
        private readonly Dictionary<string, BuildPreset> _buildPresets = new();
        
        public BotService(
            IPlayerService playerService,
            IHeroService heroService,
            ISkillService skillService,
            IBuildService buildService,
            IXpService xpService)
        {
            _playerService = playerService;
            _heroService = heroService;
            _skillService = skillService;
            _buildService = buildService;
            _xpService = xpService;
            
            InitializeBuildPresets();
        }
        
        public bool IsBot(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return false;
            return player.IsBot || player.AuthorizedSteamID == null || player.AuthorizedSteamID.SteamId64 == 0;
        }
        
        public bool IsBot(string steamId)
        {
            // Bots haben oft SteamID 0 oder spezielle IDs
            return steamId == "0" || steamId == "BOT" || _botConfigs.ContainsKey(steamId);
        }
        
        public void ConfigureBot(string steamId, BotConfiguration config)
        {
            config.SteamId = steamId;
            _botConfigs[steamId] = config;
            
            // Initialize stats if tracking enabled
            if (config.TrackStats && !_botStats.ContainsKey(steamId))
            {
                _botStats[steamId] = new BotStats { SteamId = steamId };
            }
            
            // Apply configuration
            ApplyBotConfiguration(steamId, config);
        }
        
        public BotConfiguration? GetBotConfiguration(string steamId)
        {
            return _botConfigs.GetValueOrDefault(steamId);
        }
        
        public void AssignRandomBuild(string steamId)
        {
            var config = GetBotConfiguration(steamId);
            if (config == null) return;
            
            // Get player controller for ActivateBuild
            var player = Utilities.GetPlayers()
                .FirstOrDefault(p => p.IsValid && p.AuthorizedSteamID?.SteamId64.ToString() == steamId);
            if (player == null) return;
            
            // Get random hero
            var heroes = _heroService.GetAllHeroes();
            if (heroes.Count == 0) return;
            
            var randomHero = heroes[new Random().Next(heroes.Count)];
            
            // Get random skills (separate by type)
            var activeSkills = _skillService.GetAllSkills()
                .Where(s => s.Type == SkillType.Active)
                .ToList();
            var ultimateSkills = _skillService.GetAllSkills()
                .Where(s => s.Type == SkillType.Ultimate)
                .ToList();
            var passiveSkills = _skillService.GetAllSkills()
                .Where(s => s.Type == SkillType.Passive)
                .ToList();
            
            // Random Active Skills (max 3)
            var randomActiveSkills = activeSkills
                .OrderBy(x => Guid.NewGuid())
                .Take(Math.Min(3, activeSkills.Count))
                .Select(s => s.Id)
                .ToList();
            
            // Random Ultimate Skill (optional, 50% chance)
            string? randomUltimateSkill = null;
            if (ultimateSkills.Count > 0 && new Random().Next(2) == 0)
            {
                randomUltimateSkill = ultimateSkills[new Random().Next(ultimateSkills.Count)].Id;
            }
            
            // Random Passive Skills (max 2)
            var randomPassiveSkills = passiveSkills
                .OrderBy(x => Guid.NewGuid())
                .Take(Math.Min(2, passiveSkills.Count))
                .Select(s => s.Id)
                .ToList();
            
            // Create build
            try
            {
                var build = _buildService.CreateBuild(
                    steamId,
                    1,
                    randomHero.Id,
                    randomActiveSkills,
                    randomUltimateSkill,
                    randomPassiveSkills,
                    $"Bot Build {DateTime.Now:HHmmss}"
                );
                
                if (build != null && build.IsValid)
                {
                    _buildService.ActivateBuild(steamId, 1, player);
                }
            }
            catch
            {
                // Build creation failed, try again next time
            }
        }
        
        public void AssignPredefinedBuild(string steamId, string buildPreset)
        {
            if (!_buildPresets.TryGetValue(buildPreset, out var preset))
            {
                // Fallback to random
                AssignRandomBuild(steamId);
                return;
            }
            
            // Get player controller for ActivateBuild
            var player = Utilities.GetPlayers()
                .FirstOrDefault(p => p.IsValid && p.AuthorizedSteamID?.SteamId64.ToString() == steamId);
            if (player == null) return;
            
            try
            {
                var build = _buildService.CreateBuild(
                    steamId,
                    1,
                    preset.HeroId,
                    preset.ActiveSkillIds,
                    preset.UltimateSkillId,
                    preset.PassiveSkillIds,
                    $"Bot Preset: {buildPreset}"
                );
                
                if (build != null && build.IsValid)
                {
                    _buildService.ActivateBuild(steamId, 1, player);
                }
            }
            catch
            {
                AssignRandomBuild(steamId);
            }
        }
        
        public void AssignBuildFromPool(string steamId, List<string> buildPool)
        {
            if (buildPool.Count == 0)
            {
                AssignRandomBuild(steamId);
                return;
            }
            
            var randomPreset = buildPool[new Random().Next(buildPool.Count)];
            AssignPredefinedBuild(steamId, randomPreset);
        }
        
        public BotStats GetBotStats(string steamId)
        {
            return _botStats.GetValueOrDefault(steamId) ?? new BotStats { SteamId = steamId };
        }
        
        public List<BotStats> GetAllBotStats()
        {
            return _botStats.Values.ToList();
        }
        
        public void ResetBotStats(string steamId)
        {
            _botStats.Remove(steamId);
            _botStats[steamId] = new BotStats { SteamId = steamId };
        }
        
        /// <summary>
        /// Wird aufgerufen wenn ein Bot einen Kill macht
        /// </summary>
        public void OnBotKill(string steamId, string skillId = "", float damage = 0f)
        {
            if (!_botStats.TryGetValue(steamId, out var stats)) return;
            
            stats.TotalKills++;
            stats.TotalDamage += damage;
            
            // Track skill performance
            if (!string.IsNullOrEmpty(skillId))
            {
                if (!stats.SkillPerformances.ContainsKey(skillId))
                {
                    stats.SkillPerformances[skillId] = new SkillPerformance { SkillId = skillId };
                }
                stats.SkillPerformances[skillId].Kills++;
                stats.SkillPerformances[skillId].Damage += damage;
            }
            
            // Track build performance
            var player = _playerService.GetPlayer(steamId);
            if (player?.CurrentBuild != null)
            {
                var buildId = player.CurrentBuild.BuildSlot.ToString();
                if (!stats.BuildPerformances.ContainsKey(buildId))
                {
                    stats.BuildPerformances[buildId] = new BuildPerformance { BuildId = buildId };
                }
                stats.BuildPerformances[buildId].Kills++;
                stats.BuildPerformances[buildId].Damage += damage;
            }
            
            // Track hero performance
            if (player?.CurrentHero != null)
            {
                var heroId = player.CurrentHero.Id;
                if (!stats.HeroPerformances.ContainsKey(heroId))
                {
                    stats.HeroPerformances[heroId] = new HeroPerformance { HeroId = heroId };
                }
                stats.HeroPerformances[heroId].Kills++;
                stats.HeroPerformances[heroId].Damage += damage;
            }
        }
        
        /// <summary>
        /// Wird aufgerufen wenn ein Bot stirbt
        /// </summary>
        public void OnBotDeath(string steamId)
        {
            if (!_botStats.TryGetValue(steamId, out var stats)) return;
            
            stats.TotalDeaths++;
            
            // Track build performance
            var player = _playerService.GetPlayer(steamId);
            if (player?.CurrentBuild != null)
            {
                var buildId = player.CurrentBuild.BuildSlot.ToString();
                if (!stats.BuildPerformances.ContainsKey(buildId))
                {
                    stats.BuildPerformances[buildId] = new BuildPerformance { BuildId = buildId };
                }
                stats.BuildPerformances[buildId].Deaths++;
            }
            
            // Track hero performance
            if (player?.CurrentHero != null)
            {
                var heroId = player.CurrentHero.Id;
                if (!stats.HeroPerformances.ContainsKey(heroId))
                {
                    stats.HeroPerformances[heroId] = new HeroPerformance { HeroId = heroId };
                }
                stats.HeroPerformances[heroId].Deaths++;
            }
        }
        
        /// <summary>
        /// Prüft ob Bot Build wechseln soll
        /// </summary>
        public void CheckBuildChange(string steamId)
        {
            var config = GetBotConfiguration(steamId);
            if (config == null || !config.AutoAssignBuild) return;
            
            if (!_lastBuildChange.TryGetValue(steamId, out var lastChange))
            {
                _lastBuildChange[steamId] = DateTime.UtcNow;
                return;
            }
            
            var timeSinceChange = (DateTime.UtcNow - lastChange).TotalSeconds;
            if (timeSinceChange < config.BuildChangeInterval) return;
            
            // Change build based on mode
            switch (config.BuildMode)
            {
                case BotBuildMode.Random:
                    AssignRandomBuild(steamId);
                    break;
                case BotBuildMode.Predefined:
                    if (config.PredefinedBuilds.Count > 0)
                    {
                        var randomPreset = config.PredefinedBuilds[new Random().Next(config.PredefinedBuilds.Count)];
                        AssignPredefinedBuild(steamId, randomPreset);
                    }
                    break;
                case BotBuildMode.Pool:
                    if (config.BuildPool.Count > 0)
                    {
                        AssignBuildFromPool(steamId, config.BuildPool);
                    }
                    break;
            }
            
            _lastBuildChange[steamId] = DateTime.UtcNow;
        }
        
        private void ApplyBotConfiguration(string steamId, BotConfiguration config)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return;
            
            // Set level based on mode
            int targetLevel = config.LevelMode switch
            {
                BotLevelMode.Level0 => 1,
                BotLevelMode.MaxLevel => LevelSystem.MaxHeroLevel,
                BotLevelMode.Random => new Random().Next(1, LevelSystem.MaxHeroLevel + 1),
                BotLevelMode.Fixed => config.FixedLevel,
                BotLevelMode.MatchPlayerAverage => GetAveragePlayerLevel(),
                _ => 1
            };
            
            // Set XP to match level
            float xpNeeded = 0f;
            for (int i = 1; i < targetLevel; i++)
            {
                xpNeeded += XpSystem.GetXpForLevel(i);
            }
            
            player.HeroLevel = targetLevel;
            player.CurrentXp = xpNeeded;
            
            // Assign build if auto-assign enabled
            if (config.AutoAssignBuild)
            {
                switch (config.BuildMode)
                {
                    case BotBuildMode.Random:
                        AssignRandomBuild(steamId);
                        break;
                    case BotBuildMode.Predefined:
                        if (config.PredefinedBuilds.Count > 0)
                        {
                            var randomPreset = config.PredefinedBuilds[new Random().Next(config.PredefinedBuilds.Count)];
                            AssignPredefinedBuild(steamId, randomPreset);
                        }
                        break;
                    case BotBuildMode.Pool:
                        if (config.BuildPool.Count > 0)
                        {
                            AssignBuildFromPool(steamId, config.BuildPool);
                        }
                        break;
                }
            }
        }
        
        private int GetAveragePlayerLevel()
        {
            var players = Utilities.GetPlayers()
                .Where(p => p != null && p.IsValid && !IsBot(p) && p.AuthorizedSteamID != null)
                .ToList();
            
            if (players.Count == 0) return 10; // Default
            
            int totalLevel = 0;
            int count = 0;
            
            foreach (var player in players)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var playerState = _playerService.GetPlayer(steamId);
                if (playerState != null)
                {
                    totalLevel += playerState.HeroLevel;
                    count++;
                }
            }
            
            return count > 0 ? totalLevel / count : 10;
        }
        
        private void InitializeBuildPresets()
        {
            // DPS Build
            _buildPresets["dps"] = new BuildPreset
            {
                HeroId = "vanguard",
                ActiveSkillIds = new List<string> { "fireball" },
                UltimateSkillId = null,
                PassiveSkillIds = new List<string>()
            };
            
            // Mobility Build
            _buildPresets["mobility"] = new BuildPreset
            {
                HeroId = "phantom",
                ActiveSkillIds = new List<string> { "blink" },
                UltimateSkillId = "teleport",
                PassiveSkillIds = new List<string>()
            };
            
            // Stealth Build
            _buildPresets["stealth"] = new BuildPreset
            {
                HeroId = "phantom",
                ActiveSkillIds = new List<string> { "stealth" },
                UltimateSkillId = null,
                PassiveSkillIds = new List<string> { "silentfootsteps" }
            };
            
            // Support Build
            _buildPresets["support"] = new BuildPreset
            {
                HeroId = "engineer",
                ActiveSkillIds = new List<string>(),
                UltimateSkillId = null,
                PassiveSkillIds = new List<string> { "healingaura" }
            };
            
            // Balanced Build
            _buildPresets["balanced"] = new BuildPreset
            {
                HeroId = "vanguard",
                ActiveSkillIds = new List<string> { "fireball", "blink" },
                UltimateSkillId = null,
                PassiveSkillIds = new List<string>()
            };
        }
        
        private class BuildPreset
        {
            public string HeroId { get; set; } = string.Empty;
            public List<string> ActiveSkillIds { get; set; } = new();
            public string? UltimateSkillId { get; set; } = null;
            public List<string> PassiveSkillIds { get; set; } = new();
        }
    }
}
