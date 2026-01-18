using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Infrastructure.Effects
{
    /// <summary>
    /// Verwaltet temporäre Effects auf Spielern (generisch via Handler Registry)
    /// </summary>
    public class EffectManager
    {
        private readonly Dictionary<string, List<IEffect>> _playerEffects = new(); // steamid -> effects
        private readonly EffectHandlerRegistry _handlerRegistry;
        
        public EffectManager()
        {
            _handlerRegistry = new EffectHandlerRegistry();
            RegisterHandlersViaReflection();
        }
        
        /// <summary>
        /// Automatically registers all IEffectHandler implementations via Reflection
        /// </summary>
        private void RegisterHandlersViaReflection()
        {
            var handlerType = typeof(IEffectHandler);
            var handlerTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => handlerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            
            foreach (var type in handlerTypes)
            {
                try
                {
                    var handler = (IEffectHandler)Activator.CreateInstance(type)!;
                    _handlerRegistry.RegisterHandler(handler);
                }
                catch (Exception ex)
                {
                    // Log error but continue
                    Console.WriteLine($"[EffectManager] Failed to register handler {type.Name}: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Register a custom effect handler (for extensibility)
        /// </summary>
        public void RegisterHandler(IEffectHandler handler)
        {
            _handlerRegistry.RegisterHandler(handler);
        }
        
        /// <summary>
        /// Wendet einen Effect auf einen Spieler an
        /// </summary>
        public void ApplyEffect(string steamId, IEffect effect)
        {
            if (!_playerEffects.ContainsKey(steamId))
            {
                _playerEffects[steamId] = new List<IEffect>();
            }
            
            // Check if effect already exists (stacking logic)
            var existingEffect = _playerEffects[steamId].FirstOrDefault(e => e.Id == effect.Id);
            if (existingEffect != null)
            {
                // Stack or refresh based on effect type
                HandleEffectStacking(existingEffect, effect);
                return;
            }
            
            // Apply new effect
            effect.AppliedAt = DateTime.UtcNow;
            _playerEffects[steamId].Add(effect);
            
            var player = GetPlayer(steamId);
            if (player != null && player.IsValid)
            {
                // Use handler if available, otherwise fallback to direct call
                var handler = _handlerRegistry.GetHandler(effect.Id);
                if (handler != null)
                {
                    handler.OnApply(player, effect);
                }
                else
                {
                    // Fallback for effects without handlers
                    effect.OnApply(player);
                }
            }
        }
        
        /// <summary>
        /// Entfernt einen Effect von einem Spieler
        /// </summary>
        public void RemoveEffect(string steamId, string effectId)
        {
            if (!_playerEffects.ContainsKey(steamId)) return;
            
            var effect = _playerEffects[steamId].FirstOrDefault(e => e.Id == effectId);
            if (effect != null)
            {
                var player = GetPlayer(steamId);
                if (player != null && player.IsValid)
                {
                    // Use handler if available, otherwise fallback to direct call
                    var handler = _handlerRegistry.GetHandler(effect.Id);
                    if (handler != null)
                    {
                        handler.OnRemove(player, effect);
                    }
                    else
                    {
                        // Fallback for effects without handlers
                        effect.OnRemove(player);
                    }
                }
                
                _playerEffects[steamId].Remove(effect);
                
                // Clean up empty lists
                if (_playerEffects[steamId].Count == 0)
                {
                    _playerEffects.Remove(steamId);
                }
            }
        }
        
        /// <summary>
        /// Entfernt alle Effects von einem Spieler
        /// </summary>
        public void RemoveAllEffects(string steamId)
        {
            if (!_playerEffects.ContainsKey(steamId)) return;
            
            var effects = _playerEffects[steamId].ToList();
            var player = GetPlayer(steamId);
            
            foreach (var effect in effects)
            {
                if (player != null && player.IsValid)
                {
                    // Use handler if available, otherwise fallback to direct call
                    var handler = _handlerRegistry.GetHandler(effect.Id);
                    if (handler != null)
                    {
                        handler.OnRemove(player, effect);
                    }
                    else
                    {
                        // Fallback for effects without handlers
                        effect.OnRemove(player);
                    }
                }
            }
            
            _playerEffects.Remove(steamId);
        }
        
        /// <summary>
        /// Prüft ob ein Spieler einen bestimmten Effect hat
        /// </summary>
        public bool HasEffect(string steamId, string effectId)
        {
            if (!_playerEffects.ContainsKey(steamId)) return false;
            return _playerEffects[steamId].Any(e => e.Id == effectId && !e.IsExpired());
        }
        
        /// <summary>
        /// Gibt alle aktiven Effects eines Spielers zurück
        /// </summary>
        public List<IEffect> GetPlayerEffects(string steamId)
        {
            if (!_playerEffects.ContainsKey(steamId)) return new List<IEffect>();
            return _playerEffects[steamId].Where(e => !e.IsExpired()).ToList();
        }
        
        /// <summary>
        /// Tickt alle Effects (sollte regelmäßig aufgerufen werden)
        /// </summary>
        public void TickEffects()
        {
            foreach (var playerEffects in _playerEffects.ToList())
            {
                var steamId = playerEffects.Key;
                var effects = playerEffects.Value.ToList(); // Copy to avoid modification during iteration
                
                foreach (var effect in effects)
                {
                    if (effect.IsExpired())
                    {
                        RemoveEffect(steamId, effect.Id);
                    }
                    else
                    {
                        var player = GetPlayer(steamId);
                        if (player != null && player.IsValid)
                        {
                            // Use handler if available, otherwise fallback to direct call
                            var handler = _handlerRegistry.GetHandler(effect.Id);
                            if (handler != null)
                            {
                                handler.OnTick(player, effect);
                            }
                            else
                            {
                                // Fallback for effects without handlers
                                effect.OnTick(player);
                            }
                        }
                    }
                }
            }
        }
        
        private void HandleEffectStacking(IEffect existing, IEffect newEffect)
        {
            // Default: Refresh duration
            existing.AppliedAt = DateTime.UtcNow;
        }
        
        private CCSPlayerController? GetPlayer(string steamId)
        {
            return Utilities.GetPlayers().FirstOrDefault(p => p.SteamID.ToString() == steamId);
        }
    }
}
