using UltimateHeroes.Domain.Talents;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository für Talent-Daten
    /// </summary>
    public interface ITalentRepository
    {
        PlayerTalents? GetPlayerTalents(string steamId);
        void SavePlayerTalents(PlayerTalents playerTalents);
    }
}
