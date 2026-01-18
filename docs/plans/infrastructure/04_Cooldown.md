# ⏱️ Plan: Cooldown Manager

## 📋 Zweck

Der CooldownManager verwaltet Skill Cooldowns:
- Cooldown Tracking
- Cooldown Reduction (Hero Identity)
- Cooldown UI Updates

## 🔗 Abhängigkeiten

- `ISkill` (Domain/Skills/ISkill.cs) ✅

## 📐 Cooldown Manager Struktur

### ICooldownManager Interface

```csharp
namespace UltimateHeroes.Infrastructure.Cooldown
{
    public interface ICooldownManager
    {
        void SetCooldown(string steamId, string skillId, float cooldownSeconds);
        float GetCooldown(string steamId, string skillId);
        bool IsReady(string steamId, string skillId);
        void ClearCooldown(string steamId, string skillId);
        void ClearAllCooldowns(string steamId);
    }
}
```

### CooldownManager Implementation

```csharp
namespace UltimateHeroes.Infrastructure.Cooldown
{
    public class CooldownManager : ICooldownManager
    {
        private readonly Dictionary<string, Dictionary<string, DateTime>> _cooldowns = new(); // steamid -> skillid -> endtime
        
        public void SetCooldown(string steamId, string skillId, float cooldownSeconds)
        {
            if (!_cooldowns.ContainsKey(steamId))
            {
                _cooldowns[steamId] = new Dictionary<string, DateTime>();
            }
            
            var endTime = DateTime.UtcNow.AddSeconds(cooldownSeconds);
            _cooldowns[steamId][skillId] = endTime;
        }
        
        public float GetCooldown(string steamId, string skillId)
        {
            if (!_cooldowns.ContainsKey(steamId)) return 0f;
            if (!_cooldowns[steamId].ContainsKey(skillId)) return 0f;
            
            var endTime = _cooldowns[steamId][skillId];
            var remaining = (float)(endTime - DateTime.UtcNow).TotalSeconds;
            return Math.Max(0f, remaining);
        }
        
        public bool IsReady(string steamId, string skillId)
        {
            return GetCooldown(steamId, skillId) <= 0f;
        }
        
        public void ClearCooldown(string steamId, string skillId)
        {
            if (_cooldowns.ContainsKey(steamId))
            {
                _cooldowns[steamId].Remove(skillId);
            }
        }
        
        public void ClearAllCooldowns(string steamId)
        {
            _cooldowns.Remove(steamId);
        }
    }
}
```

## 🔄 Integration

1. **SkillService**: Setzt Cooldowns nach Skill Activation
2. **UI**: Zeigt Cooldown Timers
3. **Hero Identity**: Reduziert Cooldowns

## ✅ Tests

- Cooldowns werden korrekt gesetzt
- Cooldown Reduction funktioniert
- Cooldown Expiration funktioniert

## 📝 Nächste Schritte

1. ✅ ICooldownManager Interface definieren
2. ✅ CooldownManager.cs implementieren
3. ✅ Integration mit SkillService
4. ✅ UI Integration
