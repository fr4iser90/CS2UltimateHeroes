using System.Collections.Generic;
using System.Linq;
using Dapper;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Infrastructure.Database;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository für XP History
    /// </summary>
    public class XpHistoryRepository : IXpHistoryRepository
    {
        private readonly Database _database;
        
        public XpHistoryRepository(Database database)
        {
            _database = database;
        }
        
        public void SaveXpHistory(XpHistory history)
        {
            using var connection = _database.GetConnection();
            
            connection.Execute(
                @"INSERT INTO xp_history (steamid, xp_source, amount, timestamp) 
                  VALUES (@SteamId, @Source, @Amount, @Timestamp)",
                new 
                { 
                    SteamId = history.SteamId,
                    Source = history.Source.ToString(),
                    Amount = history.Amount,
                    Timestamp = history.Timestamp
                }
            );
        }
        
        public List<XpHistory> GetXpHistory(string steamId, int limit = 50)
        {
            using var connection = _database.GetConnection();
            
            var results = connection.Query<dynamic>(
                @"SELECT steamid, xp_source, amount, timestamp 
                  FROM xp_history 
                  WHERE steamid = @SteamId 
                  ORDER BY timestamp DESC 
                  LIMIT @Limit",
                new { SteamId = steamId, Limit = limit }
            ).ToList();
            
            return results.Select(r => new XpHistory
            {
                SteamId = r.steamid,
                Source = System.Enum.Parse<XpSource>(r.xp_source),
                Amount = (float)r.amount,
                Timestamp = System.DateTime.Parse(r.timestamp)
            }).ToList();
        }
    }
}
