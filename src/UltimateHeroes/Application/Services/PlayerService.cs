using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Players;
using UltimateHeroes.Infrastructure.Database.Repositories;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Player State Management
    /// </summary>
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly Dictionary<string, UltimatePlayer> _playerCache = new();
        
        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
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
    }
}
