# ⚡ Plan: SkillService

## 📋 Zweck

Der SkillService verwaltet alle Skills:
- Skill Registration
- Skill Lookup
- Skill Activation
- Skill Cooldown Management

## 🔗 Abhängigkeiten

- `ISkill` (Domain/Skills/ISkill.cs) ✅
- `SkillBase` (Domain/Skills/SkillBase.cs) - später
- Konkrete Skills (Fireball, Blink, etc.) - später
- `CooldownManager` (Infrastructure) - später

## 📐 Service Interface

```csharp
namespace UltimateHeroes.Application.Services
{
    public interface ISkillService
    {
        // Registration
        void RegisterSkill(ISkill skill);
        void RegisterSkills(IEnumerable<ISkill> skills);
        
        // Lookup
        ISkill? GetSkill(string skillId);
        List<ISkill> GetAllSkills();
        List<ISkill> GetSkillsByType(SkillType type);
        List<ISkill> GetSkillsByTag(SkillTag tag);
        bool SkillExists(string skillId);
        
        // Activation
        bool CanActivateSkill(string steamId, string skillId);
        void ActivateSkill(string steamId, string skillId, CCSPlayerController player);
        
        // Cooldown
        float GetSkillCooldown(string steamId, string skillId);
        bool IsSkillReady(string steamId, string skillId);
    }
}
```

## 🎯 Implementierung

### Datei: `Application/Services/SkillService.cs`

```csharp
namespace UltimateHeroes.Application.Services
{
    public class SkillService : ISkillService
    {
        private readonly Dictionary<string, ISkill> _skills = new();
        private readonly ICooldownManager _cooldownManager;
        private readonly IPlayerService _playerService;
        
        public SkillService(ICooldownManager cooldownManager, IPlayerService playerService)
        {
            _cooldownManager = cooldownManager;
            _playerService = playerService;
        }
        
        public void RegisterSkill(ISkill skill)
        {
            if (_skills.ContainsKey(skill.Id))
            {
                throw new InvalidOperationException($"Skill {skill.Id} already registered");
            }
            
            _skills[skill.Id] = skill;
        }
        
        public void RegisterSkills(IEnumerable<ISkill> skills)
        {
            foreach (var skill in skills)
            {
                RegisterSkill(skill);
            }
        }
        
        public ISkill? GetSkill(string skillId)
        {
            return _skills.GetValueOrDefault(skillId);
        }
        
        public List<ISkill> GetAllSkills()
        {
            return _skills.Values.ToList();
        }
        
        public List<ISkill> GetSkillsByType(SkillType type)
        {
            return _skills.Values.Where(s => s.Type == type).ToList();
        }
        
        public List<ISkill> GetSkillsByTag(SkillTag tag)
        {
            return _skills.Values.Where(s => s.Tags.Contains(tag)).ToList();
        }
        
        public bool SkillExists(string skillId)
        {
            return _skills.ContainsKey(skillId);
        }
        
        public bool CanActivateSkill(string steamId, string skillId)
        {
            var skill = GetSkill(skillId);
            if (skill == null) return false;
            
            if (skill.Type == SkillType.Passive) return false; // Passive können nicht aktiviert werden
            
            return IsSkillReady(steamId, skillId);
        }
        
        public void ActivateSkill(string steamId, string skillId, CCSPlayerController player)
        {
            var skill = GetSkill(skillId);
            if (skill == null) return;
            
            if (!CanActivateSkill(steamId, skillId)) return;
            
            // Activate based on type
            if (skill is IActiveSkill activeSkill)
            {
                activeSkill.Activate(player);
                
                // Set Cooldown
                var playerState = _playerService.GetPlayer(steamId);
                if (playerState != null)
                {
                    var cooldown = activeSkill.Cooldown;
                    var hero = playerState.CurrentHero;
                    if (hero != null)
                    {
                        // Apply Hero Identity Cooldown Reduction
                        var reduction = hero.Identity.GetCooldownReduction(skill);
                        cooldown *= (1f - reduction);
                    }
                    
                    _cooldownManager.SetCooldown(steamId, skillId, cooldown);
                }
            }
        }
        
        public float GetSkillCooldown(string steamId, string skillId)
        {
            return _cooldownManager.GetCooldown(steamId, skillId);
        }
        
        public bool IsSkillReady(string steamId, string skillId)
        {
            return _cooldownManager.IsReady(steamId, skillId);
        }
    }
}
```

## 🔄 Integration

1. **Plugin Load**: Registriert alle Skills
2. **BuildService**: Validiert Skills in Builds
3. **Player State**: Aktiviert Skills für Player
4. **CooldownManager**: Trackt Cooldowns
5. **Menu System**: Zeigt verfügbare Skills

## ✅ Tests

- Skill Registration funktioniert
- Skill Lookup funktioniert
- Skill Activation funktioniert
- Cooldowns werden korrekt gesetzt
- Hero Identity Cooldown Reduction wird angewendet

## 📝 Nächste Schritte

1. ✅ ISkillService Interface definieren
2. ✅ SkillService.cs implementieren
3. ✅ Skill Registration im Plugin
4. ✅ Integration mit CooldownManager
5. ✅ Integration mit PlayerService
