using System;

namespace UltimateHeroes.Domain.Progression
{
    /// <summary>
    /// XP History Entry
    /// </summary>
    public class XpHistory
    {
        public string SteamId { get; set; } = string.Empty;
        public XpSource Source { get; set; }
        public float Amount { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
