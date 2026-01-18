# 🏗️ Plan: BuildService

## 📋 Zweck

Der BuildService verwaltet Builds für Spieler:
- Build Creation
- Build Saving/Loading
- Build Activation
- Build Validation

## 🔗 Abhängigkeiten

- `Build` (Domain/Builds/Build.cs) - später
- `BuildValidator` (Application/BuildValidator.cs) ✅
- `IHeroService` (Application/Services/IHeroService.cs) - später
- `ISkillService` (Application/Services/ISkillService.cs) - später
- `IBuildRepository` (Infrastructure/Database) - später
- `IPlayerService` (Application/Services/IPlayerService.cs) - später

## 📐 Service Interface

```csharp
namespace UltimateHeroes.Application.Services
{
    public interface IBuildService
    {
        // Build Management
        Build CreateBuild(string steamId, int buildSlot, string heroId, List<string> skillIds, string buildName);
        Build? GetBuild(string steamId, int buildSlot);
        List<Build> GetPlayerBuilds(string steamId);
        void SaveBuild(Build build);
        void DeleteBuild(string steamId, int buildSlot);
        
        // Build Activation
        void ActivateBuild(string steamId, int buildSlot, CCSPlayerController player);
        Build? GetActiveBuild(string steamId);
        
        // Build Validation
        ValidationResult ValidateBuild(string heroId, List<string> skillIds);
        
        // Build Slots
        List<int> GetUnlockedSlots(string steamId);
        bool IsSlotUnlocked(string steamId, int slot);
    }
}
```

## 🎯 Implementierung

### Datei: `Application/Services/BuildService.cs`

```csharp
namespace UltimateHeroes.Application.Services
{
    public class BuildService : IBuildService
    {
        private readonly IBuildRepository _buildRepository;
        private readonly IHeroService _heroService;
        private readonly ISkillService _skillService;
        private readonly BuildValidator _buildValidator;
        private readonly IPlayerService _playerService;
        
        public BuildService(
            IBuildRepository buildRepository,
            IHeroService heroService,
            ISkillService skillService,
            BuildValidator buildValidator,
            IPlayerService playerService)
        {
            _buildRepository = buildRepository;
            _heroService = heroService;
            _skillService = skillService;
            _buildValidator = buildValidator;
            _playerService = playerService;
        }
        
        public Build CreateBuild(string steamId, int buildSlot, string heroId, List<string> skillIds, string buildName)
        {
            // Validate Hero
            var hero = _heroService.GetHero(heroId);
            if (hero == null)
            {
                throw new ArgumentException($"Hero {heroId} not found");
            }
            
            // Validate Skills
            var skills = new List<ISkill>();
            foreach (var skillId in skillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null)
                {
                    throw new ArgumentException($"Skill {skillId} not found");
                }
                skills.Add(skill);
            }
            
            // Validate Build
            var validation = _buildValidator.ValidateBuild(hero, skills);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException($"Build validation failed: {string.Join(", ", validation.Errors)}");
            }
            
            // Create Build
            var build = new Build
            {
                SteamId = steamId,
                BuildSlot = buildSlot,
                HeroCoreId = heroId,
                SkillIds = skillIds,
                BuildName = buildName,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                IsValid = true
            };
            
            // Save to Database
            _buildRepository.SaveBuild(build);
            
            return build;
        }
        
        public Build? GetBuild(string steamId, int buildSlot)
        {
            return _buildRepository.GetBuild(steamId, buildSlot);
        }
        
        public List<Build> GetPlayerBuilds(string steamId)
        {
            return _buildRepository.GetPlayerBuilds(steamId);
        }
        
        public void SaveBuild(Build build)
        {
            // Re-validate before saving
            var hero = _heroService.GetHero(build.HeroCoreId);
            var skills = build.SkillIds.Select(id => _skillService.GetSkill(id)).Where(s => s != null).Cast<ISkill>().ToList();
            
            if (hero != null)
            {
                var validation = _buildValidator.ValidateBuild(hero, skills);
                build.IsValid = validation.IsValid;
                build.ValidationErrors = validation.Errors;
            }
            
            build.LastUsedAt = DateTime.UtcNow;
            _buildRepository.SaveBuild(build);
        }
        
        public void DeleteBuild(string steamId, int buildSlot)
        {
            _buildRepository.DeleteBuild(steamId, buildSlot);
        }
        
        public void ActivateBuild(string steamId, int buildSlot, CCSPlayerController player)
        {
            var build = GetBuild(steamId, buildSlot);
            if (build == null)
            {
                throw new ArgumentException($"Build {buildSlot} not found");
            }
            
            if (!build.IsValid)
            {
                throw new InvalidOperationException($"Build {buildSlot} is not valid");
            }
            
            // Deactivate other builds
            var playerBuilds = GetPlayerBuilds(steamId);
            foreach (var b in playerBuilds)
            {
                if (b.IsActive && b.BuildSlot != buildSlot)
                {
                    b.IsActive = false;
                    _buildRepository.SaveBuild(b);
                }
            }
            
            // Activate this build
            build.IsActive = true;
            build.LastUsedAt = DateTime.UtcNow;
            _buildRepository.SaveBuild(build);
            
            // Activate for player
            var hero = _heroService.GetHero(build.HeroCoreId);
            var skills = build.SkillIds.Select(id => _skillService.GetSkill(id)).Where(s => s != null).Cast<ISkill>().ToList();
            
            if (hero != null)
            {
                var playerState = _playerService.GetPlayer(steamId);
                if (playerState != null)
                {
                    playerState.ActivateBuild(build, hero, skills);
                }
            }
        }
        
        public Build? GetActiveBuild(string steamId)
        {
            return _buildRepository.GetActiveBuild(steamId);
        }
        
        public ValidationResult ValidateBuild(string heroId, List<string> skillIds)
        {
            var hero = _heroService.GetHero(heroId);
            if (hero == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { $"Hero {heroId} not found" }
                };
            }
            
            var skills = new List<ISkill>();
            foreach (var skillId in skillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Errors = new List<string> { $"Skill {skillId} not found" }
                    };
                }
                skills.Add(skill);
            }
            
            return _buildValidator.ValidateBuild(hero, skills);
        }
        
        public List<int> GetUnlockedSlots(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return new List<int> { 1, 2, 3 }; // Default: 3 Slots
            
            var level = player.HeroLevel;
            var slots = new List<int>();
            
            foreach (var slot in BuildSlot.UnlockRequirements)
            {
                if (level >= slot.Value)
                {
                    slots.Add(slot.Key);
                }
            }
            
            return slots;
        }
        
        public bool IsSlotUnlocked(string steamId, int slot)
        {
            return GetUnlockedSlots(steamId).Contains(slot);
        }
    }
}
```

## 🔄 Integration

1. **BuildRepository**: Persistiert Builds
2. **HeroService**: Validiert Heroes
3. **SkillService**: Validiert Skills
4. **BuildValidator**: Validiert Builds
5. **PlayerService**: Aktiviert Builds für Player
6. **Menu System**: Build Editor/Selection

## ✅ Tests

- Build Creation funktioniert
- Build Validation funktioniert
- Build Activation funktioniert
- Build Slots werden korrekt freigeschaltet
- Build Deletion funktioniert

## 📝 Nächste Schritte

1. ✅ IBuildService Interface definieren
2. ✅ BuildService.cs implementieren
3. ✅ Integration mit BuildRepository
4. ✅ Integration mit HeroService/SkillService
5. ✅ Integration mit PlayerService
