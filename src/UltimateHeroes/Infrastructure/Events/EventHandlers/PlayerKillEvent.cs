using System;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Progression;

namespace UltimateHeroes.Infrastructure.Events.EventHandlers
{
    /// <summary>
    /// Event: Spieler hat einen Kill gemacht
    /// </summary>
    public class PlayerKillEvent : IGameEvent
    {
        public string EventName => "PlayerKill";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string KillerSteamId { get; set; } = string.Empty;
        public string VictimSteamId { get; set; } = string.Empty;
        public CCSPlayerController? Killer { get; set; }
        public CCSPlayerController? Victim { get; set; }
        public bool IsHeadshot { get; set; }
    }
}
