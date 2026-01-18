# ✨ Plan: Effect System

## 📋 Zweck

Das Effect System verwaltet temporäre Effects auf Spielern:
- Effect Application
- Effect Removal
- Effect Stacking
- Effect Duration

## 🔗 Abhängigkeiten

- `UltimatePlayer` (Domain/Players/UltimatePlayer.cs) - später

## 📐 Effect System Struktur

### IEffect Interface

```csharp
namespace UltimateHeroes.Infrastructure.Effects
{
    public interface IEffect
    {
        string Id { get; }
        string DisplayName { get; }
        float Duration { get; }
        DateTime AppliedAt { get; set; }
        
        void OnApply(CCSPlayerController player);
        void OnTick(CCSPlayerController player);
        void OnRemove(CCSPlayerController player);
        bool IsExpired();
    }
}
```

### EffectManager

```csharp
namespace UltimateHeroes.Infrastructure.Effects
{
    public class EffectManager
    {
        private readonly Dictionary<string, List<IEffect>> _playerEffects = new(); // steamid -> effects
        
        public void ApplyEffect(string steamId, IEffect effect)
        {
            if (!_playerEffects.ContainsKey(steamId))
            {
                _playerEffects[steamId] = new List<IEffect>();
            }
            
            // Check if effect already exists (stacking logic)
            var existingEffect = _playerEffects[steamId].FirstOrDefault(e => e.Id == effect.Id);
            if (existingEffect != null)
            {
                // Stack or refresh based on effect type
                HandleEffectStacking(existingEffect, effect);
                return;
            }
            
            // Apply new effect
            effect.AppliedAt = DateTime.UtcNow;
            _playerEffects[steamId].Add(effect);
            
            var player = GetPlayer(steamId);
            if (player != null)
            {
                effect.OnApply(player);
            }
        }
        
        public void RemoveEffect(string steamId, string effectId)
        {
            if (!_playerEffects.ContainsKey(steamId)) return;
            
            var effect = _playerEffects[steamId].FirstOrDefault(e => e.Id == effectId);
            if (effect != null)
            {
                var player = GetPlayer(steamId);
                if (player != null)
                {
                    effect.OnRemove(player);
                }
                
                _playerEffects[steamId].Remove(effect);
            }
        }
        
        public void TickEffects()
        {
            foreach (var playerEffects in _playerEffects)
            {
                var steamId = playerEffects.Key;
                var effects = playerEffects.Value.ToList(); // Copy to avoid modification during iteration
                
                foreach (var effect in effects)
                {
                    if (effect.IsExpired())
                    {
                        RemoveEffect(steamId, effect.Id);
                    }
                    else
                    {
                        var player = GetPlayer(steamId);
                        if (player != null)
                        {
                            effect.OnTick(player);
                        }
                    }
                }
            }
        }
        
        private void HandleEffectStacking(IEffect existing, IEffect newEffect)
        {
            // Default: Refresh duration
            existing.AppliedAt = DateTime.UtcNow;
        }
    }
}
```

## 🎯 Konkrete Effects

### StunEffect

```csharp
namespace UltimateHeroes.Infrastructure.Effects.Effects
{
    public class StunEffect : IEffect
    {
        public string Id => "stun";
        public string DisplayName => "Stunned";
        public float Duration { get; set; } = 2f;
        public DateTime AppliedAt { get; set; }
        
        public void OnApply(CCSPlayerController player)
        {
            // Disable movement
            player.PlayerPawn.Value.MovementServices?.Disable();
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep movement disabled
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            // Re-enable movement
            player.PlayerPawn.Value.MovementServices?.Enable();
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
```

## 🔄 Integration

1. **Skills**: Wenden Effects an
2. **Player State**: Trackt Active Effects
3. **Game Loop**: Tickt Effects regelmäßig

## ✅ Tests

- Effects werden korrekt angewendet
- Effects werden korrekt entfernt
- Effect Duration funktioniert
- Effect Stacking funktioniert

## 📝 Nächste Schritte

1. ✅ IEffect Interface definieren
2. ✅ EffectManager.cs implementieren
3. ✅ Konkrete Effects (Stun, Heal, DoT, etc.)
4. ✅ Integration mit Skills
