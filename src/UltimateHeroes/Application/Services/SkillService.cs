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
            
            // EffectManager wird jetzt via Reflection gesetzt (siehe UltimateHeroes.cs)
            // Keine hardcoded Zuweisungen mehr nötig!
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
                    
                    // Apply Utility CDR Passive (if skill has Utility tag)
                    if (skill.Tags.Contains(SkillTag.Utility))
                    {
                        foreach (var passiveSkill in playerState.ActiveSkills)
                        {
                            if (passiveSkill is Domain.Skills.ConcreteSkills.UtilityCooldownReductionPassive utilityCdr)
                            {
                                var cdrReduction = utilityCdr.GetCooldownReduction();
                                cooldown *= (1f - cdrReduction);
                                break;
                            }
                        }
                    }
                    
                    // Apply Overclock Passive (HP Cost + Power Bonus)
                    foreach (var passiveSkill in playerState.ActiveSkills)
                    {
                        if (passiveSkill is Domain.Skills.ConcreteSkills.OverclockPassive overclock)
                        {
                            // Apply HP Cost
                            var hpCost = overclock.GetHpCost();
                            if (player.PlayerPawn.Value?.Health != null)
                            {
                                var currentHealth = player.PlayerPawn.Value.Health.Value;
                                var newHealth = System.Math.Max(currentHealth - hpCost, 1);
                                player.PlayerPawn.Value.Health.Value = newHealth;
                            }
                            
                            // Power Bonus is applied per-skill (skills need to check for Overclock)
                            // This is handled by individual skills that support power bonuses
                            break;
                        }
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
        
        public void ReduceCooldown(string steamId, string skillId, float reductionPercent)
        {
            var currentCooldown = GetSkillCooldown(steamId, skillId);
            if (currentCooldown > 0f)
            {
                var newCooldown = currentCooldown * (1f - reductionPercent);
                _cooldownManager.SetCooldown(steamId, skillId, newCooldown);
            }
        }
        
        /// <summary>
        /// Automatically registers all ISkill implementations via Reflection
        /// </summary>
        public void RegisterSkillsViaReflection()
        {
            var skillType = typeof(ISkill);
            var skillTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => skillType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            
            foreach (var type in skillTypes)
            {
                try
                {
                    var skill = (ISkill)System.Activator.CreateInstance(type)!;
                    RegisterSkill(skill);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SkillService] Failed to register skill {type.Name}: {ex.Message}");
                }
            }
        }
    }
}
