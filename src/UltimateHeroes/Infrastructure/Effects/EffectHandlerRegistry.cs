using System.Collections.Generic;
using System.Linq;

namespace UltimateHeroes.Infrastructure.Effects
{
    /// <summary>
    /// Registry für Effect Handlers (Strategy Pattern)
    /// </summary>
    public class EffectHandlerRegistry
    {
        private readonly Dictionary<string, IEffectHandler> _handlers = new(); // effectId -> handler
        
        public void RegisterHandler(IEffectHandler handler)
        {
            _handlers[handler.HandlesEffectId] = handler;
        }
        
        public IEffectHandler? GetHandler(string effectId)
        {
            return _handlers.GetValueOrDefault(effectId);
        }
        
        public bool HasHandler(string effectId)
        {
            return _handlers.ContainsKey(effectId);
        }
    }
}
