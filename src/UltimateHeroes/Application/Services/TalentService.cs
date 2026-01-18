using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Talents;
using UltimateHeroes.Infrastructure.Database.Repositories;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Talent-Verwaltung
    /// </summary>
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
            
            // Apply Talent Modifiers immediately if player is in-game
            // This will be applied on next spawn, but we can also apply it now if player is alive
            // Note: Full application happens in PlayerService.OnPlayerSpawn
            
            return true;
        }
        
        public bool LevelUpTalent(string steamId, string talentId)
        {
            if (!CanLevelUpTalent(steamId, talentId))
            {
                return false;
            }
            
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
            
            // Level Up Talent
            playerTalents.LevelUpTalent(talentId);
            _talentRepository.SavePlayerTalents(playerTalents);
            
            return true;
        }
        
        public bool CanLevelUpTalent(string steamId, string talentId)
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
            
            return playerTalents.CanLevelUp(talentNode);
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
        
        public int GetTalentLevel(string steamId, string talentId)
        {
            var playerTalents = _talentRepository.GetPlayerTalents(steamId);
            if (playerTalents == null) return 0;
            return playerTalents.GetTalentLevel(talentId);
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
