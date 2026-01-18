using UltimateHeroes.Domain.Players;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository Interface für Player Data Access
    /// </summary>
    public interface IPlayerRepository
    {
        UltimatePlayer? GetPlayer(string steamId);
        void SavePlayer(UltimatePlayer player);
        void DeletePlayer(string steamId);
        string? GetHeroCoreId(string steamId);
    }
}
