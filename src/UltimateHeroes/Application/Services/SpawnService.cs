using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Entity Spawning (Sentry, Drone, etc.)
    /// </summary>
    public class SpawnService : ISpawnService
    {
        private readonly Dictionary<string, SpawnedEntity> _entities = new(); // entityId -> entity
        private readonly Dictionary<string, List<string>> _playerEntities = new(); // ownerSteamId -> entityIds
        
        private class SpawnedEntity
        {
            public string Id { get; set; } = string.Empty;
            public string OwnerSteamId { get; set; } = string.Empty;
            public EntityType Type { get; set; }
            public Vector Position { get; set; }
            public float Range { get; set; }
            public int Damage { get; set; }
            public DateTime SpawnedAt { get; set; }
            public float Duration { get; set; }
            public DateTime LastAttackTime { get; set; }
            public float AttackCooldown { get; set; } = 1f; // Attack every 1 second
        }
        
        private enum EntityType
        {
            Sentry,
            Drone
        }
        
        public string SpawnSentry(string ownerSteamId, Vector position, float range, int damage, float duration)
        {
            var entityId = $"sentry_{Guid.NewGuid()}";
            
            var entity = new SpawnedEntity
            {
                Id = entityId,
                OwnerSteamId = ownerSteamId,
                Type = EntityType.Sentry,
                Position = position,
                Range = range,
                Damage = damage,
                SpawnedAt = DateTime.UtcNow,
                Duration = duration,
                LastAttackTime = DateTime.UtcNow,
                AttackCooldown = 1f
            };
            
            _entities[entityId] = entity;
            
            if (!_playerEntities.ContainsKey(ownerSteamId))
            {
                _playerEntities[ownerSteamId] = new List<string>();
            }
            _playerEntities[ownerSteamId].Add(entityId);
            
            // Spawn visual indicator
            GameHelper.SpawnParticle(position, "particles/ui/ui_electric_exp_glow.vpcf", duration);
            
            return entityId;
        }
        
        public string SpawnDrone(string ownerSteamId, Vector position, float range, float duration)
        {
            var entityId = $"drone_{Guid.NewGuid()}";
            
            var entity = new SpawnedEntity
            {
                Id = entityId,
                OwnerSteamId = ownerSteamId,
                Type = EntityType.Drone,
                Position = position,
                Range = range,
                SpawnedAt = DateTime.UtcNow,
                Duration = duration
            };
            
            _entities[entityId] = entity;
            
            if (!_playerEntities.ContainsKey(ownerSteamId))
            {
                _playerEntities[ownerSteamId] = new List<string>();
            }
            _playerEntities[ownerSteamId].Add(entityId);
            
            // Spawn visual indicator
            GameHelper.SpawnParticle(position, "particles/ui/ui_electric_exp_glow.vpcf", duration);
            
            return entityId;
        }
        
        public void RemoveEntity(string entityId)
        {
            if (!_entities.TryGetValue(entityId, out var entity)) return;
            
            var ownerSteamId = entity.OwnerSteamId;
            _entities.Remove(entityId);
            
            if (_playerEntities.ContainsKey(ownerSteamId))
            {
                _playerEntities[ownerSteamId].Remove(entityId);
                if (_playerEntities[ownerSteamId].Count == 0)
                {
                    _playerEntities.Remove(ownerSteamId);
                }
            }
        }
        
        public void RemoveAllEntities(string ownerSteamId)
        {
            if (!_playerEntities.ContainsKey(ownerSteamId)) return;
            
            var entityIds = _playerEntities[ownerSteamId].ToList();
            foreach (var entityId in entityIds)
            {
                RemoveEntity(entityId);
            }
        }
        
        public void RemoveAllEntities()
        {
            _entities.Clear();
            _playerEntities.Clear();
        }
        
        public bool HasEntity(string entityId)
        {
            return _entities.ContainsKey(entityId);
        }
        
        public List<string> GetPlayerEntities(string ownerSteamId)
        {
            return _playerEntities.GetValueOrDefault(ownerSteamId, new List<string>());
        }
        
        public void TickEntities()
        {
            var now = DateTime.UtcNow;
            var entitiesToRemove = new List<string>();
            
            foreach (var entity in _entities.Values.ToList())
            {
                // Check if expired
                var elapsed = (now - entity.SpawnedAt).TotalSeconds;
                if (elapsed >= entity.Duration)
                {
                    entitiesToRemove.Add(entity.Id);
                    continue;
                }
                
                // Handle Sentry attacks
                if (entity.Type == EntityType.Sentry)
                {
                    var timeSinceLastAttack = (now - entity.LastAttackTime).TotalSeconds;
                    if (timeSinceLastAttack >= entity.AttackCooldown)
                    {
                        AttackWithSentry(entity);
                        entity.LastAttackTime = now;
                    }
                }
                
                // Handle Drone reveals
                if (entity.Type == EntityType.Drone)
                {
                    RevealWithDrone(entity);
                }
            }
            
            // Remove expired entities
            foreach (var entityId in entitiesToRemove)
            {
                RemoveEntity(entityId);
            }
        }
        
        private void AttackWithSentry(SpawnedEntity entity)
        {
            // Find enemies in range
            var enemies = GameHelper.GetPlayersInRadius(entity.Position, entity.Range);
            
            // Find closest enemy (not owner)
            CCSPlayerController? target = null;
            float closestDistance = float.MaxValue;
            
            foreach (var enemy in enemies)
            {
                if (enemy == null || !enemy.IsValid || enemy.AuthorizedSteamID == null) continue;
                var enemySteamId = enemy.AuthorizedSteamID.SteamId64.ToString();
                if (enemySteamId == entity.OwnerSteamId) continue; // Don't attack owner
                
                var enemyPawn = enemy.PlayerPawn.Value;
                if (enemyPawn?.AbsOrigin == null) continue;
                
                var distance = Vector.Distance(entity.Position, enemyPawn.AbsOrigin);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = enemy;
                }
            }
            
            if (target != null && target.IsValid)
            {
                // Damage target
                var owner = Utilities.GetPlayers().FirstOrDefault(p => p.SteamID.ToString() == entity.OwnerSteamId);
                GameHelper.DamagePlayer(target, entity.Damage, owner);
                
                // Spawn attack particle
                var targetPos = target.PlayerPawn.Value?.AbsOrigin;
                if (targetPos != null)
                {
                    GameHelper.SpawnParticle(targetPos, "particles/weapons_fx/explosion_fireball.vpcf", 0.5f);
                }
            }
        }
        
        private void RevealWithDrone(SpawnedEntity entity)
        {
            // Find enemies in range and reveal them
            var enemies = GameHelper.GetPlayersInRadius(entity.Position, entity.Range);
            
            foreach (var enemy in enemies)
            {
                if (enemy == null || !enemy.IsValid || enemy.AuthorizedSteamID == null) continue;
                var enemySteamId = enemy.AuthorizedSteamID.SteamId64.ToString();
                if (enemySteamId == entity.OwnerSteamId) continue; // Don't reveal owner
                
                // Make enemy visible
                GameHelper.MakePlayerInvisible(enemy, false);
            }
        }
    }
}
