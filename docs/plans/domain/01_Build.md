# 🏗️ Plan: Build Domain Model

## 📋 Zweck

Das Build Domain Model repräsentiert eine gespeicherte Build-Konfiguration eines Spielers. Ein Build besteht aus:
- Hero Core (z.B. Vanguard)
- 2-3 Skills (z.B. Fireball, Blink, HealingAura)
- Build Name (z.B. "Tanky Assassin")
- Build Slot (1-5)

## 🔗 Abhängigkeiten

- `IHero` (Domain/Heroes/IHero.cs) ✅
- `ISkill` (Domain/Skills/ISkill.cs) ✅
- `BuildValidator` (Application/BuildValidator.cs) ✅

## 📐 Datenstruktur

### Build Entity

```csharp
namespace UltimateHeroes.Domain.Builds
{
    public class Build
    {
        // Primary Key
        public string SteamId { get; set; }
        public int BuildSlot { get; set; } // 1-5
        
        // Build Configuration
        public string HeroCoreId { get; set; }
        public List<string> SkillIds { get; set; } // Max 3 Skills
        
        // Metadata
        public string BuildName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }
        
        // Validation
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; }
    }
}
```

### BuildSlot Entity

```csharp
namespace UltimateHeroes.Domain.Builds
{
    public class BuildSlot
    {
        public int SlotNumber { get; set; } // 1-5
        public bool IsUnlocked { get; set; }
        public int UnlockLevel { get; set; } // z.B. Slot 4 bei Level 10
        
        public static Dictionary<int, int> UnlockRequirements = new()
        {
            { 1, 1 },   // Slot 1: Level 1 (immer)
            { 2, 1 },   // Slot 2: Level 1 (immer)
            { 3, 1 },   // Slot 3: Level 1 (immer)
            { 4, 10 },  // Slot 4: Level 10
            { 5, 20 }   // Slot 5: Level 20
        };
    }
}
```

## 🎯 Implementierung

### Datei: `Domain/Builds/Build.cs`

**Features:**
- Build Entity mit allen Properties
- Validation Logic (nutzt BuildValidator)
- Helper Methods (IsEmpty, CanActivate, etc.)

**Methoden:**
```csharp
public class Build
{
    // Validation
    public ValidationResult Validate(IHero heroCore, List<ISkill> skills)
    {
        // Nutzt BuildValidator
    }
    
    // Helper
    public bool IsEmpty() => string.IsNullOrEmpty(HeroCoreId);
    public bool CanActivate() => IsValid && !IsEmpty();
    public int GetPowerWeight(IHero heroCore, List<ISkill> skills)
    {
        return heroCore.PowerWeight + skills.Sum(s => s.PowerWeight);
    }
}
```

### Datei: `Domain/Builds/BuildSlot.cs`

**Features:**
- Build Slot Management
- Unlock Progression
- Static Unlock Requirements

## 🔄 Integration

### Mit anderen Systemen:

1. **BuildValidator**: Validiert Builds vor dem Speichern
2. **BuildService**: Erstellt/Lädt/Speichert Builds
3. **Database Repository**: Persistiert Builds
4. **Player State**: Aktiviert Builds für Spieler

## 📊 Database Schema

```sql
CREATE TABLE builds (
    steamid TEXT NOT NULL,
    build_slot INTEGER NOT NULL,
    hero_core_id TEXT NOT NULL,
    skill1_id TEXT,
    skill2_id TEXT,
    skill3_id TEXT,
    build_name TEXT,
    is_active INTEGER DEFAULT 0,
    created_at TEXT DEFAULT CURRENT_TIMESTAMP,
    last_used_at TEXT,
    PRIMARY KEY (steamid, build_slot)
);

CREATE INDEX idx_builds_steamid ON builds(steamid);
CREATE INDEX idx_builds_active ON builds(steamid, is_active) WHERE is_active = 1;
```

## ✅ Tests

- Build Validation (Power Budget, Rules)
- Build Slot Unlocking
- Build Activation/Deactivation
- Empty Build Handling

## 📝 Nächste Schritte

1. ✅ Build.cs implementieren
2. ✅ BuildSlot.cs implementieren
3. ✅ Database Schema erstellen
4. ✅ BuildRepository implementieren (siehe Infrastructure Plan)
