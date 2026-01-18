using System.Collections.Generic;
using UltimateHeroes.Domain.Heroes;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für Hero Management
    /// </summary>
    public interface IHeroService
    {
        void RegisterHero(IHero hero);
        void RegisterHeroes(IEnumerable<IHero> heroes);
        
        IHero? GetHero(string heroId);
        List<IHero> GetAllHeroes();
        bool HeroExists(string heroId);
        
        void SetPlayerHero(string steamId, string heroId);
        IHero? GetPlayerHero(string steamId);
    }
}
