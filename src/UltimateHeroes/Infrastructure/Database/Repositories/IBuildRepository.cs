using System.Collections.Generic;
using UltimateHeroes.Domain.Builds;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository Interface für Build Data Access
    /// </summary>
    public interface IBuildRepository
    {
        Build? GetBuild(string steamId, int buildSlot);
        List<Build> GetPlayerBuilds(string steamId);
        Build? GetActiveBuild(string steamId);
        void SaveBuild(Build build);
        void DeleteBuild(string steamId, int buildSlot);
    }
}
