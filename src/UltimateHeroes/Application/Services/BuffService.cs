using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Domain.Buffs;
using UltimateHeroes.Infrastructure.Buffs;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Buff Management (generisch via Handler Registry)
    /// </summary>
    public class BuffService : IBuffService
    {
        private readonly Dictionary<string, List<Buff>> _playerBuffs = new(); // steamid -> buffs
        private readonly Dictionary<string, string> _tauntTargets = new(); // taunted_steamid -> taunter_steamid
        private readonly BuffHandlerRegistry _handlerRegistry;
        
        public BuffService()
        {
            _handlerRegistry = new BuffHandlerRegistry();
            
            // Register default handlers
            RegisterDefaultHandlers();
        }
        
        private void RegisterDefaultHandlers()
        {
            // Auto-register all handlers via Reflection
            RegisterHandlersViaReflection();
        }
        
        /// <summary>
        /// Automatically registers all IBuffHandler implementations via Reflection
        /// </summary>
        private void RegisterHandlersViaReflection()
        {
            var handlerType = typeof(IBuffHandler);
            var handlerTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => handlerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            
            foreach (var type in handlerTypes)
            {
                try
                {
                    var handler = (IBuffHandler)Activator.CreateInstance(type)!;
                    _handlerRegistry.RegisterHandler(handler);
                }
                catch (Exception ex)
                {
                    // Log error but continue
                    Console.WriteLine($"[BuffService] Failed to register handler {type.Name}: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Register a custom buff handler (for extensibility)
        /// </summary>
        public void RegisterHandler(IBuffHandler handler)
        {
            _handlerRegistry.RegisterHandler(handler);
        }
        
        public void ApplyBuff(string steamId, Buff buff)
        {
            if (!_playerBuffs.ContainsKey(steamId))
            {
                _playerBuffs[steamId] = new List<Buff>();
            }
            
            var existingBuff = _playerBuffs[steamId].FirstOrDefault(b => b.Id == buff.Id);
            if (existingBuff != null)
            {
                HandleBuffStacking(existingBuff, buff);
                return;
            }
            
            buff.AppliedAt = DateTime.UtcNow;
            _playerBuffs[steamId].Add(buff);
            
            // Apply immediate effects via handler (generisch)
            ApplyBuffEffects(steamId, buff);
        }
        
        public void RemoveBuff(string steamId, string buffId)
        {
            if (!_playerBuffs.ContainsKey(steamId)) return;
            
            var buff = _playerBuffs[steamId].FirstOrDefault(b => b.Id == buffId);
            if (buff != null)
            {
                RemoveBuffEffects(steamId, buff);
                _playerBuffs[steamId].Remove(buff);
                
                // Clean up taunt tracking
                if (buff.Type == BuffType.Taunt)
                {
                    _tauntTargets.Remove(steamId);
                }
            }
            
            if (_playerBuffs[steamId].Count == 0)
            {
                _playerBuffs.Remove(steamId);
            }
        }
        
        public void RemoveAllBuffs(string steamId)
        {
            if (!_playerBuffs.ContainsKey(steamId)) return;
            
            var buffs = _playerBuffs[steamId].ToList();
            foreach (var buff in buffs)
            {
                RemoveBuffEffects(steamId, buff);
            }
            
            _playerBuffs.Remove(steamId);
            _tauntTargets.Remove(steamId);
        }
        
        public bool HasBuff(string steamId, string buffId)
        {
            if (!_playerBuffs.ContainsKey(steamId)) return false;
            return _playerBuffs[steamId].Any(b => b.Id == buffId && !b.IsExpired());
        }
        
        public List<Buff> GetPlayerBuffs(string steamId)
        {
            if (!_playerBuffs.ContainsKey(steamId)) return new List<Buff>();
            return _playerBuffs[steamId].Where(b => !b.IsExpired()).ToList();
        }
        
        public List<Buff> GetPlayerBuffs(string steamId, BuffType type)
        {
            return GetPlayerBuffs(steamId).Where(b => b.Type == type).ToList();
        }
        
        public float GetBuffValue(string steamId, BuffType type, string parameterKey, float defaultValue = 0f)
        {
            var buffs = GetPlayerBuffs(steamId, type);
            if (buffs.Count == 0) return defaultValue;
            
            // Return first buff's value (or max if multiple)
            return buffs.First().Parameters.GetValueOrDefault(parameterKey, defaultValue);
        }
        
        public float GetTotalBuffValue(string steamId, BuffType type, string parameterKey, float defaultValue = 0f)
        {
            var buffs = GetPlayerBuffs(steamId, type);
            if (buffs.Count == 0) return defaultValue;
            
            // Sum all buff values of this type
            return buffs.Sum(b => b.Parameters.GetValueOrDefault(parameterKey, defaultValue));
        }
        
        // Helper Methods (for convenience, but Skills should create Buff objects directly)
        public void ApplyDamageBoost(string steamId, float multiplier, float duration)
        {
            var buff = new Buff
            {
                Id = $"damage_boost_{Guid.NewGuid()}",
                DisplayName = "Damage Boost",
                Type = BuffType.DamageBoost,
                Duration = duration,
                Parameters = new Dictionary<string, float> { { "multiplier", multiplier } }
            };
            ApplyBuff(steamId, buff);
        }
        
        public void ApplySpeedBoost(string steamId, float multiplier, float duration)
        {
            var buff = new Buff
            {
                Id = $"speed_boost_{Guid.NewGuid()}",
                DisplayName = "Speed Boost",
                Type = BuffType.SpeedBoost,
                Duration = duration,
                Parameters = new Dictionary<string, float> { { "multiplier", multiplier } }
            };
            ApplyBuff(steamId, buff);
            
            // Apply immediately
            var player = GetPlayer(steamId);
            if (player != null && player.IsValid)
            {
                GameHelpers.SetMovementSpeed(player, 1.0f + multiplier);
            }
        }
        
        public void ApplyWeaponSpread(string steamId, float spreadMultiplier, float duration)
        {
            var buff = new Buff
            {
                Id = $"weapon_spread_{Guid.NewGuid()}",
                DisplayName = "Weapon Spread",
                Type = BuffType.WeaponSpreadIncrease,
                Duration = duration,
                Parameters = new Dictionary<string, float> { { "multiplier", spreadMultiplier } }
            };
            ApplyBuff(steamId, buff);
        }
        
        public void ApplyTaunt(string steamId, string taunterSteamId, float duration)
        {
            var buff = new Buff
            {
                Id = $"taunt_{Guid.NewGuid()}",
                DisplayName = "Taunted",
                Type = BuffType.Taunt,
                Duration = duration,
                Parameters = new Dictionary<string, float>
                {
                    { "taunter_steamid", float.Parse(taunterSteamId) }, // Hack: Store as float
                    { "damage_reduction", 0.5f }, // 50% damage reduction if not attacking taunter
                    { "spread_multiplier", 2.0f } // 2x weapon spread
                }
            };
            ApplyBuff(steamId, buff);
            _tauntTargets[steamId] = taunterSteamId;
            
            var player = GetPlayer(steamId);
            if (player != null && player.IsValid)
            {
                player.PrintToChat($" {ChatColors.Red}[Taunt]{ChatColors.Default} You are taunted! Attack the taunter or suffer penalties!");
            }
        }
        
        public void ApplyReveal(string steamId, float duration)
        {
            var buff = new Buff
            {
                Id = $"reveal_{Guid.NewGuid()}",
                DisplayName = "Revealed",
                Type = BuffType.Reveal,
                Duration = duration
            };
            ApplyBuff(steamId, buff);
            
            // Make player visible
            var player = GetPlayer(steamId);
            if (player != null && player.IsValid)
            {
                GameHelpers.MakePlayerInvisible(player, false);
            }
        }
        
        public void ApplyExecutionMark(string steamId, float damageMultiplier, float duration)
        {
            var buff = new Buff
            {
                Id = "execution_mark", // Fixed ID so it refreshes instead of stacking
                DisplayName = "Execution Mark",
                Type = BuffType.ExecutionMark,
                Duration = duration,
                StackingType = BuffStackingType.Refresh, // Refresh duration instead of stacking
                Parameters = new Dictionary<string, float> { { "damage_multiplier", damageMultiplier } }
            };
            ApplyBuff(steamId, buff);
        }
        
        public void ApplyShield(string steamId, float damageReduction, float duration)
        {
            var buff = new Buff
            {
                Id = $"shield_{Guid.NewGuid()}",
                DisplayName = "Shield",
                Type = BuffType.Shield,
                Duration = duration,
                Parameters = new Dictionary<string, float> { { "damage_reduction", damageReduction } }
            };
            ApplyBuff(steamId, buff);
        }
        
        public void ApplyInfiniteAmmo(string steamId, float duration)
        {
            var buff = new Buff
            {
                Id = "infinite_ammo", // Fixed ID so it refreshes instead of stacking
                DisplayName = "Infinite Ammo",
                Type = BuffType.InfiniteAmmo,
                Duration = duration,
                StackingType = BuffStackingType.Refresh // Refresh duration instead of stacking
            };
            ApplyBuff(steamId, buff);
        }
        
        public void TickBuffs()
        {
            foreach (var playerBuffs in _playerBuffs.ToList())
            {
                var steamId = playerBuffs.Key;
                var buffs = playerBuffs.Value.ToList();
                
                foreach (var buff in buffs)
                {
                    if (buff.IsExpired())
                    {
                        RemoveBuff(steamId, buff.Id);
                    }
                    else
                    {
                        // Apply tick effects (e.g., speed boost, infinite ammo)
                        TickBuffEffects(steamId, buff);
                    }
                }
            }
        }
        
        // Helper Methods
        private void HandleBuffStacking(Buff existing, Buff newBuff)
        {
            switch (existing.StackingType)
            {
                case BuffStackingType.Refresh:
                    existing.AppliedAt = DateTime.UtcNow;
                    existing.Duration = newBuff.Duration;
                    break;
                case BuffStackingType.Additive:
                    existing.CurrentStacks = Math.Min(existing.CurrentStacks + 1, existing.MaxStacks);
                    existing.AppliedAt = DateTime.UtcNow;
                    // Add parameter values
                    foreach (var param in newBuff.Parameters)
                    {
                        if (existing.Parameters.ContainsKey(param.Key))
                        {
                            existing.Parameters[param.Key] += param.Value;
                        }
                        else
                        {
                            existing.Parameters[param.Key] = param.Value;
                        }
                    }
                    break;
                case BuffStackingType.Multiplicative:
                    existing.AppliedAt = DateTime.UtcNow;
                    // Multiply parameter values
                    foreach (var param in newBuff.Parameters)
                    {
                        if (existing.Parameters.ContainsKey(param.Key))
                        {
                            existing.Parameters[param.Key] *= param.Value;
                        }
                        else
                        {
                            existing.Parameters[param.Key] = param.Value;
                        }
                    }
                    break;
                case BuffStackingType.Max:
                    existing.AppliedAt = DateTime.UtcNow;
                    // Take maximum value
                    foreach (var param in newBuff.Parameters)
                    {
                        if (existing.Parameters.ContainsKey(param.Key))
                        {
                            existing.Parameters[param.Key] = Math.Max(existing.Parameters[param.Key], param.Value);
                        }
                        else
                        {
                            existing.Parameters[param.Key] = param.Value;
                        }
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Apply buff effects via handler (generisch - kein switch case mehr)
        /// </summary>
        private void ApplyBuffEffects(string steamId, Buff buff)
        {
            var player = GetPlayer(steamId);
            if (player == null || !player.IsValid) return;
            
            // Get handler for this buff type
            var handler = _handlerRegistry.GetHandler(buff.Type);
            if (handler != null)
            {
                handler.OnApply(player, buff);
            }
            // If no handler registered, buff is passive (e.g., DamageBoost, Shield - handled in DamagePlayer)
        }
        
        /// <summary>
        /// Remove buff effects via handler (generisch)
        /// </summary>
        private void RemoveBuffEffects(string steamId, Buff buff)
        {
            var player = GetPlayer(steamId);
            if (player == null || !player.IsValid) return;
            
            // Get handler for this buff type
            var handler = _handlerRegistry.GetHandler(buff.Type);
            handler?.OnRemove(player, buff);
        }
        
        /// <summary>
        /// Tick buff effects via handler (generisch)
        /// </summary>
        private void TickBuffEffects(string steamId, Buff buff)
        {
            var player = GetPlayer(steamId);
            if (player == null || !player.IsValid) return;
            
            // Get handler for this buff type
            var handler = _handlerRegistry.GetHandler(buff.Type);
            handler?.OnTick(player, buff);
        }
        
        public bool IsTaunted(string steamId)
        {
            return _tauntTargets.ContainsKey(steamId);
        }
        
        public string? GetTaunter(string steamId)
        {
            return _tauntTargets.GetValueOrDefault(steamId);
        }
        
        private CCSPlayerController? GetPlayer(string steamId)
        {
            return Utilities.GetPlayers().FirstOrDefault(p => p.SteamID.ToString() == steamId);
        }
    }
}
