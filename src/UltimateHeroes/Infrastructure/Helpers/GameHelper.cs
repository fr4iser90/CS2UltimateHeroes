using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace UltimateHeroes.Infrastructure.Helpers
{
    /// <summary>
    /// Static Helper für IGameHelpers (für Domain Skills - Dependency Injection via Service Locator)
    /// Verwendet Service Locator Pattern für Domain-Kompatibilität
    /// </summary>
    public static class GameHelper
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
        public static void TeleportPlayer(CCSPlayerController player, Vector position)
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
        
        public static void SpawnParticle(Vector position, string particleName, float duration = 5f)
        {
            _gameHelpers?.SpawnParticle(particleName, position);
        }
        
        public static Vector CalculatePositionInFront(CCSPlayerController player, float distance, float heightOffset = 0)
        {
            var result = _gameHelpers?.CalculatePositionInFront(player, distance);
            if (result == null) return Vector.Zero;
            
            // Apply height offset
            if (heightOffset != 0)
            {
                return new Vector(result.X, result.Y, result.Z + heightOffset);
            }
            return result;
        }
        
        public static List<CCSPlayerController> GetPlayersInRadius(Vector position, float radius)
        {
            return _gameHelpers?.GetPlayersInRadius(position, radius) ?? new List<CCSPlayerController>();
        }
        
        public static void SetArmor(CCSPlayerController player, int armor)
        {
            _gameHelpers?.SetArmor(player, armor);
        }
        
        public static void AddArmor(CCSPlayerController player, int armor)
        {
            _gameHelpers?.AddArmor(player, armor);
        }
        
        public static void MakePlayerInvisible(CCSPlayerController player, bool invisible)
        {
            _gameHelpers?.MakePlayerInvisible(player, invisible);
        }
        
        public static void SetMovementSpeed(CCSPlayerController player, float speed)
        {
            _gameHelpers?.SetMovementSpeed(player, speed);
        }
        
        public static void ApplyKnockback(CCSPlayerController player, Vector direction, float force)
        {
            _gameHelpers?.ApplyKnockback(player, direction, force);
        }
    }
}
