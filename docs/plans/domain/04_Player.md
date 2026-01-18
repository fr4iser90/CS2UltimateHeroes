# 👤 Plan: Player State Domain Model

## 📋 Zweck

Das Player State Model repräsentiert den aktuellen Zustand eines Spielers im Match:
- Aktiver Hero
- Aktiver Build
- Aktive Skills (mit Level)
- Active Effects
- Cooldowns
- XP, Level, Talents

## 🔗 Abhängigkeiten

- `IHero` (Domain/Heroes/IHero.cs) ✅
- `ISkill` (Domain/Skills/ISkill.cs) ✅
- `Build` (Domain/Builds/Build.cs) - später
- `EffectManager` (Infrastructure) - später
- `CooldownManager` (Infrastructure) - später

## 📐 Player State Struktur

### UltimatePlayer Entity

```csharp
namespace UltimateHeroes.Domain.Players
{
    public class UltimatePlayer
    {
        // Identity
        public string SteamId { get; set; }
        public CCSPlayerController? PlayerController { get; set; }
        
        // Current State
        public IHero? CurrentHero { get; set; }
        public Build? CurrentBuild { get; set; }
        public List<ISkill> ActiveSkills { get; set; } = new();
        public Dictionary<string, int> SkillLevels { get; set; } = new(); // skill_id -> level
        
        // Progression
        public int HeroLevel { get; set; } = 1;
        public float CurrentXp { get; set; } = 0f;
        public float XpToNextLevel { get; set; } = 100f;
        
        // Active Effects
        public List<IEffect> ActiveEffects { get; set; } = new();
        
        // Cooldowns
        public Dictionary<string, DateTime> SkillCooldowns { get; set; } = new(); // skill_id -> cooldown_end
        
        // Match Stats
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int Headshots { get; set; } = 0;
        
        // Role Influence
        public RoleInfluence CurrentRole { get; set; } = RoleInfluence.None;
        
        // Helper Methods
        public bool IsSkillReady(string skillId)
        {
            if (!SkillCooldowns.ContainsKey(skillId)) return true;
            return DateTime.UtcNow >= SkillCooldowns[skillId];
        }
        
        public void SetSkillCooldown(string skillId, float cooldownSeconds)
        {
            SkillCooldowns[skillId] = DateTime.UtcNow.AddSeconds(cooldownSeconds);
        }
        
        public void ActivateBuild(Build build, IHero hero, List<ISkill> skills)
        {
            CurrentBuild = build;
            CurrentHero = hero;
            ActiveSkills = skills;
            
            // Initialize Hero Passives
            hero.OnPlayerSpawn(PlayerController);
        }
    }
}
```

### PlayerBuild (Active Build Reference)

```csharp
namespace UltimateHeroes.Domain.Players
{
    public class PlayerBuild
    {
        public string SteamId { get; set; }
        public Build Build { get; set; }
        public IHero Hero { get; set; }
        public List<ISkill> Skills { get; set; }
        public Dictionary<string, int> SkillLevels { get; set; } // skill_id -> level
        
        // Helper
        public int GetTotalPowerWeight()
        {
            return Hero.PowerWeight + Skills.Sum(s => s.PowerWeight);
        }
        
        public bool IsValid()
        {
            // Nutzt BuildValidator
            return true;
        }
    }
}
```

### RoleInfluence Enum

```csharp
namespace UltimateHeroes.Domain.Players
{
    public enum RoleInfluence
    {
        None,
        DPS,        // Viel Damage
        Support,    // Viele Smokes/Heals
        Initiator,  // Entry Kills
        Clutch      // Clutch Rounds
    }
}
```

## 🎯 Implementierung

### Datei: `Domain/Players/UltimatePlayer.cs`

**Features:**
- Player State Management
- Build Activation
- Skill Management
- Effect Tracking
- Cooldown Tracking
- Progression Tracking

**Methoden:**
```csharp
public class UltimatePlayer
{
    // Build Management
    public void ActivateBuild(Build build, IHero hero, List<ISkill> skills);
    public void DeactivateBuild();
    
    // Skill Management
    public void AddSkill(ISkill skill, int level = 1);
    public void RemoveSkill(string skillId);
    public ISkill? GetSkill(string skillId);
    public bool CanActivateSkill(string skillId);
    
    // Effect Management
    public void AddEffect(IEffect effect);
    public void RemoveEffect(string effectId);
    public bool HasEffect(string effectId);
    
    // Progression
    public void AddXp(float amount);
    public void LevelUp();
    public float GetXpProgress(); // 0.0 - 1.0
    
    // Stats
    public void OnKill(CCSPlayerController victim);
    public void OnDeath();
    public void OnHeadshot();
}
```

### Datei: `Domain/Players/PlayerBuild.cs`

**Features:**
- Active Build Reference
- Skill Level Management
- Build Validation

## 🔄 Integration

1. **PlayerService**: Verwaltet Player States
2. **BuildService**: Aktiviert Builds für Player
3. **SkillService**: Aktiviert Skills für Player
4. **EffectManager**: Wendet Effects an
5. **CooldownManager**: Trackt Cooldowns
6. **XpService**: Awarded XP

## 📊 Database Schema

```sql
CREATE TABLE players (
    steamid TEXT PRIMARY KEY,
    hero_core_id TEXT,
    hero_level INTEGER DEFAULT 1,
    current_xp REAL DEFAULT 0,
    xp_to_next_level REAL DEFAULT 100,
    current_role TEXT,
    last_updated TEXT DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE player_skills (
    steamid TEXT NOT NULL,
    skill_id TEXT NOT NULL,
    skill_level INTEGER DEFAULT 1,
    PRIMARY KEY (steamid, skill_id),
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

CREATE TABLE player_stats (
    steamid TEXT PRIMARY KEY,
    total_kills INTEGER DEFAULT 0,
    total_deaths INTEGER DEFAULT 0,
    total_assists INTEGER DEFAULT 0,
    total_headshots INTEGER DEFAULT 0,
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);
```

## ✅ Tests

- Player State wird korrekt initialisiert
- Build Activation funktioniert
- Skills werden korrekt aktiviert
- Cooldowns werden getrackt
- XP wird korrekt vergeben
- Level-Ups funktionieren

## 📝 Nächste Schritte

1. ✅ UltimatePlayer.cs implementieren
2. ✅ PlayerBuild.cs implementieren
3. ✅ RoleInfluence Enum
4. ✅ Database Schema erstellen
5. ✅ PlayerRepository implementieren (siehe Infrastructure Plan)
