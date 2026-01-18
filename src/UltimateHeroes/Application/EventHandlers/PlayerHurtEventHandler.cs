using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Infrastructure.Events;
using UltimateHeroes.Infrastructure.Events.EventHandlers;

namespace UltimateHeroes.Application.EventHandlers
{
    /// <summary>
    /// Handler für PlayerHurt Events
    /// </summary>
    public class PlayerHurtEventHandler
    {
        private readonly EventSystem _eventSystem;
        
        public PlayerHurtEventHandler(EventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }
        
        public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;
            
            if (attacker != null && attacker.IsValid && attacker.AuthorizedSteamID != null &&
                victim != null && victim.IsValid && victim.AuthorizedSteamID != null)
            {
                var attackerSteamId = attacker.AuthorizedSteamID.SteamId64.ToString();
                var victimSteamId = victim.AuthorizedSteamID.SteamId64.ToString();
                var damage = @event.DmgHealth;
                
                var hurtEvent = new PlayerHurtEvent
                {
                    AttackerSteamId = attackerSteamId,
                    VictimSteamId = victimSteamId,
                    Attacker = attacker,
                    Player = victim,
                    Damage = damage
                };
                
                _eventSystem.Dispatch(hurtEvent);
            }
            
            return HookResult.Continue;
        }
    }
}
