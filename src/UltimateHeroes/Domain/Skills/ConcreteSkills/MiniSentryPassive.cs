using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Mini Sentry Passive - Passive Skill: Platziert automatisch Mini-Sentry nach Cooldown
    /// </summary>
    public class MiniSentryPassive : PassiveSkillBase
    {
        public override string Id => "mini_sentry_passive";
        public override string DisplayName => "Mini Sentry";
        public override string Description => "Platziert automatisch Mini-Sentry nach Cooldown";
        public override int PowerWeight => 20;
        public override List<SkillTag> Tags => new() { SkillTag.Utility, SkillTag.Area };
        public override int MaxLevel => 5;
        
        private const float BaseCooldown = 30f;
        private const int BaseDamage = 10;
        private const float BaseRange = 500f;
        
        // Track last sentry placement per player
        private static Dictionary<string, System.DateTime> _lastSentryTime = new();
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            _lastSentryTime[steamId] = System.DateTime.UtcNow;
            
            // Place initial sentry after spawn
            Server.NextFrame(() =>
            {
                PlaceSentry(player);
            });
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // Check if cooldown is ready
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            if (!_lastSentryTime.TryGetValue(steamId, out var lastTime)) return;
            
            var cooldown = BaseCooldown - (CurrentLevel * 3f); // 30s - 15s
            var elapsed = (System.DateTime.UtcNow - lastTime).TotalSeconds;
            
            if (elapsed >= cooldown)
            {
                PlaceSentry(player);
                _lastSentryTime[steamId] = System.DateTime.UtcNow;
            }
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            // Check if cooldown is ready
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            if (!_lastSentryTime.TryGetValue(steamId, out var lastTime)) return;
            
            var cooldown = BaseCooldown - (CurrentLevel * 3f);
            var elapsed = (System.DateTime.UtcNow - lastTime).TotalSeconds;
            
            if (elapsed >= cooldown)
            {
                PlaceSentry(player);
                _lastSentryTime[steamId] = System.DateTime.UtcNow;
            }
        }
        
        // SpawnService wird über Helper gesetzt
        public static Application.Services.ISpawnService? SpawnService { get; set; }
        
        private void PlaceSentry(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null || player.AuthorizedSteamID == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null) return;
            
            // Calculate sentry stats based on level
            var damage = BaseDamage + (CurrentLevel * 3);
            var range = BaseRange + (CurrentLevel * 50);
            var duration = 30f; // Sentry lasts 30 seconds
            
            // Spawn sentry via SpawnService
            if (SpawnService != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var sentryPos = new Vector(pawn.AbsOrigin.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + 10);
                SpawnService.SpawnSentry(steamId, sentryPos, range, damage, duration);
            }
            
            player.PrintToChat($" {ChatColors.Green}[Mini Sentry]{ChatColors.Default} Sentry placed! Damage: {damage}, Range: {range:F0}");
        }
    }
}
