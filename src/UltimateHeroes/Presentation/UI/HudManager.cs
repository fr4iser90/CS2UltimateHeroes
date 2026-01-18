using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Players;
using UltimateHeroes.Infrastructure.Cooldown;

namespace UltimateHeroes.Presentation.UI
{
    /// <summary>
    /// HUD Manager - Verwaltet und aktualisiert alle HUD-Elemente
    /// </summary>
    public class HudManager
    {
        private readonly SkillHud _skillHud;
        private readonly ProgressionHud _progressionHud;
        private readonly IPlayerService _playerService;
        private readonly HashSet<int> _enabledPlayers = new();
        
        private readonly IAccountService? _accountService;
        
        public HudManager(
            ISkillService skillService,
            IXpService xpService,
            IPlayerService playerService,
            ICooldownManager cooldownManager,
            IMasteryService? masteryService = null,
            IAccountService? accountService = null)
        {
            _accountService = accountService;
            _skillHud = new SkillHud(skillService, cooldownManager);
            _progressionHud = new ProgressionHud(xpService, masteryService, accountService);
            _playerService = playerService;
        }
        
        /// <summary>
        /// Aktiviert HUD für einen Spieler
        /// </summary>
        public void EnableHud(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            _enabledPlayers.Add(player.Slot);
        }
        
        /// <summary>
        /// Deaktiviert HUD für einen Spieler
        /// </summary>
        public void DisableHud(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            _enabledPlayers.Remove(player.Slot);
        }
        
        /// <summary>
        /// Aktualisiert HUD für alle aktiven Spieler
        /// </summary>
        public void UpdateHud()
        {
            var players = Utilities.GetPlayers();
            
            foreach (var player in players)
            {
                if (player == null || !player.IsValid || player.PlayerPawn.Value == null) continue;
                // Note: IsAlive doesn't exist on CCSPlayerPawn, check Health instead
                // Note: Health is int, not int? - use directly
                if (player.PlayerPawn.Value.Health <= 0) continue;
                if (!_enabledPlayers.Contains(player.Slot)) continue;
                if (player.AuthorizedSteamID == null) continue;
                
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var playerState = _playerService.GetPlayer(steamId);
                
                if (playerState == null) continue;
                
                // Combine both HUDs into one HTML string
                var html = RenderCombinedHud(player, playerState);
                player.PrintToCenterHtml(html);
            }
        }
        
        /// <summary>
        /// Rendert beide HUDs kombiniert
        /// </summary>
        private string RenderCombinedHud(CCSPlayerController player, Domain.Players.UltimatePlayer playerState)
        {
            var progressionHtml = _progressionHud.GetHtml(playerState);
            var skillHtml = _skillHud.GetHtml(player, playerState);
            
            return progressionHtml + skillHtml;
        }
        
        /// <summary>
        /// Prüft ob HUD für einen Spieler aktiviert ist
        /// </summary>
        public bool IsHudEnabled(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return false;
            return _enabledPlayers.Contains(player.Slot);
        }
    }
}
