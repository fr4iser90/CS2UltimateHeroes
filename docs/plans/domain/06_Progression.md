# 📈 Plan: Progression System (XP, Level, Mastery)

## 📋 Zweck

Das Progression System verwaltet:
- XP System (XP Quellen, Level Calculation)
- Level System (Hero Level, Skill Level)
- Skill Mastery (Usage Tracking, Rewards)

## 🔗 Abhängigkeiten

- `UltimatePlayer` (Domain/Players/UltimatePlayer.cs) - später
- `ISkill` (Domain/Skills/ISkill.cs) ✅

## 📐 Progression Struktur

### XpSystem

```csharp
namespace UltimateHeroes.Domain.Progression
{
    public class XpSystem
    {
        // XP Sources
        public const float XpPerKill = 10f;
        public const float XpPerHeadshot = 5f;
        public const float XpPerAssist = 3f;
        public const float XpPerObjective = 20f;
        public const float XpPerRoundWin = 30f;
        public const float XpPerFlashAssist = 5f;
        
        // Level Calculation
        public static float GetXpForLevel(int level)
        {
            // Exponential: 100 * (1.2 ^ (level - 1))
            return 100f * (float)Math.Pow(1.2, level - 1);
        }
        
        public static int GetLevelFromXp(float totalXp)
        {
            int level = 1;
            float xp = 0f;
            
            while (xp < totalXp)
            {
                float xpNeeded = GetXpForLevel(level);
                if (xp + xpNeeded > totalXp) break;
                xp += xpNeeded;
                level++;
            }
            
            return level;
        }
        
        public static float GetXpProgress(float currentXp, int currentLevel)
        {
            float xpForCurrentLevel = GetXpForLevel(currentLevel);
            float xpInCurrentLevel = currentXp;
            
            // Subtract XP from previous levels
            for (int i = 1; i < currentLevel; i++)
            {
                xpInCurrentLevel -= GetXpForLevel(i);
            }
            
            return xpInCurrentLevel / xpForCurrentLevel; // 0.0 - 1.0
        }
    }
}
```

### LevelSystem

```csharp
namespace UltimateHeroes.Domain.Progression
{
    public class LevelSystem
    {
        // Hero Level (1-20)
        public const int MaxHeroLevel = 20;
        
        // Skill Level (1-5)
        public const int MaxSkillLevel = 5;
        
        // Talent Points
        public static int GetTalentPointsForLevel(int heroLevel)
        {
            // 1 Talent Point pro Level
            return heroLevel;
        }
        
        // Skill Slots Unlock
        public static int GetSkillSlotsForLevel(int heroLevel)
        {
            if (heroLevel >= 20) return 5;
            if (heroLevel >= 10) return 4;
            return 3;
        }
    }
}
```

### SkillMastery

```csharp
namespace UltimateHeroes.Domain.Progression
{
    public class SkillMastery
    {
        public string SteamId { get; set; }
        public string SkillId { get; set; }
        
        // Mastery Stats
        public int Kills { get; set; } = 0;
        public int Uses { get; set; } = 0;
        public float TotalDamage { get; set; } = 0f;
        public int Escapes { get; set; } = 0; // Für Mobility Skills
        
        // Mastery Level
        public int MasteryLevel { get; set; } = 0; // 0-5
        
        // Mastery Rewards
        public List<string> UnlockedRewards { get; set; } = new();
        
        // Helper
        public void UpdateMasteryLevel()
        {
            int newLevel = CalculateMasteryLevel();
            if (newLevel > MasteryLevel)
            {
                MasteryLevel = newLevel;
                UnlockMasteryRewards(newLevel);
            }
        }
        
        private int CalculateMasteryLevel()
        {
            // Level 1: 100 Kills oder 200 Uses
            // Level 2: 500 Kills oder 1000 Uses
            // Level 3: 1000 Kills oder 2000 Uses
            // Level 4: 2500 Kills oder 5000 Uses
            // Level 5: 5000 Kills oder 10000 Uses
            
            if (Kills >= 5000 || Uses >= 10000) return 5;
            if (Kills >= 2500 || Uses >= 5000) return 4;
            if (Kills >= 1000 || Uses >= 2000) return 3;
            if (Kills >= 500 || Uses >= 1000) return 2;
            if (Kills >= 100 || Uses >= 200) return 1;
            return 0;
        }
        
        private void UnlockMasteryRewards(int level)
        {
            // Level 1: Cosmetic Trail
            // Level 2: Alt Animation
            // Level 3: Talent Modifier
            // Level 4: Mastery Badge
            // Level 5: Mastery Title
        }
    }
}
```

### XpSource Enum

```csharp
namespace UltimateHeroes.Domain.Progression
{
    public enum XpSource
    {
        Kill,
        Headshot,
        Assist,
        Objective,
        RoundWin,
        FlashAssist,
        ClutchRound,
        FirstBlood
    }
}
```

## 🎯 Implementierung

### Schritt 1: XpSystem.cs

**Datei**: `Domain/Progression/XpSystem.cs`

- XP Sources Definition
- Level Calculation
- XP Progress Calculation

### Schritt 2: LevelSystem.cs

**Datei**: `Domain/Progression/LevelSystem.cs`

- Max Levels
- Talent Points Calculation
- Skill Slots Unlock

### Schritt 3: SkillMastery.cs

**Datei**: `Domain/Progression/SkillMastery.cs`

- Mastery Tracking
- Mastery Level Calculation
- Mastery Rewards

### Schritt 4: XpHistory (optional)

**Datei**: `Domain/Progression/XpHistory.cs`

```csharp
public class XpHistory
{
    public string SteamId { get; set; }
    public XpSource Source { get; set; }
    public float Amount { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## 🔄 Integration

1. **XpService**: Awarded XP, Level Calculation
2. **Event Handlers**: Trackt Kills, Objectives, etc.
3. **Player State**: Speichert XP, Level
4. **MasteryService**: Trackt Skill Usage
5. **Database**: Persistiert Progression

## 📊 Database Schema

```sql
CREATE TABLE xp_history (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    steamid TEXT NOT NULL,
    xp_source TEXT NOT NULL,
    amount REAL NOT NULL,
    timestamp TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

CREATE TABLE skill_mastery (
    steamid TEXT NOT NULL,
    skill_id TEXT NOT NULL,
    kills INTEGER DEFAULT 0,
    uses INTEGER DEFAULT 0,
    total_damage REAL DEFAULT 0,
    escapes INTEGER DEFAULT 0,
    mastery_level INTEGER DEFAULT 0,
    PRIMARY KEY (steamid, skill_id),
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

CREATE TABLE mastery_rewards (
    steamid TEXT NOT NULL,
    skill_id TEXT NOT NULL,
    reward_id TEXT NOT NULL,
    unlocked_at TEXT DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (steamid, skill_id, reward_id),
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);
```

## ✅ Tests

- XP wird korrekt vergeben
- Level Calculation funktioniert
- XP Progress wird korrekt berechnet
- Skill Mastery wird getrackt
- Mastery Rewards werden freigeschaltet
- Talent Points werden korrekt vergeben

## 📝 Nächste Schritte

1. ✅ XpSystem.cs implementieren
2. ✅ LevelSystem.cs implementieren
3. ✅ SkillMastery.cs implementieren
4. ✅ XpHistory.cs (optional)
5. ✅ XpService implementieren (siehe Application Plan)
6. ✅ MasteryService implementieren (siehe Application Plan)
