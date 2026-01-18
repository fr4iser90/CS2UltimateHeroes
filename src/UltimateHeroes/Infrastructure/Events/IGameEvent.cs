using System;

namespace UltimateHeroes.Infrastructure.Events
{
    /// <summary>
    /// Base Interface für alle Game Events
    /// </summary>
    public interface IGameEvent
    {
        string EventName { get; }
        DateTime Timestamp { get; }
    }
}
