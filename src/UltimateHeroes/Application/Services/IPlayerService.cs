using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Players;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service Interface für Player Management
    /// </summary>
    public interface IPlayerService
    {
        UltimatePlayer GetOrCreatePlayer(string steamId, CCSPlayerController? playerController = null);
        UltimatePlayer? GetPlayer(string steamId);
        void SavePlayer(UltimatePlayer player);
        void RemovePlayer(string steamId);
        
        void OnPlayerConnect(string steamId, CCSPlayerController player);
        void OnPlayerDisconnect(string steamId);
        void OnPlayerSpawn(string steamId, CCSPlayerController player);
    }
}
