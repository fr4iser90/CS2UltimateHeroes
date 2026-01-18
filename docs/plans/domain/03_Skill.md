# ⚡ Plan: Skill Implementations

## 📋 Zweck

Skills sind modulare Fähigkeiten, die in Build-Slots kombiniert werden können. Sie sind unabhängig von Heroes.

## 🔗 Abhängigkeiten

- `ISkill` (Domain/Skills/ISkill.cs) ✅
- `IPassiveSkill`, `IActiveSkill`, `IUltimateSkill` ✅
- `SkillTag` Enum ✅
- `CooldownManager` (Infrastructure) - später
- `EffectManager` (Infrastructure) - später

## 📐 Skill Struktur

### Base Class: SkillBase

```csharp
namespace UltimateHeroes.Domain.Skills
{
    public abstract class SkillBase : ISkill
    {
        public abstract string Id { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract SkillType Type { get; }
        public abstract int PowerWeight { get; }
        public abstract List<SkillTag> Tags { get; }
        public abstract int MaxLevel { get; }
        
        // Level-basierte Properties
        public int CurrentLevel { get; set; } = 1;
        
        // Helper
        protected float GetLevelMultiplier()
        {
            return 1.0f + (CurrentLevel - 1) * 0.1f; // +10% pro Level
        }
    }
}
```

## 🎯 Konkrete Skill Implementierungen

### 1. Fireball (Active Skill)

**Datei**: `Domain/Skills/ConcreteSkills/Fireball.cs`

**Properties:**
- Type: Active
- PowerWeight: 25
- Tags: Damage, Area
- Cooldown: 10s
- MaxLevel: 5

**Funktionalität:**
- Wirft Fireball in Blickrichtung
- Damage: 30 + (Level * 5)
- Radius: 150 + (Level * 20)
- Kann brennen (Damage Over Time)

**Implementierung:**
```csharp
public class Fireball : SkillBase, IActiveSkill
{
    public override string Id => "fireball";
    public override string DisplayName => "Fireball";
    public override string Description => "Wirft einen Feuerball";
    public override SkillType Type => SkillType.Active;
    public override int PowerWeight => 25;
    public override List<SkillTag> Tags => new() { SkillTag.Damage, SkillTag.Area };
    public override int MaxLevel => 5;
    
    public float Cooldown => 10f;
    
    public void Activate(CCSPlayerController player)
    {
        // Fireball Logic
        var damage = 30 + (CurrentLevel * 5);
        var radius = 150 + (CurrentLevel * 20);
        // ... Fireball Implementation
    }
}
```

### 2. Blink (Active Skill)

**Datei**: `Domain/Skills/ConcreteSkills/Blink.cs`

**Properties:**
- Type: Active
- PowerWeight: 20
- Tags: Mobility
- Cooldown: 5s
- MaxLevel: 5

**Funktionalität:**
- Teleportiert Spieler in Blickrichtung
- Range: 300 + (Level * 50)
- Kein Damage während Blink (0.2s)

### 3. Stealth (Active Skill)

**Datei**: `Domain/Skills/ConcreteSkills/Stealth.cs`

**Properties:**
- Type: Active
- PowerWeight: 30
- Tags: Stealth, Utility
- Cooldown: 15s
- MaxLevel: 5

**Funktionalität:**
- Macht Spieler unsichtbar
- Duration: 5s + (Level * 1s)
- Movement Speed: +20%
- Bricht bei Damage/Shoot

### 4. HealingAura (Passive Skill)

**Datei**: `Domain/Skills/ConcreteSkills/HealingAura.cs`

**Properties:**
- Type: Passive
- PowerWeight: 15
- Tags: Support, Area
- MaxLevel: 5

**Funktionalität:**
- Heilt Spieler und Teammates in Radius
- Heal per Second: 2 + (Level * 0.5)
- Radius: 200 + (Level * 30)

### 5. Teleport (Ultimate Skill)

**Datei**: `Domain/Skills/ConcreteSkills/Teleport.cs`

**Properties:**
- Type: Ultimate
- PowerWeight: 40
- Tags: Mobility, Ultimate
- Cooldown: 60s
- MaxLevel: 3

**Funktionalität:**
- Teleportiert zu beliebiger Position (auf Map)
- Range: Unlimited
- Cast Time: 2s

## 🎯 Implementierung

### Schritt 1: SkillBase

**Datei**: `Domain/Skills/SkillBase.cs`

- Abstract Base Class
- Level Management
- Helper Methods

### Schritt 2: PassiveSkillBase

**Datei**: `Domain/Skills/PassiveSkillBase.cs`

```csharp
public abstract class PassiveSkillBase : SkillBase, IPassiveSkill
{
    public override SkillType Type => SkillType.Passive;
    
    public abstract void OnPlayerSpawn(CCSPlayerController player);
    public abstract void OnPlayerHurt(CCSPlayerController player, int damage);
    public abstract void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim);
}
```

### Schritt 3: ActiveSkillBase

**Datei**: `Domain/Skills/ActiveSkillBase.cs`

```csharp
public abstract class ActiveSkillBase : SkillBase, IActiveSkill
{
    public override SkillType Type => SkillType.Active;
    public abstract float Cooldown { get; }
    public abstract void Activate(CCSPlayerController player);
}
```

### Schritt 4: Konkrete Skills

Für jeden Skill:
1. Erstelle Klasse die von Base erbt
2. Implementiere alle Properties
3. Implementiere Activate/Passive Logic

### Schritt 5: Skill Registry

**Datei**: `Application/Services/SkillService.cs` (siehe Application Plan)

Skills werden registriert:
```csharp
var skills = new List<ISkill>
{
    new Fireball(),
    new Blink(),
    new Stealth(),
    new HealingAura(),
    new Teleport()
};
```

## 📊 Skill Properties Tabelle

| Skill | Type | PowerWeight | Tags | Cooldown | MaxLevel |
|-------|------|-------------|------|----------|----------|
| Fireball | Active | 25 | Damage, Area | 10s | 5 |
| Blink | Active | 20 | Mobility | 5s | 5 |
| Stealth | Active | 30 | Stealth, Utility | 15s | 5 |
| HealingAura | Passive | 15 | Support, Area | - | 5 |
| Teleport | Ultimate | 40 | Mobility, Ultimate | 60s | 3 |

## 🔄 Integration

1. **SkillService**: Registriert und verwaltet Skills
2. **Build System**: Skills werden in Builds gespeichert
3. **CooldownManager**: Trackt Cooldowns
4. **EffectManager**: Wendet Effects an
5. **Menu System**: Skill-Auswahl im Menu

## ✅ Tests

- Skill PowerWeight korrekt
- Skill Tags korrekt
- Active Skills können aktiviert werden
- Passive Skills werden automatisch aktiviert
- Cooldowns funktionieren
- Level-Scaling funktioniert

## 📝 Nächste Schritte

1. ✅ SkillBase.cs implementieren
2. ✅ PassiveSkillBase.cs implementieren
3. ✅ ActiveSkillBase.cs implementieren
4. ✅ Fireball.cs implementieren
5. ✅ Blink.cs implementieren
6. ✅ Stealth.cs implementieren
7. ✅ HealingAura.cs implementieren
8. ✅ Teleport.cs implementieren
9. ✅ Weitere Skills (später)
