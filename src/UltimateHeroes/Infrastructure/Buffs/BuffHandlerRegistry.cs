using System.Collections.Generic;
using System.Linq;
using UltimateHeroes.Domain.Buffs;

namespace UltimateHeroes.Infrastructure.Buffs
{
    /// <summary>
    /// Registry für Buff Handlers (Strategy Pattern)
    /// </summary>
    public class BuffHandlerRegistry
    {
        private readonly Dictionary<BuffType, IBuffHandler> _handlers = new();
        
        public void RegisterHandler(IBuffHandler handler)
        {
            _handlers[handler.HandlesType] = handler;
        }
        
        public IBuffHandler? GetHandler(BuffType type)
        {
            return _handlers.GetValueOrDefault(type);
        }
        
        public bool HasHandler(BuffType type)
        {
            return _handlers.ContainsKey(type);
        }
    }
}
