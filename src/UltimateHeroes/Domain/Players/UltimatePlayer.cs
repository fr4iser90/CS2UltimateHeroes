using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Builds;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Domain.Players
{
    /// <summary>
    /// Repräsentiert den aktuellen Zustand eines Spielers im Match
    /// </summary>
    public class UltimatePlayer
    {
        // Identity
        public string SteamId { get; set; } = string.Empty;
        public CCSPlayerController? PlayerController { get; set; }
        
        // Current State
        public IHero? CurrentHero { get; set; }
        public Build? CurrentBuild { get; set; }
        public List<ISkill> ActiveSkills { get; set; } = new();
        public Dictionary<string, int> SkillLevels { get; set; } = new(); // skill_id -> level
        
        // Progression
        public int HeroLevel { get; set; } = 1;
        public float CurrentXp { get; set; } = 0f;
        public float XpToNextLevel { get; set; } = 100f;
        
        // Active Effects (später mit IEffect)
        // public List<IEffect> ActiveEffects { get; set; } = new();
        
        // Cooldowns
        public Dictionary<string, DateTime> SkillCooldowns { get; set; } = new(); // skill_id -> cooldown_end
        
        // Match Stats
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int Headshots { get; set; } = 0;
        
        // Role Influence
        public RoleInfluence CurrentRole { get; set; } = RoleInfluence.None;
        
        // Talent Modifiers (applied from TalentService)
        public Dictionary<string, float> TalentModifiers { get; set; } = new(); // modifier_key -> value
        
        // Item Modifiers (temporäre Match-Items)
        public Dictionary<string, float> ItemModifiers { get; set; } = new(); // modifier_key -> value (damage_boost, speed_boost, etc.)
        
        /// <summary>
        /// Prüft ob ein Skill bereit ist (Cooldown abgelaufen)
        /// </summary>
        public bool IsSkillReady(string skillId)
        {
            if (!SkillCooldowns.ContainsKey(skillId)) return true;
            return DateTime.UtcNow >= SkillCooldowns[skillId];
        }
        
        /// <summary>
        /// Setzt Cooldown für einen Skill
        /// </summary>
        public void SetSkillCooldown(string skillId, float cooldownSeconds)
        {
            SkillCooldowns[skillId] = DateTime.UtcNow.AddSeconds(cooldownSeconds);
        }
        
        /// <summary>
        /// Aktiviert einen Build für den Spieler
        /// </summary>
        public void ActivateBuild(Build build, IHero hero, List<ISkill> skills)
        {
            CurrentBuild = build;
            CurrentHero = hero;
            ActiveSkills = skills;
            
            // Initialize Hero Passives
            if (PlayerController != null && PlayerController.IsValid)
            {
                hero.OnPlayerSpawn(PlayerController);
            }
        }
        
        /// <summary>
        /// Deaktiviert den aktuellen Build
        /// </summary>
        public void DeactivateBuild()
        {
            CurrentBuild = null;
            CurrentHero = null;
            ActiveSkills.Clear();
        }
        
        /// <summary>
        /// Fügt einen Skill hinzu
        /// </summary>
        public void AddSkill(ISkill skill, int level = 1)
        {
            if (!ActiveSkills.Any(s => s.Id == skill.Id))
            {
                ActiveSkills.Add(skill);
            }
            SkillLevels[skill.Id] = level;
        }
        
        /// <summary>
        /// Entfernt einen Skill
        /// </summary>
        public void RemoveSkill(string skillId)
        {
            ActiveSkills.RemoveAll(s => s.Id == skillId);
            SkillLevels.Remove(skillId);
        }
        
        /// <summary>
        /// Gibt einen Skill zurück
        /// </summary>
        public ISkill? GetSkill(string skillId)
        {
            return ActiveSkills.FirstOrDefault(s => s.Id == skillId);
        }
        
        /// <summary>
        /// Prüft ob ein Skill aktiviert werden kann
        /// </summary>
        public bool CanActivateSkill(string skillId)
        {
            var skill = GetSkill(skillId);
            if (skill == null) return false;
            if (skill.Type == SkillType.Passive) return false;
            return IsSkillReady(skillId);
        }
        
        /// <summary>
        /// Event: Spieler hat einen Kill gemacht
        /// </summary>
        public void OnKill(CCSPlayerController victim)
        {
            Kills++;
            
            // Trigger Passive Skills
            foreach (var skill in ActiveSkills)
            {
                if (skill is IPassiveSkill passiveSkill && PlayerController != null && victim != null)
                {
                    passiveSkill.OnPlayerKill(PlayerController, victim);
                }
            }
        }
        
        /// <summary>
        /// Event: Spieler ist gestorben
        /// </summary>
        public void OnDeath()
        {
            Deaths++;
        }
        
        /// <summary>
        /// Event: Headshot gemacht
        /// </summary>
        public void OnHeadshot()
        {
            Headshots++;
        }
    }
}
