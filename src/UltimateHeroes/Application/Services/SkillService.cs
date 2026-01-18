using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Infrastructure.Cooldown;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Skill Management
    /// </summary>
    public class SkillService : ISkillService
    {
        private readonly Dictionary<string, ISkill> _skills = new();
        private readonly ICooldownManager _cooldownManager;
        private readonly IPlayerService _playerService;
        private readonly Infrastructure.Effects.EffectManager? _effectManager;
        private readonly IMasteryService? _masteryService;
        
        public SkillService(ICooldownManager cooldownManager, IPlayerService playerService, Infrastructure.Effects.EffectManager? effectManager = null, IMasteryService? masteryService = null)
        {
            _cooldownManager = cooldownManager;
            _playerService = playerService;
            _effectManager = effectManager;
            _masteryService = masteryService;
            
            // Set EffectManager for skills that need it
            if (_effectManager != null)
            {
                ConcreteSkills.Stealth.EffectManager = _effectManager;
            }
        }
        
        public void RegisterSkill(ISkill skill)
        {
            if (_skills.ContainsKey(skill.Id))
            {
                throw new InvalidOperationException($"Skill {skill.Id} already registered");
            }
            
            _skills[skill.Id] = skill;
        }
        
        public void RegisterSkills(IEnumerable<ISkill> skills)
        {
            foreach (var skill in skills)
            {
                RegisterSkill(skill);
            }
        }
        
        public ISkill? GetSkill(string skillId)
        {
            return _skills.GetValueOrDefault(skillId);
        }
        
        public List<ISkill> GetAllSkills()
        {
            return _skills.Values.ToList();
        }
        
        public List<ISkill> GetSkillsByType(SkillType type)
        {
            return _skills.Values.Where(s => s.Type == type).ToList();
        }
        
        public List<ISkill> GetSkillsByTag(SkillTag tag)
        {
            return _skills.Values.Where(s => s.Tags.Contains(tag)).ToList();
        }
        
        public bool SkillExists(string skillId)
        {
            return _skills.ContainsKey(skillId);
        }
        
        public bool CanActivateSkill(string steamId, string skillId)
        {
            var skill = GetSkill(skillId);
            if (skill == null) return false;
            
            if (skill.Type == SkillType.Passive) return false; // Passive können nicht aktiviert werden
            
            return IsSkillReady(steamId, skillId);
        }
        
        public void ActivateSkill(string steamId, string skillId, CCSPlayerController player)
        {
            var skill = GetSkill(skillId);
            if (skill == null) return;
            
            if (!CanActivateSkill(steamId, skillId)) return;
            
            // Activate based on type
            if (skill is IActiveSkill activeSkill)
            {
                // Store skill ID for damage tracking
                var currentSkillId = skillId;
                
                activeSkill.Activate(player);
                
                // Track Skill Use for Mastery
                _masteryService?.TrackSkillUse(steamId, skillId);
                
                // Note: Damage tracking for skills like Fireball is done within the skill itself
                // Skills that deal damage should call TrackSkillDamage via a callback or event
                // For now, we track uses and kills, damage tracking can be added per-skill
                
                // Set Cooldown
                var playerState = _playerService.GetPlayer(steamId);
                if (playerState != null)
                {
                    var cooldown = activeSkill.Cooldown;
                    var hero = playerState.CurrentHero;
                    if (hero != null)
                    {
                        // Apply Hero Identity Cooldown Reduction
                        var reduction = hero.Identity.GetCooldownReduction(skill);
                        cooldown *= (1f - reduction);
                    }
                    
                    _cooldownManager.SetCooldown(steamId, skillId, cooldown);
                }
            }
        }
        
        /// <summary>
        /// Tracks damage dealt by a skill (called by skills after dealing damage)
        /// </summary>
        public void TrackSkillDamage(string steamId, string skillId, float damage)
        {
            _masteryService?.TrackSkillDamage(steamId, skillId, damage);
        }
        
        public float GetSkillCooldown(string steamId, string skillId)
        {
            return _cooldownManager.GetCooldown(steamId, skillId);
        }
        
        public bool IsSkillReady(string steamId, string skillId)
        {
            return _cooldownManager.IsReady(steamId, skillId);
        }
    }
}
