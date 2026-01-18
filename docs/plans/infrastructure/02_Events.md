# 📡 Plan: Event System

## 📋 Zweck

Das Event System verwaltet Game Events:
- Event Registration
- Event Dispatching
- Event Handlers

## 🔗 Abhängigkeiten

- CounterStrikeSharp.API Events

## 📐 Event System Struktur

### EventHandler Interface

```csharp
namespace UltimateHeroes.Infrastructure.Events
{
    public interface IEventHandler<T> where T : IGameEvent
    {
        void Handle(T eventData);
    }
    
    public interface IGameEvent
    {
        string EventName { get; }
        DateTime Timestamp { get; }
    }
}
```

### Event System

```csharp
namespace UltimateHeroes.Infrastructure.Events
{
    public class EventSystem
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();
        
        public void RegisterHandler<T>(IEventHandler<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<object>();
            }
            
            _handlers[eventType].Add(handler);
        }
        
        public void Dispatch<T>(T eventData) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (!_handlers.ContainsKey(eventType)) return;
            
            foreach (var handler in _handlers[eventType])
            {
                if (handler is IEventHandler<T> typedHandler)
                {
                    typedHandler.Handle(eventData);
                }
            }
        }
    }
}
```

## 🎯 Event Handlers

### PlayerHurtHandler

```csharp
namespace UltimateHeroes.Infrastructure.Events.EventHandlers
{
    public class PlayerHurtHandler : IEventHandler<PlayerHurtEvent>
    {
        private readonly IXpService _xpService;
        private readonly IPlayerService _playerService;
        
        public void Handle(PlayerHurtEvent eventData)
        {
            var player = _playerService.GetPlayer(eventData.AttackerSteamId);
            if (player == null) return;
            
            // Trigger Passive Skills
            foreach (var skill in player.ActiveSkills)
            {
                if (skill is IPassiveSkill passiveSkill)
                {
                    passiveSkill.OnPlayerHurt(eventData.Player, eventData.Damage);
                }
            }
        }
    }
}
```

### PlayerKillHandler

```csharp
namespace UltimateHeroes.Infrastructure.Events.EventHandlers
{
    public class PlayerKillHandler : IEventHandler<PlayerKillEvent>
    {
        private readonly IXpService _xpService;
        private readonly IPlayerService _playerService;
        
        public void Handle(PlayerKillEvent eventData)
        {
            var player = _playerService.GetPlayer(eventData.KillerSteamId);
            if (player == null) return;
            
            // Award XP
            _xpService.AwardXp(eventData.KillerSteamId, XpSource.Kill);
            
            if (eventData.IsHeadshot)
            {
                _xpService.AwardXp(eventData.KillerSteamId, XpSource.Headshot);
            }
            
            // Trigger Passive Skills
            foreach (var skill in player.ActiveSkills)
            {
                if (skill is IPassiveSkill passiveSkill)
                {
                    passiveSkill.OnPlayerKill(eventData.Player, eventData.Victim);
                }
            }
        }
    }
}
```

## 🔄 Integration

1. **Plugin Load**: Registriert Event Handlers
2. **CounterStrikeSharp Events**: Hookt Game Events
3. **Services**: Nutzen Event System

## ✅ Tests

- Events werden korrekt dispatched
- Handlers werden korrekt aufgerufen
- Event Registration funktioniert

## 📝 Nächste Schritte

1. ✅ EventSystem.cs implementieren
2. ✅ Event Handler Interfaces
3. ✅ Konkrete Event Handlers
4. ✅ Integration mit CounterStrikeSharp Events
