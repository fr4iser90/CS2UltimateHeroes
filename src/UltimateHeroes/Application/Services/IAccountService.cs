using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für Account Level Management
    /// </summary>
    public interface IAccountService
    {
        // Account Level
        AccountLevel GetAccountLevel(string steamId);
        int GetAccountLevelValue(string steamId);
        float GetAccountXp(string steamId);
        float GetAccountXpProgress(string steamId);
        
        // Account XP
        void AwardAccountXp(string steamId, AccountXpSource source, float amount);
        void AwardAccountXp(string steamId, AccountXpSource source);
        
        // Account Unlocks
        List<string> GetUnlockedTitles(string steamId);
        List<string> GetUnlockedCosmetics(string steamId);
        bool HasUnlockedTitle(string steamId, string titleId);
        bool HasUnlockedCosmetic(string steamId, string cosmeticId);
        
        // Account Level Up
        void CheckAccountLevelUp(string steamId);
        void OnAccountLevelUp(string steamId, int newLevel, CCSPlayerController? player = null);
        
        // Prestige
        bool CanPrestige(string steamId);
    }
}
