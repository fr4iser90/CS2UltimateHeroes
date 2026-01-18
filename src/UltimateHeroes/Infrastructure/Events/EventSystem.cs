using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimateHeroes.Infrastructure.Events
{
    /// <summary>
    /// Event System für Game Events
    /// </summary>
    public class EventSystem
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();
        
        /// <summary>
        /// Registriert einen Event Handler
        /// </summary>
        public void RegisterHandler<T>(IEventHandler<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<object>();
            }
            
            _handlers[eventType].Add(handler);
        }
        
        /// <summary>
        /// Dispatched ein Event an alle registrierten Handler
        /// </summary>
        public void Dispatch<T>(T eventData) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (!_handlers.ContainsKey(eventType)) return;
            
            foreach (var handler in _handlers[eventType])
            {
                if (handler is IEventHandler<T> typedHandler)
                {
                    try
                    {
                        typedHandler.Handle(eventData);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't break other handlers
                        Console.WriteLine($"[EventSystem] Error in handler for {eventType.Name}: {ex.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Entfernt alle Handler für einen Event-Typ
        /// </summary>
        public void UnregisterHandlers<T>() where T : IGameEvent
        {
            var eventType = typeof(T);
            _handlers.Remove(eventType);
        }
        
        /// <summary>
        /// Entfernt alle Handler
        /// </summary>
        public void Clear()
        {
            _handlers.Clear();
        }
    }
}
