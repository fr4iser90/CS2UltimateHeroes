using System;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Domain.Skills.ConcreteSkills;
using UltimateHeroes.Infrastructure.Events;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Infrastructure.Events.EventHandlers
{
    /// <summary>
    /// Handler für Player Kill Events
    /// </summary>
    public class PlayerKillHandler : IEventHandler<PlayerKillEvent>
    {
        private readonly IXpService _xpService;
        private readonly IPlayerService _playerService;
        private readonly IMasteryService? _masteryService;
        private readonly Application.Services.IInMatchEvolutionService? _inMatchEvolutionService;
        private readonly Application.Services.IBotService? _botService;
        private readonly AssistTracking _assistTracking;
        private readonly Application.Services.IBuffService? _buffService;
        private readonly Application.Services.ISkillService? _skillService;
        
        public PlayerKillHandler(
            IXpService xpService, 
            IPlayerService playerService, 
            AssistTracking assistTracking,
            IMasteryService? masteryService = null, 
            Application.Services.IInMatchEvolutionService? inMatchEvolutionService = null, 
            Application.Services.IBotService? botService = null,
            Application.Services.IBuffService? buffService = null,
            Application.Services.ISkillService? skillService = null)
        {
            _xpService = xpService;
            _playerService = playerService;
            _assistTracking = assistTracking;
            _masteryService = masteryService;
            _inMatchEvolutionService = inMatchEvolutionService;
            _botService = botService;
            _buffService = buffService;
            _skillService = skillService;
        }
        
        public void Handle(PlayerKillEvent eventData)
        {
            var player = _playerService.GetPlayer(eventData.KillerSteamId);
            if (player == null) return;
            
            // Check for Backstab (180° behind victim)
            bool isBackstab = IsBackstab(eventData.Killer, eventData.Victim);
            
            // Anti-Exploit: Kill-Diminishing (gemäß LEVELING.md)
            var victimSteamId = eventData.VictimSteamId;
            player.KillTracking.RecordKill(victimSteamId);
            var killMultiplier = player.KillTracking.GetKillXpMultiplier(victimSteamId);
            
            // Award XP mit Kill-Diminishing
            var baseXp = Domain.Progression.XpSystem.XpPerKill;
            var adjustedXp = baseXp * killMultiplier;
            _xpService.AwardXp(eventData.KillerSteamId, XpSource.Kill, adjustedXp);
            
            if (eventData.IsHeadshot)
            {
                _xpService.AwardXp(eventData.KillerSteamId, XpSource.Headshot);
            }
            
            // Handle Backstab Momentum Passive (Cooldown Reduction)
            if (isBackstab)
            {
                foreach (var skill in player.ActiveSkills)
                {
                    if (skill is BackstabMomentumPassive backstabPassive)
                    {
                        var reduction = backstabPassive.GetCooldownReduction();
                        // Apply cooldown reduction to all active skills
                        foreach (var activeSkill in player.ActiveSkills.Where(s => s.Type != SkillType.Passive))
                        {
                            _skillService?.ReduceCooldown(eventData.KillerSteamId, activeSkill.Id, reduction);
                        }
                        
                        if (eventData.Killer != null && eventData.Killer.IsValid)
                        {
                            eventData.Killer.PrintToChat($" {ChatColors.Purple}[Backstab Momentum]{ChatColors.Default} Cooldowns reduced by {reduction * 100:F0}%!");
                        }
                    }
                }
            }
            
            // Track Skill Mastery (if kill was with a skill)
            // Note: We need to detect which skill was used for the kill
            // For now, we'll track kills for all active skills (simplified)
            foreach (var skill in player.ActiveSkills)
            {
                _masteryService?.TrackSkillKill(eventData.KillerSteamId, skill.Id);
            }
            
            // Update player stats
            if (eventData.Victim != null)
            {
                player.OnKill(eventData.Victim);
            }
            
            // Track kill for In-Match Evolution
            _inMatchEvolutionService?.OnKill(eventData.KillerSteamId);
            
            // Track death for victim
            _inMatchEvolutionService?.OnDeath(eventData.VictimSteamId);
            
            // Handle Assists
            var assists = _assistTracking.GetAssists(eventData.KillerSteamId, eventData.VictimSteamId);
            foreach (var assistSteamId in assists)
            {
                // Award XP for assist
                _xpService.AwardXp(assistSteamId, XpSource.Assist);
                
                // Trigger Shield on Assist Passive
                var assistPlayer = _playerService.GetPlayer(assistSteamId);
                if (assistPlayer != null && eventData.Victim != null)
                {
                    foreach (var skill in assistPlayer.ActiveSkills)
                    {
                        if (skill is ShieldOnAssistPassive shieldOnAssist)
                        {
                            var assistController = Utilities.GetPlayers()
                                .FirstOrDefault(p => p != null && p.IsValid && 
                                    p.AuthorizedSteamID?.SteamId64.ToString() == assistSteamId);
                            
                            if (assistController != null && assistController.IsValid)
                            {
                                shieldOnAssist.OnPlayerKill(assistController, eventData.Victim);
                            }
                        }
                    }
                }
            }
            
            // Clear assist tracking for victim
            _assistTracking.ClearVictimTracking(eventData.VictimSteamId);
            
            // Track bot stats
            if (_botService != null && _botService.IsBot(eventData.KillerSteamId))
            {
                // Find which skill was used (simplified - track all active skills)
                string? skillId = null;
                foreach (var skill in player.ActiveSkills)
                {
                    if (skill.Type != SkillType.Passive)
                    {
                        skillId = skill.Id;
                        break; // Use first active skill
                    }
                }
                _botService.OnBotKill(eventData.KillerSteamId, skillId ?? "", 0f);
            }
            
            if (_botService != null && _botService.IsBot(eventData.VictimSteamId))
            {
                _botService.OnBotDeath(eventData.VictimSteamId);
            }
        }
        
        /// <summary>
        /// Checks if a kill is a backstab (killer is behind victim, ~180° angle)
        /// </summary>
        private bool IsBackstab(CCSPlayerController? killer, CCSPlayerController? victim)
        {
            if (killer == null || !killer.IsValid || victim == null || !victim.IsValid) return false;
            
            var killerPawn = killer.PlayerPawn.Value;
            var victimPawn = victim.PlayerPawn.Value;
            
            if (killerPawn?.AbsOrigin == null || victimPawn?.AbsOrigin == null) return false;
            if (killerPawn.EyeAngles == null || victimPawn.EyeAngles == null) return false;
            
            // Calculate direction from victim to killer
            var direction = new Vector(
                killerPawn.AbsOrigin.X - victimPawn.AbsOrigin.X,
                killerPawn.AbsOrigin.Y - victimPawn.AbsOrigin.Y,
                0 // Ignore Z for horizontal angle
            );
            
            // Normalize direction
            var distance = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (distance < 0.1f) return false; // Too close, can't determine
            
            direction.X /= distance;
            direction.Y /= distance;
            
            // Get victim's forward direction (yaw angle)
            var victimYaw = victimPawn.EyeAngles.Y * (Math.PI / 180.0);
            var victimForwardX = Math.Cos(victimYaw);
            var victimForwardY = Math.Sin(victimYaw);
            
            // Calculate dot product (cosine of angle)
            var dotProduct = direction.X * (float)victimForwardX + direction.Y * (float)victimForwardY;
            
            // Backstab if angle is > 90° (dot product < 0) and within reasonable distance
            // More lenient: allow up to 135° (dot product < -0.707)
            return dotProduct < -0.707f && distance < 200f; // Within 200 units
        }
    }
}

