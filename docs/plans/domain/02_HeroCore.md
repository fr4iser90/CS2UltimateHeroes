# 🎭 Plan: Hero Core Implementations

## 📋 Zweck

Hero Cores sind die Basis-Identität eines Heroes. Sie definieren:
- Passive Fähigkeiten (immer aktiv)
- Hero Identity Auras (Modifier für Skills)
- Power Weight (für Power Budget)

## 🔗 Abhängigkeiten

- `IHero` (Domain/Heroes/IHero.cs) ✅
- `HeroIdentity` (Domain/Heroes/HeroIdentity.cs) ✅
- `IPassiveSkill` (Domain/Skills/ISkill.cs) ✅

## 📐 Hero Core Struktur

### Base Class: HeroCore

```csharp
namespace UltimateHeroes.Domain.Heroes
{
    public abstract class HeroCore : IHero
    {
        public abstract string Id { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract int PowerWeight { get; }
        
        public abstract List<IPassiveSkill> PassiveSkills { get; }
        public abstract HeroIdentity Identity { get; }
        
        // Helper
        public virtual void OnPlayerSpawn(CCSPlayerController player)
        {
            foreach (var passive in PassiveSkills)
            {
                passive.OnPlayerSpawn(player);
            }
        }
    }
}
```

## 🎯 Konkrete Hero Implementierungen

### 1. Vanguard (Tank)

**Datei**: `Domain/Heroes/ConcreteHeroes/Vanguard.cs`

**Properties:**
- PowerWeight: 30
- Passive: +5 Armor per Kill, Frontshield Active
- Identity: +10% Defense Skills, +20% Shield Duration

**Passive Skills:**
- `ArmorPerKillPassive`: +5 Armor nach Kill
- `FrontshieldPassive`: Frontshield bei Spawn

**Identity Modifiers:**
```csharp
TagModifiers = {
    { SkillTag.Defense, 1.1f },  // +10% Defense
    { SkillTag.Support, 1.05f }  // +5% Support
}
SpecialBonuses = {
    { "ShieldDuration", 1.2f }  // +20% Shield Duration
}
```

### 2. Phantom (Stealth)

**Datei**: `Domain/Heroes/ConcreteHeroes/Phantom.cs`

**Properties:**
- PowerWeight: 30
- Passive: Silent Footsteps, Blink Active
- Identity: -10% Mobility Cooldown, +15% Backstab Damage

**Passive Skills:**
- `SilentFootstepsPassive`: Keine Footstep-Sounds
- `BlinkPassive`: Blink bei Spawn (1x)

**Identity Modifiers:**
```csharp
TagModifiers = {
    { SkillTag.Mobility, 0.9f },  // -10% Cooldown
    { SkillTag.Stealth, 1.1f }   // +10% Stealth
}
SpecialBonuses = {
    { "BackstabDamage", 1.15f }  // +15% Backstab
}
```

### 3. Engineer (Tech)

**Datei**: `Domain/Heroes/ConcreteHeroes/Engineer.cs`

**Properties:**
- PowerWeight: 30
- Passive: Mini-Sentry, Utility Cooldown Reduction
- Identity: +25% Utility Range, +30% Sentry Damage

**Passive Skills:**
- `MiniSentryPassive`: Spawnt Mini-Sentry
- `UtilityCooldownReductionPassive`: -15% Utility Cooldown

**Identity Modifiers:**
```csharp
TagModifiers = {
    { SkillTag.Utility, 1.25f },  // +25% Range
    { SkillTag.Area, 1.1f }       // +10% Area
}
SpecialBonuses = {
    { "SentryDamage", 1.3f }     // +30% Sentry Damage
}
```

## 🎯 Implementierung

### Schritt 1: HeroCore Base Class

**Datei**: `Domain/Heroes/HeroCore.cs`

```csharp
public abstract class HeroCore : IHero
{
    // Abstract Properties (müssen implementiert werden)
    public abstract string Id { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }
    public abstract int PowerWeight { get; }
    public abstract List<IPassiveSkill> PassiveSkills { get; }
    public abstract HeroIdentity Identity { get; }
    
    // Helper Methods
    public virtual void Initialize(CCSPlayerController player) { }
    public virtual void OnRoundStart(CCSPlayerController player) { }
}
```

### Schritt 2: Konkrete Heroes

Für jeden Hero:
1. Erstelle Klasse die von `HeroCore` erbt
2. Implementiere alle abstract Properties
3. Definiere Passive Skills
4. Definiere HeroIdentity

### Schritt 3: Hero Registry

**Datei**: `Application/Services/HeroService.cs` (siehe Application Plan)

Heroes werden registriert:
```csharp
var heroes = new List<IHero>
{
    new Vanguard(),
    new Phantom(),
    new Engineer()
};
```

## 📊 Hero Properties Tabelle

| Hero | PowerWeight | Passive Skills | Identity Focus |
|------|-------------|----------------|----------------|
| Vanguard | 30 | Armor/Kill, Frontshield | Defense +10% |
| Phantom | 30 | Silent, Blink | Mobility -10% CD |
| Engineer | 30 | Mini-Sentry, Utility CDR | Utility +25% Range |

## 🔄 Integration

1. **HeroService**: Registriert und verwaltet Heroes
2. **Build System**: Heroes werden in Builds gespeichert
3. **Player State**: Aktiver Hero wird gespeichert
4. **Menu System**: Hero-Auswahl im Menu

## ✅ Tests

- Hero PowerWeight korrekt
- Passive Skills werden aktiviert
- Identity Modifiers werden angewendet
- Hero Registry funktioniert

## 📝 Nächste Schritte

1. ✅ HeroCore.cs (Base Class) implementieren
2. ✅ Vanguard.cs implementieren
3. ✅ Phantom.cs implementieren
4. ✅ Engineer.cs implementieren
5. ✅ Weitere Heroes (später)
