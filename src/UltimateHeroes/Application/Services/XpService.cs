using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Domain.Players;
using UltimateHeroes.Domain.Progression;
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
        // private readonly ITalentService _talentService; // Später
        
        public XpService(
            IPlayerRepository playerRepository,
            IPlayerService playerService)
        {
            _playerRepository = playerRepository;
            _playerService = playerService;
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
                _ => 0f
            };
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
            
            // Award Talent Points (später mit TalentService)
            // var talentPoints = newLevel - oldLevel;
            // _talentService.AwardTalentPoints(steamId, talentPoints);
            
            // Notify Player
            var playerController = player.PlayerController;
            if (playerController != null && playerController.IsValid)
            {
                playerController.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Level Up! You are now Level {newLevel}!");
            }
            
            // Save Player
            _playerService.SavePlayer(player);
        }
        
        public List<XpHistory> GetXpHistory(string steamId, int limit = 50)
        {
            // TODO: Implement when XpHistory Repository is ready
            return new List<XpHistory>();
        }
    }
}
