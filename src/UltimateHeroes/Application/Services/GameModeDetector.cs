using System;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Game Mode Detection - Erkennt den aktuellen Spielmodus
    /// </summary>
    public enum GameMode
    {
        Competitive,    // Round-based
        Casual,         // Round-based
        Wingman,        // Round-based
        Deathmatch,     // Time-based
        ArmsRace,       // Time-based
        Unknown
    }
    
    public static class GameModeDetector
    {
        /// <summary>
        /// Erkennt den aktuellen Game Mode
        /// </summary>
        public static GameMode DetectCurrentMode()
        {
            // Prüfe Server-Variablen via ConVar
            // Note: ConVar API not available at compile time - will be tested at runtime
            // For now, skip ConVar-based detection and use map name fallback
            try
            {
                // Try to find ConVar via Reflection (runtime test)
                var conVarType = Type.GetType("CounterStrikeSharp.API.Core.ConVar, CounterStrikeSharp.API");
                if (conVarType != null)
                {
                    var findMethod = conVarType.GetMethod("Find", new[] { typeof(string) });
                    if (findMethod != null && findMethod.IsStatic)
                    {
                        var gamemodeCvar = findMethod.Invoke(null, new object[] { "mp_gamemode" });
                        if (gamemodeCvar != null)
                        {
                            var getValueMethod = gamemodeCvar.GetType().GetMethod("GetPrimitiveValue");
                            if (getValueMethod != null)
                            {
                                var genericMethod = getValueMethod.MakeGenericMethod(typeof(string));
                                var mode = genericMethod.Invoke(gamemodeCvar, null) as string;
                                if (!string.IsNullOrEmpty(mode))
                                {
                                    return ParseGameMode(mode);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fallback to map name
            }
            
            // Prüfe Map-Name (Fallback)
            var mapName = Server.MapName.ToLower();
            if (mapName.Contains("dm_") || mapName.Contains("deathmatch"))
            {
                return GameMode.Deathmatch;
            }
            
            if (mapName.Contains("ar_") || mapName.Contains("armsrace"))
            {
                return GameMode.ArmsRace;
            }
            
            // Default: Competitive (Round-based)
            return GameMode.Competitive;
        }
        
        /// <summary>
        /// Prüft ob der Modus Round-based ist
        /// </summary>
        public static bool IsRoundBased(GameMode mode)
        {
            return mode == GameMode.Competitive || 
                   mode == GameMode.Casual || 
                   mode == GameMode.Wingman;
        }
        
        /// <summary>
        /// Prüft ob der Modus Time-based ist
        /// </summary>
        public static bool IsTimeBased(GameMode mode)
        {
            return mode == GameMode.Deathmatch || 
                   mode == GameMode.ArmsRace;
        }
        
        /// <summary>
        /// Gibt die Round-Dauer für Time-based Modes zurück (in Minuten)
        /// </summary>
        public static float GetTimeBasedRoundDuration(GameMode mode)
        {
            return mode switch
            {
                GameMode.Deathmatch => 10f,  // 10 Minuten = 1 "Round"
                GameMode.ArmsRace => 5f,     // 5 Minuten = 1 "Round"
                _ => 2f                       // Default: 2 Minuten
            };
        }
        
        private static GameMode ParseGameMode(string mode)
        {
            var lower = mode.ToLower();
            
            if (lower.Contains("competitive") || lower == "comp")
                return GameMode.Competitive;
            
            if (lower.Contains("casual") || lower == "cas")
                return GameMode.Casual;
            
            if (lower.Contains("wingman") || lower == "wm")
                return GameMode.Wingman;
            
            if (lower.Contains("deathmatch") || lower.Contains("dm"))
                return GameMode.Deathmatch;
            
            if (lower.Contains("armsrace") || lower.Contains("ar"))
                return GameMode.ArmsRace;
            
            return GameMode.Unknown;
        }
    }
}
