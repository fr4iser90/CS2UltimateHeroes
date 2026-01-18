# 🌳 Plan: Talent System

## 📋 Zweck

Das Talent System ermöglicht nicht-lineare Progression durch Talent Trees:
- Combat Tree (Damage, Recoil, Armor Pen)
- Utility Tree (Nades, Plant/Defuse Speed)
- Movement Tree (Air Control, Ladder Speed, Silent Drop)

## 🔗 Abhängigkeiten

- `UltimatePlayer` (Domain/Players/UltimatePlayer.cs) - später
- `XpSystem` (Domain/Progression/XpSystem.cs) - später

## 📐 Talent Struktur

### TalentNode

```csharp
namespace UltimateHeroes.Domain.Talents
{
    public class TalentNode
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        
        // Tree & Position
        public TalentTreeType TreeType { get; set; }
        public int Row { get; set; } // 1-5
        public int Column { get; set; } // 1-3
        
        // Progression
        public int MaxLevel { get; set; } = 5;
        public List<string> Prerequisites { get; set; } = new(); // Talent IDs
        
        // Effects
        public TalentEffect Effect { get; set; }
        
        // Helper
        public bool CanUnlock(List<string> unlockedTalents)
        {
            if (Prerequisites.Count == 0) return true;
            return Prerequisites.All(p => unlockedTalents.Contains(p));
        }
    }
}
```

### TalentTree

```csharp
namespace UltimateHeroes.Domain.Talents
{
    public class TalentTree
    {
        public TalentTreeType Type { get; set; }
        public string DisplayName { get; set; }
        public List<TalentNode> Nodes { get; set; } = new();
        
        // Helper
        public TalentNode? GetNode(string nodeId)
        {
            return Nodes.FirstOrDefault(n => n.Id == nodeId);
        }
        
        public List<TalentNode> GetUnlockableNodes(List<string> unlockedTalents)
        {
            return Nodes.Where(n => n.CanUnlock(unlockedTalents) && !unlockedTalents.Contains(n.Id)).ToList();
        }
    }
    
    public enum TalentTreeType
    {
        Combat,
        Utility,
        Movement
    }
}
```

### TalentEffect

```csharp
namespace UltimateHeroes.Domain.Talents
{
    public class TalentEffect
    {
        public TalentEffectType Type { get; set; }
        public Dictionary<string, float> Parameters { get; set; } = new();
        
        // z.B. { "damage_bonus": 0.05f } = +5% Damage
        // z.B. { "recoil_reduction": 0.1f } = -10% Recoil
    }
    
    public enum TalentEffectType
    {
        DamageBonus,
        RecoilReduction,
        ArmorPenetration,
        ExtraNade,
        PlantSpeed,
        DefuseSpeed,
        AirControl,
        LadderSpeed,
        SilentDrop
    }
}
```

### PlayerTalents

```csharp
namespace UltimateHeroes.Domain.Talents
{
    public class PlayerTalents
    {
        public string SteamId { get; set; }
        public List<string> UnlockedTalents { get; set; } = new(); // Talent IDs
        public Dictionary<string, int> TalentLevels { get; set; } = new(); // talent_id -> level
        public int AvailableTalentPoints { get; set; } = 0;
        
        // Helper
        public bool IsUnlocked(string talentId)
        {
            return UnlockedTalents.Contains(talentId);
        }
        
        public int GetTalentLevel(string talentId)
        {
            return TalentLevels.GetValueOrDefault(talentId, 0);
        }
        
        public bool CanUnlock(TalentNode node)
        {
            if (AvailableTalentPoints <= 0) return false;
            if (IsUnlocked(node.Id)) return false;
            return node.CanUnlock(UnlockedTalents);
        }
        
        public void UnlockTalent(string talentId, int level = 1)
        {
            if (!UnlockedTalents.Contains(talentId))
            {
                UnlockedTalents.Add(talentId);
            }
            TalentLevels[talentId] = level;
            AvailableTalentPoints--;
        }
    }
}
```

## 🎯 Talent Definitions

### Combat Tree

**Row 1:**
- Headshot Damage +5/10/15/20/25% (5 Levels)
- Recoil Control -5/10/15/20/25% (5 Levels)
- Armor Penetration +2/4/6/8/10% (5 Levels)

**Row 2:**
- Damage per Kill +1/2/3/4/5 (5 Levels)
- Reload Speed +10/20/30/40/50% (5 Levels)
- Weapon Accuracy +5/10/15/20/25% (5 Levels)

**Row 3-5:**
- Weitere Combat Talents...

### Utility Tree

**Row 1:**
- Extra Nade (1/2/3) (3 Levels)
- Faster Plant -0.5/1/1.5/2/2.5s (5 Levels)
- Defuse Speed +10/20/30/40/50% (5 Levels)

**Row 2:**
- Flash Duration +0.5/1/1.5/2/2.5s (5 Levels)
- Smoke Duration +2/4/6/8/10s (5 Levels)
- Utility Cooldown -5/10/15/20/25% (5 Levels)

**Row 3-5:**
- Weitere Utility Talents...

### Movement Tree

**Row 1:**
- Air Control +10/20/30/40/50% (5 Levels)
- Faster Ladder +15/30/45/60/75% (5 Levels)
- Silent Drop (on/off) (1 Level)

**Row 2:**
- Movement Speed +5/10/15/20/25% (5 Levels)
- Jump Height +10/20/30/40/50% (5 Levels)
- Fall Damage Reduction -20/40/60/80/100% (5 Levels)

**Row 3-5:**
- Weitere Movement Talents...

## 🎯 Implementierung

### Schritt 1: TalentNode.cs

**Datei**: `Domain/Talents/TalentNode.cs`

- Talent Node Definition
- Prerequisites Logic
- Unlock Logic

### Schritt 2: TalentTree.cs

**Datei**: `Domain/Talents/TalentTree.cs`

- Talent Tree Structure
- Node Management

### Schritt 3: TalentEffect.cs

**Datei**: `Domain/Talents/TalentEffect.cs`

- Effect Definition
- Parameter System

### Schritt 4: PlayerTalents.cs

**Datei**: `Domain/Talents/PlayerTalents.cs`

- Player Talent Progress
- Unlock Logic

### Schritt 5: Talent Definitions

**Datei**: `Domain/Talents/TalentDefinitions.cs`

```csharp
public static class TalentDefinitions
{
    public static List<TalentNode> GetCombatTalents()
    {
        return new List<TalentNode>
        {
            new TalentNode
            {
                Id = "combat_headshot_damage",
                DisplayName = "Headshot Damage",
                TreeType = TalentTreeType.Combat,
                Row = 1,
                Column = 1,
                MaxLevel = 5,
                Effect = new TalentEffect
                {
                    Type = TalentEffectType.DamageBonus,
                    Parameters = new Dictionary<string, float>
                    {
                        { "headshot_bonus", 0.05f } // +5% per level
                    }
                }
            },
            // ... weitere Talents
        };
    }
    
    public static List<TalentNode> GetUtilityTalents() { ... }
    public static List<TalentNode> GetMovementTalents() { ... }
}
```

## 🔄 Integration

1. **TalentService**: Verwaltet Talents
2. **XpService**: Verleiht Talent Points bei Level-Up
3. **Player State**: Speichert unlocked Talents
4. **Effect System**: Wendet Talent Effects an
5. **Menu System**: Talent Tree Display

## 📊 Database Schema

```sql
CREATE TABLE talents (
    steamid TEXT NOT NULL,
    talent_id TEXT NOT NULL,
    talent_level INTEGER DEFAULT 1,
    PRIMARY KEY (steamid, talent_id),
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

CREATE TABLE talent_points (
    steamid TEXT PRIMARY KEY,
    available_points INTEGER DEFAULT 0,
    total_earned INTEGER DEFAULT 0,
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);
```

## ✅ Tests

- Talent Unlocking funktioniert
- Prerequisites werden geprüft
- Talent Points werden korrekt vergeben
- Talent Effects werden angewendet
- Talent Tree Navigation funktioniert

## 📝 Nächste Schritte

1. ✅ TalentNode.cs implementieren
2. ✅ TalentTree.cs implementieren
3. ✅ TalentEffect.cs implementieren
4. ✅ PlayerTalents.cs implementieren
5. ✅ TalentDefinitions.cs (Combat/Utility/Movement)
6. ✅ TalentService implementieren (siehe Application Plan)
