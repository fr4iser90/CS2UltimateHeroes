using UltimateHeroes.Application.Helpers;

namespace UltimateHeroes.Infrastructure.Helpers
{
    /// <summary>
    /// Static Helper für IGameHelpers (für Domain Skills - Dependency Injection via Service Locator)
    /// Verwendet Service Locator Pattern für Domain-Kompatibilität
    /// </summary>
    public static class GameHelpersHelper
    {
        private static IGameHelpers? _gameHelpers;
        
        public static void SetGameHelpers(IGameHelpers gameHelpers)
        {
            _gameHelpers = gameHelpers;
        }
        
        public static IGameHelpers? GetGameHelpers()
        {
            return _gameHelpers;
        }
        
        /// <summary>
        /// Convenience-Methoden für statische Aufrufe (delegiert an IGameHelpers)
        /// </summary>
        public static void TeleportPlayer(CCSPlayerController player, CounterStrikeSharp.API.Modules.Utils.Vector position)
        {
            _gameHelpers?.TeleportPlayer(player, position);
        }
        
        public static void DamagePlayer(CCSPlayerController player, float damage, CCSPlayerController? attacker = null)
        {
            _gameHelpers?.DamagePlayer(player, damage, attacker);
        }
        
        public static void HealPlayer(CCSPlayerController player, float amount)
        {
            _gameHelpers?.HealPlayer(player, amount);
        }
        
        public static void SpawnParticle(CounterStrikeSharp.API.Modules.Utils.Vector position, string particleName, float duration = 5f)
        {
            _gameHelpers?.SpawnParticle(particleName, position);
        }
        
        public static CounterStrikeSharp.API.Modules.Utils.Vector CalculatePositionInFront(CCSPlayerController player, float distance, float heightOffset = 0)
        {
            var result = _gameHelpers?.CalculatePositionInFront(player, distance);
            if (result == null) return CounterStrikeSharp.API.Modules.Utils.Vector.Zero;
            
            // Apply height offset
            if (heightOffset != 0)
            {
                return new CounterStrikeSharp.API.Modules.Utils.Vector(result.X, result.Y, result.Z + heightOffset);
            }
            return result;
        }
        
        public static System.Collections.Generic.List<CCSPlayerController> GetPlayersInRadius(CounterStrikeSharp.API.Modules.Utils.Vector position, float radius)
        {
            return _gameHelpers?.GetPlayersInRadius(position, radius) ?? new System.Collections.Generic.List<CCSPlayerController>();
        }
    }
}
