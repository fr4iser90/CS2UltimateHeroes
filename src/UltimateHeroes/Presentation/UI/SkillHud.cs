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
            
            // Separate Active and Ultimate Skills
            var activeSkills = playerState.ActiveSkills
                .Where(s => s.Type == SkillType.Active)
                .ToList();
            
            var ultimateSkill = playerState.ActiveSkills
                .FirstOrDefault(s => s.Type == SkillType.Ultimate);
            
            if (activeSkills.Count == 0 && ultimateSkill == null) return string.Empty;
            
            var html = "<div style='position: fixed; bottom: 20px; left: 50%; transform: translateX(-50%); display: flex; flex-direction: column; gap: 8px; z-index: 1000; align-items: center;'>";
            
            // Active Skills Row (1-3)
            html += "<div style='display: flex; gap: 10px;'>";
            for (int i = 0; i < activeSkills.Count && i < 3; i++)
            {
                var skill = activeSkills[i];
                var cooldown = _cooldownManager.GetCooldown(playerState.SteamId, skill.Id);
                var isReady = cooldown <= 0;
                
                html += RenderSkillSlot(skill, cooldown, isReady, false, i + 1);
            }
            html += "</div>";
            
            // Ultimate Skill Row (separate, centered)
            if (ultimateSkill != null)
            {
                var ultimateCooldown = _cooldownManager.GetCooldown(playerState.SteamId, ultimateSkill.Id);
                var ultimateReady = ultimateCooldown <= 0;
                
                html += "<div style='display: flex; gap: 10px;'>";
                html += RenderSkillSlot(ultimateSkill, ultimateCooldown, ultimateReady, true, 0); // 0 = Ultimate
                html += "</div>";
            }
            
            html += "</div>";
            
            return html;
        }
        
        private string RenderSkillSlot(ISkill skill, float cooldown, bool isReady, bool isUltimate, int slotNumber)
        {
            var skillName = skill.DisplayName;
            var skillColor = isUltimate ? "#FFD700" : "#4A90E2";
            var bgColor = isReady 
                ? (isUltimate ? "rgba(255, 215, 0, 0.3)" : "rgba(74, 144, 226, 0.3)") 
                : "rgba(100, 100, 100, 0.3)";
            var borderColor = isReady 
                ? (isUltimate ? "#FFD700" : "#4A90E2") 
                : "#666666";
            
            var slotLabel = isUltimate ? "ULT" : slotNumber.ToString();
            
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
                        [{slotLabel}] {skillName}
                    </div>";
            
            if (!isReady && cooldown > 0)
            {
                var cooldownText = cooldown > 1 ? $"{cooldown:F1}s" : $"{cooldown:F2}s";
                html += $@"
                    <div style='font-size: 14px; color: #FF4444; font-weight: bold;'>
                        {cooldownText}
                    </div>";
            }
            else
            {
                var readyColor = isUltimate ? "#FFD700" : "#4CAF50";
                html += $@"
                    <div style='font-size: 11px; color: {readyColor}; font-weight: bold;'>
                        READY
                    </div>";
            }
            
            html += "</div>";
            
            return html;
        }
    }
}
