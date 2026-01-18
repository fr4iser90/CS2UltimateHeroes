using System.Collections.Generic;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository Interface für XP History
    /// </summary>
    public interface IXpHistoryRepository
    {
        void SaveXpHistory(XpHistory history);
        List<XpHistory> GetXpHistory(string steamId, int limit = 50);
    }
}
