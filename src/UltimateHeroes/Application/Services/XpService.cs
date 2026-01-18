using System;
using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;
using UltimateHeroes.Domain.Players;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Infrastructure.Configuration;
using UltimateHeroes.Infrastructure.Database.Repositories;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für XP und Level Management
    /// </summary>
    public class XpService : IXpService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerService _playerService;
        private readonly ITalentService? _talentService;
        private readonly IAccountService? _accountService;
        private PluginConfiguration? _config;
        
        public XpService(
            IPlayerRepository playerRepository,
            IPlayerService playerService,
            ITalentService? talentService = null,
            IAccountService? accountService = null)
        {
            _playerRepository = playerRepository;
            _playerService = playerService;
            _talentService = talentService;
            _accountService = accountService;
        }
        
        public void SetConfig(PluginConfiguration config)
        {
            _config = config;
        }
        
        public void AwardXp(string steamId, XpSource source, float amount)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return;
            
            // Apply Role Influence Bonus
            var bonus = GetRoleInfluenceBonus(player.CurrentRole, source);
            amount *= (1f + bonus);
            
            // Award XP
            player.CurrentXp += amount;
            
            // Save to XpHistory (if repository is available)
            // Note: XpHistoryRepository needs to be injected via constructor
            // For now, we skip this (implementation requires repository injection)
            
            // Check Level Up
            CheckLevelUp(steamId);
            
            // Save Player
            _playerService.SavePlayer(player);
        }
        
        public void AwardXp(string steamId, XpSource source)
        {
            var amount = GetXpAmount(source);
            AwardXp(steamId, source, amount);
        }
        
        private float GetXpAmount(XpSource source)
        {
            return source switch
            {
                XpSource.Kill => XpSystem.XpPerKill,
                XpSource.Headshot => XpSystem.XpPerHeadshot,
                XpSource.Assist => XpSystem.XpPerAssist,
                XpSource.Objective => XpSystem.XpPerObjective,
                XpSource.RoundWin => XpSystem.XpPerRoundWin,
                XpSource.FlashAssist => XpSystem.XpPerFlashAssist,
                XpSource.ClutchRound => 50f,
                XpSource.FirstBlood => 15f,
                XpSource.MatchCompletion => XpSystem.XpPerMatchCompletion,
                XpSource.WinBonus => 0f, // Win Bonus ist Multiplikator, nicht direkter XP
                _ => 0f
            };
        }
        
        /// <summary>
        /// Berechnet Match-XP mit Modus-Multiplikator und Win Bonus
        /// </summary>
        public float CalculateMatchXp(float baseXp, Application.Services.GameMode gameMode, bool wonMatch)
        {
            // Modus-Multiplikator
            float multiplier = gameMode switch
            {
                Application.Services.GameMode.Casual => XpSystem.CasualMultiplier,
                Application.Services.GameMode.Competitive => XpSystem.RankedMultiplier,
                Application.Services.GameMode.Deathmatch => XpSystem.DeathmatchMultiplier,
                _ => XpSystem.RankedMultiplier // Default: Ranked
            };
            
            // Win Bonus
            if (wonMatch)
            {
                multiplier *= (1f + XpSystem.WinBonusMultiplier);
            }
            
            return baseXp * multiplier;
        }
        
        private float GetRoleInfluenceBonus(RoleInfluence role, XpSource source)
        {
            // Support Role: +20% XP für Support-Aktionen
            if (role == RoleInfluence.Support && (source == XpSource.FlashAssist || source == XpSource.Assist))
            {
                return 0.2f;
            }
            
            // DPS Role: +10% XP für Kills
            if (role == RoleInfluence.DPS && source == XpSource.Kill)
            {
                return 0.1f;
            }
            
            return 0f;
        }
        
        public float GetCurrentXp(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            return player?.CurrentXp ?? 0f;
        }
        
        public int GetHeroLevel(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            return player?.HeroLevel ?? 1;
        }
        
        public float GetXpProgress(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return 0f;
            
            return XpSystem.GetXpProgress(player.CurrentXp, player.HeroLevel);
        }
        
        public void CheckLevelUp(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return;
            
            var newLevel = XpSystem.GetLevelFromXp(player.CurrentXp);
            
            if (newLevel > player.HeroLevel)
            {
                OnLevelUp(steamId, newLevel);
            }
        }
        
        public void OnLevelUp(string steamId, int newLevel)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return;
            
            var oldLevel = player.HeroLevel;
            player.HeroLevel = newLevel;
            player.XpToNextLevel = XpSystem.GetXpForLevel(newLevel);
            
            // Award Account XP für Hero Level Up
            if (_accountService != null)
            {
                var accountXp = _accountService.CalculateHeroLevelUpAccountXp(newLevel);
                if (accountXp > 0f)
                {
                    _accountService.AwardAccountXp(steamId, Domain.Progression.AccountXpSource.HeroLevelUp, accountXp);
                }
            }
            
            // Check if Max Level reached (Prestige-Vorbereitung)
            if (newLevel >= LevelSystem.MaxHeroLevel)
            {
                var playerController = player.PlayerController;
                if (playerController != null && playerController.IsValid)
                {
                    playerController.PrintToChat($" {ChatColors.Gold}[Ultimate Heroes]{ChatColors.Default} Max Level {LevelSystem.MaxHeroLevel} reached! Prestige available!");
                }
            }
            
            // Award Talent Points
            var talentPoints = newLevel - oldLevel;
            _talentService?.AwardTalentPoints(steamId, talentPoints);
            
            // Notify Player
            var playerController2 = player.PlayerController;
            if (playerController2 != null && playerController2.IsValid)
            {
                playerController2.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Level Up! You are now Level {newLevel}!");
                
                // Update player name for scoreboard
                if (_config != null)
                {
                    Server.NextFrame(() =>
                    {
                        if (playerController2 != null && playerController2.IsValid)
                        {
                            PlayerNameHelper.RefreshPlayerName(playerController2, _playerService, _accountService, _config);
                        }
                    });
                }
            }
            
            // Save Player
            _playerService.SavePlayer(player);
        }
        
        /// <summary>
        /// Award Match Completion XP
        /// </summary>
        public void AwardMatchCompletion(string steamId, Application.Services.GameMode gameMode, bool wonMatch)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return;
            
            // Base XP: Match Completion
            var baseXp = XpSystem.XpPerMatchCompletion;
            
            // Apply Modus-Multiplikator und Win Bonus
            var finalXp = CalculateMatchXp(baseXp, gameMode, wonMatch);
            
            AwardXp(steamId, XpSource.MatchCompletion, finalXp);
            
            // Award Account XP für Match Completion
            _accountService?.AwardAccountXp(steamId, Domain.Progression.AccountXpSource.MatchCompletion);
            
            // Reset Kill Tracking für neues Match
            player.KillTracking.ResetForNewMatch();
        }
        
        public List<XpHistory> GetXpHistory(string steamId, int limit = 50)
        {
            // Note: XpHistoryRepository needs to be injected via constructor
            // For now, return empty list (implementation requires repository injection)
            return new List<XpHistory>();
        }
    }
}
