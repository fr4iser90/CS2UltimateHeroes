using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace UltimateHeroes.Application.Helpers
{
    /// <summary>
    /// Service-Implementierung von IGameHelpers (verwendet statische GameHelpers intern)
    /// Für Dependency Injection in Domain Skills
    /// </summary>
    public class GameHelpersService : IGameHelpers
    {
        public void TeleportPlayer(CCSPlayerController player, Vector position)
        {
            GameHelpers.TeleportPlayer(player, position);
        }

        public void DamagePlayer(CCSPlayerController player, float damage, CCSPlayerController? attacker = null)
        {
            GameHelpers.DamagePlayer(player, (int)damage, attacker);
        }

        public void HealPlayer(CCSPlayerController player, float amount)
        {
            GameHelpers.HealPlayer(player, (int)amount);
        }

        public void MakePlayerInvisible(CCSPlayerController player, bool invisible)
        {
            GameHelpers.MakePlayerInvisible(player, invisible);
        }

        public void SpawnParticle(string particleName, Vector position)
        {
            GameHelpers.SpawnParticle(position, particleName);
        }

        public Vector CalculatePositionInFront(CCSPlayerController player, float distance)
        {
            return GameHelpers.CalculatePositionInFront(player, distance);
        }

        public List<CCSPlayerController> GetPlayersInRadius(Vector center, float radius, CCSPlayerController? excludePlayer = null)
        {
            var players = GameHelpers.GetPlayersInRadius(center, radius);
            if (excludePlayer != null)
            {
                players.RemoveAll(p => p == excludePlayer);
            }
            return players;
        }

        public void SetArmor(CCSPlayerController player, int armor)
        {
            GameHelpers.SetArmor(player, armor);
        }

        public void AddArmor(CCSPlayerController player, int armor)
        {
            GameHelpers.AddArmor(player, armor);
        }

        public void SetMovementSpeed(CCSPlayerController player, float speed)
        {
            GameHelpers.SetMovementSpeed(player, speed);
        }

        public void ApplyKnockback(CCSPlayerController player, Vector direction, float force)
        {
            GameHelpers.ApplyKnockback(player, direction, force);
        }
    }
}
