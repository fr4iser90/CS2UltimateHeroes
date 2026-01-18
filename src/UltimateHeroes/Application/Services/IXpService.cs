using System.Collections.Generic;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für XP Management
    /// </summary>
    public interface IXpService
    {
        void AwardXp(string steamId, XpSource source, float amount);
        void AwardXp(string steamId, XpSource source);
        float GetCurrentXp(string steamId);
        int GetHeroLevel(string steamId);
        float GetXpProgress(string steamId); // 0.0 - 1.0
        
        void CheckLevelUp(string steamId);
        void OnLevelUp(string steamId, int newLevel);
        
        /// <summary>
        /// Award Match Completion XP mit Modus-Multiplikator und Win Bonus (gemäß LEVELING.md)
        /// </summary>
        void AwardMatchCompletion(string steamId, Application.Services.GameMode gameMode, bool wonMatch);
        
        /// <summary>
        /// Berechnet Match-XP mit Modus-Multiplikator und Win Bonus
        /// </summary>
        float CalculateMatchXp(float baseXp, Application.Services.GameMode gameMode, bool wonMatch);
        
        List<XpHistory> GetXpHistory(string steamId, int limit = 50);
    }
}
