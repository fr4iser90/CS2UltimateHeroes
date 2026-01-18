using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Application.Helpers
{
    /// <summary>
    /// Interface für Game Helper Operations (Domain-kompatibel)
    /// </summary>
    public interface IGameHelpers
    {
        void TeleportPlayer(CCSPlayerController player, Vector position);
        void DamagePlayer(CCSPlayerController player, float damage, CCSPlayerController? attacker = null);
        void HealPlayer(CCSPlayerController player, float amount);
        void MakePlayerInvisible(CCSPlayerController player, bool invisible);
        void SpawnParticle(string particleName, Vector position);
        Vector CalculatePositionInFront(CCSPlayerController player, float distance);
        List<CCSPlayerController> GetPlayersInRadius(Vector center, float radius, CCSPlayerController? excludePlayer = null);
        void SetArmor(CCSPlayerController player, int armor);
        void AddArmor(CCSPlayerController player, int armor);
        void SetMovementSpeed(CCSPlayerController player, float speed);
        void ApplyKnockback(CCSPlayerController player, Vector direction, float force);
    }
}
