# 🎲 Plan: In-Match Evolution System

## 📋 Zweck

Das In-Match Evolution System ermöglicht **Mikro-Progression während eines Matches**:
- Mini-Upgrades nach bestimmten Meilensteinen
- Kill Streak Rewards
- Objective Rewards
- Zeit-basierte Upgrades (für Deathmatch)

## 🎮 Game Mode Support

### **Round-based Modes** (Competitive, Casual, Wingman)
- Evolution pro Round
- Round-basierte Rewards

### **Time-based Modes** (Deathmatch, Arms Race)
- Evolution alle X Minuten (z.B. alle 2 Minuten = 1 "Round")
- Zeit-basierte Rewards

## 📐 System Struktur

### GameModeDetector

```csharp
namespace UltimateHeroes.Application.Services
{
    public enum GameMode
    {
        Competitive,    // Round-based
        Casual,         // Round-based
        Wingman,        // Round-based
        Deathmatch,     // Time-based
        ArmsRace,       // Time-based
        Unknown
    }
    
    public class GameModeDetector
    {
        public static GameMode DetectCurrentMode()
        {
            // Prüfe Server-Variablen oder Map-Name
            // z.B. mp_gamemode, game_mode, etc.
            return GameMode.Unknown;
        }
        
        public static bool IsRoundBased(GameMode mode)
        {
            return mode == GameMode.Competitive || 
                   mode == GameMode.Casual || 
                   mode == GameMode.Wingman;
        }
        
        public static bool IsTimeBased(GameMode mode)
        {
            return mode == GameMode.Deathmatch || 
                   mode == GameMode.ArmsRace;
        }
    }
}
```

### InMatchEvolution

```csharp
namespace UltimateHeroes.Application.Services
{
    public class InMatchEvolution
    {
        private readonly Dictionary<string, MatchProgress> _matchProgress = new();
        private readonly GameModeDetector _gameModeDetector;
        
        // Round-based: Evolution pro Round
        public void OnRoundStart(string steamId, int roundNumber)
        {
            // Mini-Upgrade nach Round 3, 6, 9, etc.
            if (roundNumber % 3 == 0)
            {
                AwardMiniUpgrade(steamId);
            }
        }
        
        // Time-based: Evolution alle X Minuten
        public void OnTimeInterval(string steamId, float minutesElapsed)
        {
            // Mini-Upgrade alle 2 Minuten
            int intervals = (int)(minutesElapsed / 2f);
            var progress = GetOrCreateProgress(steamId);
            
            if (intervals > progress.LastUpgradeInterval)
            {
                AwardMiniUpgrade(steamId);
                progress.LastUpgradeInterval = intervals;
            }
        }
        
        // Kill Streak Rewards
        public void OnKillStreak(string steamId, int streak)
        {
            if (streak >= 3) AwardKillStreakReward(steamId, streak);
        }
        
        // Objective Rewards
        public void OnObjective(string steamId, ObjectiveType type)
        {
            AwardObjectiveReward(steamId, type);
        }
    }
}
```

## 🎯 Mini-Upgrades

Mini-Upgrades sind **temporäre Buffs** für das aktuelle Match:
- +5% Damage
- +10% Movement Speed
- -10% Cooldown Reduction
- +20 HP
- etc.

## ⚠️ **WICHTIG: Für MVP NICHT notwendig!**

Dieses System ist **Phase 2+** und kann später hinzugefügt werden, wenn:
- Core-Systeme stabil sind
- Balance getestet wurde
- Community Feedback vorhanden ist

---

## 📝 **Empfehlung**

**Für MVP: NICHT implementieren**
- Core-Systeme sind wichtiger
- Kann später hinzugefügt werden
- Komplexität ist hoch für MVP

**Für Phase 2:**
- GameModeDetector implementieren
- InMatchEvolution mit Round/Time Support
- Mini-Upgrade System
- Kill Streak Tracking
