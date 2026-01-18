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
        
        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }
        
        public void SetTalentService(ITalentService talentService)
        {
            _talentService = talentService;
        }
        
        public UltimatePlayer GetOrCreatePlayer(string steamId, CCSPlayerController? playerController = null)
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
            }
            
            // Cache
            _playerCache[steamId] = player;
            
            return player;
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
                _playerCache[steamId] = dbPlayer;
            }
            
            return dbPlayer;
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
                    pawn.MovementServices.MoveSpeedFactor = 1.0f + movementSpeed;
                }
            }
            
            // Jump Height
            if (modifiers.TryGetValue("jump_height", out var jumpHeight))
            {
                // TODO: Apply jump height modifier
                // This might require game-specific implementation
            }
            
            // Air Control
            if (modifiers.TryGetValue("air_control", out var airControl))
            {
                // TODO: Apply air control modifier
            }
            
            // Other modifiers are applied on-demand (damage, recoil, etc.)
        }
    }
}
