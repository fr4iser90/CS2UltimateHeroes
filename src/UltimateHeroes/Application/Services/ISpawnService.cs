using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für Entity Spawning (Sentry, Drone, etc.)
    /// </summary>
    public interface ISpawnService
    {
        // Entity Management
        string SpawnSentry(string ownerSteamId, Vector position, float range, int damage, float duration);
        string SpawnDrone(string ownerSteamId, Vector position, float range, float duration);
        void RemoveEntity(string entityId);
        void RemoveAllEntities(string ownerSteamId);
        void RemoveAllEntities(); // Remove all entities (on map change)
        
        // Entity Queries
        bool HasEntity(string entityId);
        List<string> GetPlayerEntities(string ownerSteamId);
        
        // Tick (should be called regularly)
        void TickEntities();
    }
}
