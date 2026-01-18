using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository Interface für Account Level Data Access
    /// </summary>
    public interface IAccountRepository
    {
        AccountLevel? GetAccountLevel(string steamId);
        void SaveAccountLevel(AccountLevel accountLevel);
    }
}
