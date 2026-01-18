using System;
using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Heroes;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Hero Management
    /// </summary>
    public class HeroService : IHeroService
    {
        private readonly Dictionary<string, IHero> _heroes = new();
        private readonly Dictionary<string, IHero> _playerHeroes = new(); // steamid -> hero
        
        public void RegisterHero(IHero hero)
        {
            if (_heroes.ContainsKey(hero.Id))
            {
                throw new InvalidOperationException($"Hero {hero.Id} already registered");
            }
            
            _heroes[hero.Id] = hero;
        }
        
        public void RegisterHeroes(IEnumerable<IHero> heroes)
        {
            foreach (var hero in heroes)
            {
                RegisterHero(hero);
            }
        }
        
        public IHero? GetHero(string heroId)
        {
            return _heroes.GetValueOrDefault(heroId);
        }
        
        public List<IHero> GetAllHeroes()
        {
            return _heroes.Values.ToList();
        }
        
        public bool HeroExists(string heroId)
        {
            return _heroes.ContainsKey(heroId);
        }
        
        public void SetPlayerHero(string steamId, string heroId)
        {
            var hero = GetHero(heroId);
            if (hero == null)
            {
                throw new ArgumentException($"Hero {heroId} not found");
            }
            
            _playerHeroes[steamId] = hero;
        }
        
        public IHero? GetPlayerHero(string steamId)
        {
            return _playerHeroes.GetValueOrDefault(steamId);
        }
        
        /// <summary>
        /// Automatically registers all IHero implementations via Reflection
        /// </summary>
        public void RegisterHeroesViaReflection()
        {
            var heroType = typeof(IHero);
            var heroTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => heroType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            
            foreach (var type in heroTypes)
            {
                try
                {
                    var hero = (IHero)System.Activator.CreateInstance(type)!;
                    RegisterHero(hero);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[HeroService] Failed to register hero {type.Name}: {ex.Message}");
                }
            }
        }
    }
}
