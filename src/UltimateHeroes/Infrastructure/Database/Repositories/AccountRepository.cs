using Dapper;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Infrastructure.Database;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository Implementation für Account Level Data Access
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly Database _database;
        
        public AccountRepository(Database database)
        {
            _database = database;
        }
        
        public AccountLevel? GetAccountLevel(string steamId)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            var account = connection.QueryFirstOrDefault<AccountLevel>(
                "SELECT * FROM account_levels WHERE steamid = @SteamId",
                new { SteamId = steamId }
            );
            
            if (account != null)
            {
                // Load unlocks (stored as JSON or comma-separated)
                var titlesJson = connection.QueryFirstOrDefault<string>(
                    "SELECT unlocked_titles FROM account_levels WHERE steamid = @SteamId",
                    new { SteamId = steamId }
                );
                
                if (!string.IsNullOrEmpty(titlesJson))
                {
                    // Simple parsing (assuming comma-separated for now)
                    account.UnlockedTitles = titlesJson.Split(',').Where(t => !string.IsNullOrEmpty(t)).ToList();
                }
                
                var cosmeticsJson = connection.QueryFirstOrDefault<string>(
                    "SELECT unlocked_cosmetics FROM account_levels WHERE steamid = @SteamId",
                    new { SteamId = steamId }
                );
                
                if (!string.IsNullOrEmpty(cosmeticsJson))
                {
                    account.UnlockedCosmetics = cosmeticsJson.Split(',').Where(c => !string.IsNullOrEmpty(c)).ToList();
                }
            }
            
            return account;
        }
        
        public void SaveAccountLevel(AccountLevel accountLevel)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            connection.Execute(@"
                INSERT OR REPLACE INTO account_levels 
                (steamid, account_level, account_xp, xp_to_next_level, unlocked_titles, unlocked_cosmetics, last_updated)
                VALUES 
                (@SteamId, @AccountLevel, @AccountXp, @XpToNextLevel, @UnlockedTitles, @UnlockedCosmetics, CURRENT_TIMESTAMP)",
                new
                {
                    accountLevel.SteamId,
                    AccountLevel = accountLevel.Level,
                    AccountXp = accountLevel.AccountXp,
                    XpToNextLevel = accountLevel.XpToNextLevel,
                    UnlockedTitles = string.Join(",", accountLevel.UnlockedTitles),
                    UnlockedCosmetics = string.Join(",", accountLevel.UnlockedCosmetics)
                }
            );
        }
    }
}
