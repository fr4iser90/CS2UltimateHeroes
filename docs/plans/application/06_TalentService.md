# 🌳 Plan: TalentService

## 📋 Zweck

Der TalentService verwaltet Talents für Spieler:
- Talent Unlocking
- Talent Point Allocation
- Talent Effects Application

## 🔗 Abhängigkeiten

- `TalentTree` (Domain/Talents/TalentTree.cs) - später
- `TalentNode` (Domain/Talents/TalentNode.cs) - später
- `PlayerTalents` (Domain/Talents/PlayerTalents.cs) - später
- `ITalentRepository` (Infrastructure/Database) - später

## 📐 Service Interface

```csharp
namespace UltimateHeroes.Application.Services
{
    public interface ITalentService
    {
        // Talent Points
        void AwardTalentPoints(string steamId, int points);
        int GetAvailablePoints(string steamId);
        
        // Talent Management
        bool UnlockTalent(string steamId, string talentId);
        bool CanUnlockTalent(string steamId, string talentId);
        List<TalentNode> GetUnlockableTalents(string steamId, TalentTreeType treeType);
        List<TalentNode> GetUnlockedTalents(string steamId, TalentTreeType treeType);
        
        // Talent Trees
        TalentTree GetTalentTree(TalentTreeType treeType);
        List<TalentTree> GetAllTalentTrees();
        
        // Talent Effects
        Dictionary<string, float> GetTalentModifiers(string steamId);
    }
}
```

## 🎯 Implementierung

### Datei: `Application/Services/TalentService.cs`

```csharp
namespace UltimateHeroes.Application.Services
{
    public class TalentService : ITalentService
    {
        private readonly ITalentRepository _talentRepository;
        private readonly Dictionary<TalentTreeType, TalentTree> _talentTrees = new();
        
        public TalentService(ITalentRepository talentRepository)
        {
            _talentRepository = talentRepository;
            InitializeTalentTrees();
        }
        
        private void InitializeTalentTrees()
        {
            _talentTrees[TalentTreeType.Combat] = new TalentTree
            {
                Type = TalentTreeType.Combat,
                DisplayName = "Combat",
                Nodes = TalentDefinitions.GetCombatTalents()
            };
            
            _talentTrees[TalentTreeType.Utility] = new TalentTree
            {
                Type = TalentTreeType.Utility,
                DisplayName = "Utility",
                Nodes = TalentDefinitions.GetUtilityTalents()
            };
            
            _talentTrees[TalentTreeType.Movement] = new TalentTree
            {
                Type = TalentTreeType.Movement,
                DisplayName = "Movement",
                Nodes = TalentDefinitions.GetMovementTalents()
            };
        }
        
        public void AwardTalentPoints(string steamId, int points)
        {
            var playerTalents = _talentRepository.GetPlayerTalents(steamId);
            if (playerTalents == null)
            {
                playerTalents = new PlayerTalents { SteamId = steamId };
            }
            
            playerTalents.AvailableTalentPoints += points;
            _talentRepository.SavePlayerTalents(playerTalents);
        }
        
        public int GetAvailablePoints(string steamId)
        {
            var playerTalents = _talentRepository.GetPlayerTalents(steamId);
            return playerTalents?.AvailableTalentPoints ?? 0;
        }
        
        public bool UnlockTalent(string steamId, string talentId)
        {
            if (!CanUnlockTalent(steamId, talentId))
            {
                return false;
            }
            
            var playerTalents = _talentRepository.GetPlayerTalents(steamId);
            if (playerTalents == null)
            {
                playerTalents = new PlayerTalents { SteamId = steamId };
            }
            
            // Find Talent Node
            TalentNode? talentNode = null;
            foreach (var tree in _talentTrees.Values)
            {
                talentNode = tree.GetNode(talentId);
                if (talentNode != null) break;
            }
            
            if (talentNode == null) return false;
            
            // Unlock Talent
            playerTalents.UnlockTalent(talentId, 1);
            _talentRepository.SavePlayerTalents(playerTalents);
            
            return true;
        }
        
        public bool CanUnlockTalent(string steamId, string talentId)
        {
            var playerTalents = _talentRepository.GetPlayerTalents(steamId);
            if (playerTalents == null) return false;
            
            // Find Talent Node
            TalentNode? talentNode = null;
            foreach (var tree in _talentTrees.Values)
            {
                talentNode = tree.GetNode(talentId);
                if (talentNode != null) break;
            }
            
            if (talentNode == null) return false;
            
            return playerTalents.CanUnlock(talentNode);
        }
        
        public List<TalentNode> GetUnlockableTalents(string steamId, TalentTreeType treeType)
        {
            var playerTalents = _talentRepository.GetPlayerTalents(steamId);
            if (playerTalents == null) return new List<TalentNode>();
            
            var tree = _talentTrees.GetValueOrDefault(treeType);
            if (tree == null) return new List<TalentNode>();
            
            return tree.GetUnlockableNodes(playerTalents.UnlockedTalents);
        }
        
        public List<TalentNode> GetUnlockedTalents(string steamId, TalentTreeType treeType)
        {
            var playerTalents = _talentRepository.GetPlayerTalents(steamId);
            if (playerTalents == null) return new List<TalentNode>();
            
            var tree = _talentTrees.GetValueOrDefault(treeType);
            if (tree == null) return new List<TalentNode>();
            
            return tree.Nodes.Where(n => playerTalents.IsUnlocked(n.Id)).ToList();
        }
        
        public TalentTree GetTalentTree(TalentTreeType treeType)
        {
            return _talentTrees.GetValueOrDefault(treeType) ?? new TalentTree { Type = treeType };
        }
        
        public List<TalentTree> GetAllTalentTrees()
        {
            return _talentTrees.Values.ToList();
        }
        
        public Dictionary<string, float> GetTalentModifiers(string steamId)
        {
            var playerTalents = _talentRepository.GetPlayerTalents(steamId);
            if (playerTalents == null) return new Dictionary<string, float>();
            
            var modifiers = new Dictionary<string, float>();
            
            foreach (var tree in _talentTrees.Values)
            {
                foreach (var node in tree.Nodes)
                {
                    if (playerTalents.IsUnlocked(node.Id))
                    {
                        var level = playerTalents.GetTalentLevel(node.Id);
                        ApplyTalentEffect(node.Effect, level, modifiers);
                    }
                }
            }
            
            return modifiers;
        }
        
        private void ApplyTalentEffect(TalentEffect effect, int level, Dictionary<string, float> modifiers)
        {
            foreach (var param in effect.Parameters)
            {
                var key = param.Key;
                var value = param.Value * level; // Scale by level
                
                if (modifiers.ContainsKey(key))
                {
                    modifiers[key] += value;
                }
                else
                {
                    modifiers[key] = value;
                }
            }
        }
    }
}
```

## 🔄 Integration

1. **XpService**: Verleiht Talent Points bei Level-Up
2. **TalentRepository**: Persistiert Talents
3. **Player State**: Wendet Talent Effects an
4. **Menu System**: Talent Tree Display

## ✅ Tests

- Talent Points werden korrekt vergeben
- Talent Unlocking funktioniert
- Prerequisites werden geprüft
- Talent Effects werden korrekt angewendet

## 📝 Nächste Schritte

1. ✅ ITalentService Interface definieren
2. ✅ TalentService.cs implementieren
3. ✅ Integration mit TalentRepository
4. ✅ Integration mit XpService
