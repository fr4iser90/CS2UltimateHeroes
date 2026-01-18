using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Infrastructure.Database.Repositories;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Account Level Management
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly Dictionary<string, AccountLevel> _accountCache = new();
        
        // Account XP Quellen
        private const float AccountXpPerMatchCompletion = 10f;
        private const float AccountXpPerHeroLevel10 = 50f;
        private const float AccountXpPerHeroLevel20 = 100f;
        private const float AccountXpPerHeroLevel30 = 150f;
        private const float AccountXpPerHeroLevel40 = 200f;
        
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public AccountLevel GetAccountLevel(string steamId)
        {
            if (_accountCache.TryGetValue(steamId, out var account))
            {
                return account;
            }
            
            AccountLevel? dbAccount = null;
            try
            {
                dbAccount = _accountRepository.GetAccountLevel(steamId);
            }
            catch
            {
                // Table might not exist yet, create default
            }
            
            if (dbAccount == null)
            {
                dbAccount = new AccountLevel
                {
                    SteamId = steamId,
                    Level = 1,
                    AccountXp = 0f,
                    XpToNextLevel = 100f
                };
                try
                {
                    _accountRepository.SaveAccountLevel(dbAccount);
                }
                catch
                {
                    // Table might not exist, just use in-memory
                }
            }
            
            _accountCache[steamId] = dbAccount;
            return dbAccount;
        }
        
        public int GetAccountLevelValue(string steamId)
        {
            return GetAccountLevel(steamId).Level;
        }
        
        public float GetAccountXp(string steamId)
        {
            return GetAccountLevel(steamId).AccountXp;
        }
        
        public float GetAccountXpProgress(string steamId)
        {
            var account = GetAccountLevel(steamId);
            return AccountLevel.GetAccountXpProgress(account.AccountXp, account.Level);
        }
        
        public void AwardAccountXp(string steamId, AccountXpSource source, float amount)
        {
            var account = GetAccountLevel(steamId);
            account.AccountXp += amount;
            
            // Check Level Up
            CheckAccountLevelUp(steamId);
            
            // Save
            _accountRepository.SaveAccountLevel(account);
            _accountCache[steamId] = account;
        }
        
        public void AwardAccountXp(string steamId, AccountXpSource source)
        {
            var amount = GetAccountXpAmount(source);
            AwardAccountXp(steamId, source, amount);
        }
        
        private float GetAccountXpAmount(AccountXpSource source)
        {
            return source switch
            {
                AccountXpSource.MatchCompletion => AccountXpPerMatchCompletion,
                AccountXpSource.HeroLevelUp => 0f, // Wird separat berechnet basierend auf Hero-Level
                _ => 0f
            };
        }
        
        /// <summary>
        /// Berechnet Account XP für Hero Level Up
        /// </summary>
        public float CalculateHeroLevelUpAccountXp(int heroLevel)
        {
            return heroLevel switch
            {
                10 => AccountXpPerHeroLevel10,
                20 => AccountXpPerHeroLevel20,
                30 => AccountXpPerHeroLevel30,
                40 => AccountXpPerHeroLevel40,
                _ => 0f // Nur bei bestimmten Milestones
            };
        }
        
        public void CheckAccountLevelUp(string steamId)
        {
            var account = GetAccountLevel(steamId);
            var newLevel = AccountLevel.GetAccountLevelFromXp(account.AccountXp);
            
            if (newLevel > account.Level)
            {
                OnAccountLevelUp(steamId, newLevel);
            }
        }
        
        public void OnAccountLevelUp(string steamId, int newLevel, CCSPlayerController? player = null)
        {
            var account = GetAccountLevel(steamId);
            var oldLevel = account.Level;
            account.Level = newLevel;
            account.XpToNextLevel = AccountLevel.GetXpForAccountLevel(newLevel);
            
            // Check for new unlocks
            CheckAccountUnlocks(steamId, oldLevel, newLevel);
            
            // Notify Player
            if (player != null && player.IsValid)
            {
                var title = AccountLevel.GetTitleForLevel(newLevel);
                player.PrintToChat($" {ChatColors.Gold}[Account]{ChatColors.Default} Account Level Up! You are now Level {newLevel} ({title})!");
            }
            
            // Save
            _accountRepository.SaveAccountLevel(account);
            _accountCache[steamId] = account;
        }
        
        private void CheckAccountUnlocks(string steamId, int oldLevel, int newLevel)
        {
            var account = GetAccountLevel(steamId);
            
            // Check Title Unlocks
            var titlesToUnlock = new List<string>();
            if (newLevel >= 25 && oldLevel < 25) titlesToUnlock.Add("novice");
            if (newLevel >= 50 && oldLevel < 50) titlesToUnlock.Add("experienced");
            if (newLevel >= 100 && oldLevel < 100) titlesToUnlock.Add("veteran");
            if (newLevel >= 200 && oldLevel < 200) titlesToUnlock.Add("master");
            if (newLevel >= 500 && oldLevel < 500) titlesToUnlock.Add("legend");
            
            foreach (var title in titlesToUnlock)
            {
                if (!account.UnlockedTitles.Contains(title))
                {
                    account.UnlockedTitles.Add(title);
                }
            }
            
            // Check Prestige Unlock (Level 200)
            if (newLevel >= 200 && oldLevel < 200)
            {
                // Prestige wird freigeschaltet (wird in Prestige System verwendet)
            }
        }
        
        public List<string> GetUnlockedTitles(string steamId)
        {
            return GetAccountLevel(steamId).UnlockedTitles;
        }
        
        public List<string> GetUnlockedCosmetics(string steamId)
        {
            return GetAccountLevel(steamId).UnlockedCosmetics;
        }
        
        public bool HasUnlockedTitle(string steamId, string titleId)
        {
            return GetAccountLevel(steamId).UnlockedTitles.Contains(titleId);
        }
        
        public bool HasUnlockedCosmetic(string steamId, string cosmeticId)
        {
            return GetAccountLevel(steamId).UnlockedCosmetics.Contains(cosmeticId);
        }
        
        public bool CanPrestige(string steamId)
        {
            var account = GetAccountLevel(steamId);
            return AccountLevel.IsPrestigeUnlocked(account.Level);
        }
    }
}
