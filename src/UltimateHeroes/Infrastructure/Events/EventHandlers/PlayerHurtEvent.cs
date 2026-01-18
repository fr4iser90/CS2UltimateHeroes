using System;
using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Infrastructure.Events.EventHandlers
{
    /// <summary>
    /// Event: Spieler wurde verletzt
    /// </summary>
    public class PlayerHurtEvent : IGameEvent
    {
        public string EventName => "PlayerHurt";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string AttackerSteamId { get; set; } = string.Empty;
        public string VictimSteamId { get; set; } = string.Empty;
        public CCSPlayerController? Attacker { get; set; }
        public CCSPlayerController? Player { get; set; }
        public int Damage { get; set; }
    }
}
