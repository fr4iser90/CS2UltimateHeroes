using System;
using System.Reflection;
using CounterStrikeSharp.API.Core;

namespace UltimateHeroes.Application.Helpers
{
    /// <summary>
    /// Helper für Admin-Berechtigungsprüfungen
    /// Verwendet Reflection, falls AdminManager nicht verfügbar ist
    /// </summary>
    public static class AdminHelper
    {
        private static bool? _adminManagerAvailable = null;
        private static MethodInfo? _playerHasPermissionsMethod = null;
        
        /// <summary>
        /// Prüft ob AdminManager verfügbar ist
        /// </summary>
        private static bool IsAdminManagerAvailable()
        {
            if (_adminManagerAvailable.HasValue)
                return _adminManagerAvailable.Value;
            
            try
            {
                var adminManagerType = Type.GetType("CounterStrikeSharp.API.Modules.Admin.AdminManager, CounterStrikeSharp.API");
                if (adminManagerType != null)
                {
                    _playerHasPermissionsMethod = adminManagerType.GetMethod("PlayerHasPermissions", 
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new[] { typeof(CCSPlayerController), typeof(string) },
                        null);
                    
                    _adminManagerAvailable = _playerHasPermissionsMethod != null;
                }
                else
                {
                    _adminManagerAvailable = false;
                }
            }
            catch
            {
                _adminManagerAvailable = false;
            }
            
            return _adminManagerAvailable.Value;
        }
        
        /// <summary>
        /// Prüft ob ein Spieler Admin ist (hat @css/root oder @css/generic Flag)
        /// </summary>
        public static bool IsAdmin(CCSPlayerController? player)
        {
            if (player == null || !player.IsValid) return false;
            
            // Versuche AdminManager zu verwenden
            if (IsAdminManagerAvailable() && _playerHasPermissionsMethod != null)
            {
                try
                {
                    var hasRoot = (bool)_playerHasPermissionsMethod.Invoke(null, new object[] { player, "@css/root" })!;
                    var hasGeneric = (bool)_playerHasPermissionsMethod.Invoke(null, new object[] { player, "@css/generic" })!;
                    return hasRoot || hasGeneric;
                }
                catch
                {
                    // Fallback: AdminManager nicht verfügbar
                }
            }
            
            // Fallback: Keine Admin-Prüfung möglich, erlaube alle (für Testing)
            // In Production sollte hier false zurückgegeben werden
            Console.WriteLine("[AdminHelper] AdminManager nicht verfügbar - Admin-Prüfung deaktiviert");
            return false; // Sicherheitshalber: Keine Admin-Rechte ohne AdminManager
        }
        
        /// <summary>
        /// Prüft ob ein Spieler ein spezifisches Admin-Flag hat
        /// </summary>
        public static bool HasPermission(CCSPlayerController? player, string permission)
        {
            if (player == null || !player.IsValid) return false;
            
            if (IsAdminManagerAvailable() && _playerHasPermissionsMethod != null)
            {
                try
                {
                    return (bool)_playerHasPermissionsMethod.Invoke(null, new object[] { player, permission })!;
                }
                catch
                {
                    // Fallback
                }
            }
            
            return false;
        }
    }
}
