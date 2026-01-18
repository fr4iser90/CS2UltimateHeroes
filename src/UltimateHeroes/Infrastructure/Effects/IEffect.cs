using System;
using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Infrastructure.Effects
{
    /// <summary>
    /// Interface für temporäre Effects auf Spielern
    /// </summary>
    public interface IEffect
    {
        string Id { get; }
        string DisplayName { get; }
        float Duration { get; }
        DateTime AppliedAt { get; set; }
        
        void OnApply(CCSPlayerController player);
        void OnTick(CCSPlayerController player);
        void OnRemove(CCSPlayerController player);
        bool IsExpired();
    }
}
