# 🔧 Plan: HeroService

## 📋 Zweck

Der HeroService verwaltet alle Hero Cores:
- Hero Registration
- Hero Lookup
- Hero Selection für Player

## 🔗 Abhängigkeiten

- `IHero` (Domain/Heroes/IHero.cs) ✅
- `HeroCore` (Domain/Heroes/HeroCore.cs) - später
- Konkrete Heroes (Vanguard, Phantom, Engineer) - später

## 📐 Service Interface

```csharp
namespace UltimateHeroes.Application.Services
{
    public interface IHeroService
    {
        // Registration
        void RegisterHero(IHero hero);
        void RegisterHeroes(IEnumerable<IHero> heroes);
        
        // Lookup
        IHero? GetHero(string heroId);
        List<IHero> GetAllHeroes();
        bool HeroExists(string heroId);
        
        // Player Management
        void SetPlayerHero(string steamId, string heroId);
        IHero? GetPlayerHero(string steamId);
    }
}
```

## 🎯 Implementierung

### Datei: `Application/Services/HeroService.cs`

```csharp
namespace UltimateHeroes.Application.Services
{
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
    }
}
```

## 🔄 Initialization

Im `UltimateHeroes.cs` Plugin:

```csharp
public override void Load(bool hotReload)
{
    // Initialize HeroService
    var heroService = new HeroService();
    
    // Register Heroes
    heroService.RegisterHeroes(new List<IHero>
    {
        new Vanguard(),
        new Phantom(),
        new Engineer()
    });
    
    // Store in DI Container oder static
    Services.HeroService = heroService;
}
```

## 🔄 Integration

1. **Plugin Load**: Registriert alle Heroes
2. **BuildService**: Validiert Hero in Builds
3. **Player State**: Setzt aktiven Hero
4. **Menu System**: Zeigt verfügbare Heroes

## ✅ Tests

- Hero Registration funktioniert
- Hero Lookup funktioniert
- Player Hero wird korrekt gesetzt
- Duplicate Registration wird verhindert

## 📝 Nächste Schritte

1. ✅ IHeroService Interface definieren
2. ✅ HeroService.cs implementieren
3. ✅ Hero Registration im Plugin
4. ✅ Integration mit BuildService
