using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Players;
using UltimateHeroes.Infrastructure.Database.Repositories;
using UltimateHeroes.Application.Services;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Player State Management
    /// </summary>
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly Dictionary<string, UltimatePlayer> _playerCache = new();
        private ITalentService? _talentService;
        private IHeroService? _heroService;
        
        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }
        
        public void SetTalentService(ITalentService talentService)
        {
            _talentService = talentService;
        }
        
        public void SetHeroService(IHeroService heroService)
        {
            _heroService = heroService;
        }
        
        public UltimatePlayer GetOrCreatePlayer(string steamId, CCSPlayerController? playerController = null, bool isBot = false, string? botXpPersistence = null)
        {
            // Check Cache
            if (_playerCache.TryGetValue(steamId, out var cachedPlayer))
            {
                if (playerController != null)
                {
                    cachedPlayer.PlayerController = playerController;
                }
                return cachedPlayer;
            }
            
            // Load from Database
            var player = _playerRepository.GetPlayer(steamId);
            if (player == null)
            {
                // Create new player
                player = new UltimatePlayer
                {
                    SteamId = steamId,
                    PlayerController = playerController,
                    HeroLevel = 1,
                    CurrentXp = 0f,
                    XpToNextLevel = 100f
                };
                
                _playerRepository.SavePlayer(player);
            }
            else
            {
                player.PlayerController = playerController;
                // Restore CurrentHero from database
                RestoreCurrentHero(player);
                
                // Handle Bot XP Persistence
                if (isBot && botXpPersistence != null)
                {
                    ApplyBotXpPersistence(player, botXpPersistence);
                }
            }
            
            // Cache
            _playerCache[steamId] = player;
            
            return player;
        }
        
        /// <summary>
        /// Applies Bot XP persistence settings (Reset, Persistent, Max)
        /// </summary>
        private void ApplyBotXpPersistence(UltimatePlayer player, string persistenceMode)
        {
            switch (persistenceMode.ToLower())
            {
                case "reset":
                    // Reset to Level 1 every round
                    player.HeroLevel = 1;
                    player.CurrentXp = 0f;
                    player.XpToNextLevel = Domain.Progression.XpSystem.GetXpForLevel(1);
                    _playerRepository.SavePlayer(player);
                    break;
                    
                case "max":
                    // Set to Max Level (don't overwrite if already higher)
                    int maxLevel = Domain.Progression.LevelSystem.MaxHeroLevel;
                    if (player.HeroLevel < maxLevel)
                    {
                        player.HeroLevel = maxLevel;
                        // Calculate XP needed for max level
                        float totalXp = 0f;
                        for (int i = 1; i < maxLevel; i++)
                        {
                            totalXp += Domain.Progression.XpSystem.GetXpForLevel(i);
                        }
                        player.CurrentXp = totalXp;
                        player.XpToNextLevel = Domain.Progression.XpSystem.GetXpForLevel(maxLevel);
                        _playerRepository.SavePlayer(player);
                    }
                    break;
                    
                case "persistent":
                default:
                    // Keep XP from database (default behavior, do nothing)
                    break;
            }
        }
        
        public UltimatePlayer? GetPlayer(string steamId)
        {
            if (_playerCache.TryGetValue(steamId, out var player))
            {
                return player;
            }
            
            var dbPlayer = _playerRepository.GetPlayer(steamId);
            if (dbPlayer != null)
            {
                // Restore CurrentHero from database
                RestoreCurrentHero(dbPlayer);
                _playerCache[steamId] = dbPlayer;
            }
            
            return dbPlayer;
        }
        
        private void RestoreCurrentHero(UltimatePlayer player)
        {
            if (_heroService == null || player.CurrentHero != null) return;
            
            var heroCoreId = _playerRepository.GetHeroCoreId(player.SteamId);
            if (!string.IsNullOrEmpty(heroCoreId))
            {
                var hero = _heroService.GetHero(heroCoreId);
                if (hero != null)
                {
                    player.CurrentHero = hero;
                    _heroService.SetPlayerHero(player.SteamId, heroCoreId);
                }
            }
        }
        
        public void SavePlayer(UltimatePlayer player)
        {
            _playerRepository.SavePlayer(player);
            _playerCache[player.SteamId] = player;
        }
        
        public void RemovePlayer(string steamId)
        {
            var player = GetPlayer(steamId);
            if (player != null)
            {
                SavePlayer(player); // Save before removing
            }
            
            _playerCache.Remove(steamId);
        }
        
        public void OnPlayerConnect(string steamId, CCSPlayerController player)
        {
            var ultimatePlayer = GetOrCreatePlayer(steamId, player);
            // Initialize player state
        }
        
        public void OnPlayerDisconnect(string steamId)
        {
            RemovePlayer(steamId);
        }
        
        public void OnPlayerSpawn(string steamId, CCSPlayerController player)
        {
            var ultimatePlayer = GetPlayer(steamId);
            if (ultimatePlayer == null) return;
            
            ultimatePlayer.PlayerController = player;
            
            // Apply Talent Modifiers
            if (_talentService != null)
            {
                var modifiers = _talentService.GetTalentModifiers(steamId);
                ultimatePlayer.TalentModifiers = modifiers;
                
                // Apply modifiers to player (movement speed, etc.)
                ApplyTalentModifiers(player, modifiers);
            }
            
            // Activate Hero Passives
            if (ultimatePlayer.CurrentHero != null)
            {
                ultimatePlayer.CurrentHero.OnPlayerSpawn(player);
            }
            
            // Activate Passive Skills
            foreach (var skill in ultimatePlayer.ActiveSkills)
            {
                if (skill is Domain.Skills.IPassiveSkill passiveSkill)
                {
                    passiveSkill.OnPlayerSpawn(player);
                }
            }
        }
        
        private void ApplyTalentModifiers(CCSPlayerController player, Dictionary<string, float> modifiers)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            
            // Movement Speed
            if (modifiers.TryGetValue("movement_speed", out var movementSpeed))
            {
                if (pawn.MovementServices != null)
                {
                    // Note: MoveSpeedFactor doesn't exist in CS2 API
                    // Movement speed is controlled via other means (e.g., buffs, effects)
                    // This is a placeholder - actual implementation may require game-specific mechanics
                }
            }
            
            // Jump Height
            if (modifiers.TryGetValue("jump_height", out var jumpHeight))
            {
                // Apply jump height modifier (CS2 API)
                // Note: CS2 API may not directly support jump height modification
                // This is a placeholder - actual implementation may require game-specific mechanics
                // For now, we track the modifier for potential future use
            }
            
            // Air Control
            if (modifiers.TryGetValue("air_control", out var airControl))
            {
                // Apply air control modifier (CS2 API)
                // Note: CS2 API may not directly support air control modification
                // This is a placeholder - actual implementation may require game-specific mechanics
                // For now, we track the modifier for potential future use
            }
            
            // Model Size Reduction (Rat Passive)
            if (modifiers.TryGetValue("model_size_reduction", out var modelSizeReduction))
            {
                // Calculate scale: 1.0 - reduction (e.g., 0.02 per level = 0.98 at level 1, 0.90 at level 5)
                var scale = 1.0f - modelSizeReduction;
                Application.Helpers.GameHelpers.SetModelScale(player, scale);
            }
            
            // Other modifiers are applied on-demand (damage, recoil, etc.)
        }
    }
}
