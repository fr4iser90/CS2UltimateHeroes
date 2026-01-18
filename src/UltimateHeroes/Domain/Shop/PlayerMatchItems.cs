using System;
using System.Collections.Generic;

namespace UltimateHeroes.Domain.Shop
{
    /// <summary>
    /// Verwaltet temporäre Match-Items eines Spielers
    /// </summary>
    public class PlayerMatchItems
    {
        public string SteamId { get; set; } = string.Empty;
        public Dictionary<string, ActiveItem> ActiveItems { get; set; } = new(); // item_id -> ActiveItem
        public int CurrentMoney { get; set; } = 800; // Start-Geld (wie CS2)
        
        /// <summary>
        /// Prüft ob ein Item aktiv ist
        /// </summary>
        public bool HasActiveItem(string itemId)
        {
            return ActiveItems.ContainsKey(itemId) && !ActiveItems[itemId].IsExpired();
        }
        
        /// <summary>
        /// Entfernt abgelaufene Items
        /// </summary>
        public void CleanupExpiredItems()
        {
            var expired = new List<string>();
            foreach (var kvp in ActiveItems)
            {
                if (kvp.Value.IsExpired())
                {
                    expired.Add(kvp.Key);
                }
            }
            
            foreach (var itemId in expired)
            {
                ActiveItems.Remove(itemId);
            }
        }
        
        /// <summary>
        /// Setzt Items für neues Match zurück
        /// </summary>
        public void ResetForNewMatch()
        {
            ActiveItems.Clear();
            CurrentMoney = 800; // Reset auf Start-Geld
        }
    }
    
    /// <summary>
    /// Ein aktives Item mit verbleibender Dauer
    /// </summary>
    public class ActiveItem
    {
        public string ItemId { get; set; } = string.Empty;
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public float DurationSeconds { get; set; } = 0f; // 0 = permanent für dieses Match
        
        public bool IsExpired()
        {
            if (DurationSeconds <= 0) return false; // Permanent für Match
            return (DateTime.UtcNow - PurchasedAt).TotalSeconds >= DurationSeconds;
        }
        
        public float RemainingSeconds()
        {
            if (DurationSeconds <= 0) return float.MaxValue; // Permanent
            var elapsed = (DateTime.UtcNow - PurchasedAt).TotalSeconds;
            return Math.Max(0, DurationSeconds - (float)elapsed);
        }
    }
}
