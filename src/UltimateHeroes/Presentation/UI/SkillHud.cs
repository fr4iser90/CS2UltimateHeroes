using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Infrastructure.Cooldown;

namespace UltimateHeroes.Presentation.UI
{
    /// <summary>
    /// Skill HUD - Zeigt aktive Skills mit Cooldowns
    /// </summary>
    public class SkillHud
    {
        private readonly ISkillService _skillService;
        private readonly ICooldownManager _cooldownManager;
        
        public SkillHud(ISkillService skillService, ICooldownManager cooldownManager)
        {
            _skillService = skillService;
            _cooldownManager = cooldownManager;
        }
        
        /// <summary>
        /// Rendert das Skill HUD für einen Spieler
        /// </summary>
        public void Render(CCSPlayerController player, Domain.Players.UltimatePlayer playerState)
        {
            if (player == null || !player.IsValid || playerState == null) return;
            player.PrintToCenterHtml(GetHtml(player, playerState));
        }
        
        /// <summary>
        /// Gibt HTML für Skill HUD zurück
        /// </summary>
        public string GetHtml(CCSPlayerController player, Domain.Players.UltimatePlayer playerState)
        {
            if (player == null || !player.IsValid || playerState == null) return string.Empty;
            
            var activeSkills = playerState.ActiveSkills
                .Where(s => s.Type == SkillType.Active || s.Type == SkillType.Ultimate)
                .OrderBy(s => s.Type == SkillType.Ultimate ? 1 : 0) // Ultimates zuletzt
                .ToList();
            
            if (activeSkills.Count == 0) return string.Empty;
            
            var html = "<div style='position: fixed; bottom: 20px; left: 50%; transform: translateX(-50%); display: flex; gap: 10px; z-index: 1000;'>";
            
            for (int i = 0; i < activeSkills.Count && i < 4; i++)
            {
                var skill = activeSkills[i];
                var cooldown = _cooldownManager.GetCooldown(playerState.SteamId, skill.Id);
                var isReady = cooldown <= 0;
                var isUltimate = skill.Type == SkillType.Ultimate;
                
                html += RenderSkillSlot(skill, cooldown, isReady, isUltimate, i + 1);
            }
            
            html += "</div>";
            
            return html;
        }
        
        private string RenderSkillSlot(ISkill skill, float cooldown, bool isReady, bool isUltimate, int slotNumber)
        {
            var skillName = skill.DisplayName;
            var skillColor = isUltimate ? "#FFD700" : "#4A90E2";
            var bgColor = isReady ? "rgba(74, 144, 226, 0.3)" : "rgba(100, 100, 100, 0.3)";
            var borderColor = isReady ? "#4A90E2" : "#666666";
            
            var html = $@"
                <div style='
                    background: {bgColor};
                    border: 2px solid {borderColor};
                    border-radius: 8px;
                    padding: 8px 12px;
                    min-width: 100px;
                    text-align: center;
                    box-shadow: 0 2px 8px rgba(0,0,0,0.3);
                '>
                    <div style='font-size: 12px; color: {skillColor}; font-weight: bold; margin-bottom: 4px;'>
                        [{slotNumber}] {skillName}
                    </div>";
            
            if (!isReady && cooldown > 0)
            {
                var cooldownText = cooldown > 1 ? $"{cooldown:F1}s" : $"{cooldown:F2}s";
                html += $@"
                    <div style='font-size: 14px; color: #FF4444; font-weight: bold;'>
                        {cooldownText}
                    </div>";
            }
            else if (isUltimate)
            {
                html += $@"
                    <div style='font-size: 11px; color: #FFD700; font-weight: bold;'>
                        READY
                    </div>";
            }
            else
            {
                html += $@"
                    <div style='font-size: 11px; color: #4CAF50; font-weight: bold;'>
                        READY
                    </div>";
            }
            
            html += "</div>";
            
            return html;
        }
    }
}
