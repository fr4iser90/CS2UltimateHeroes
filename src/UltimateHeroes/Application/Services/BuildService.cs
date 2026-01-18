using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Application;
using UltimateHeroes.Domain.Builds;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Infrastructure.Database.Repositories;

namespace UltimateHeroes.Application.Services
{
    /// <summary>
    /// Service für Build Management
    /// </summary>
    public class BuildService : IBuildService
    {
        private readonly IBuildRepository _buildRepository;
        private readonly IHeroService _heroService;
        private readonly ISkillService _skillService;
        private readonly BuildValidator _buildValidator;
        private readonly IPlayerService _playerService;
        private readonly ITalentService? _talentService;
        
        public BuildService(
            IBuildRepository buildRepository,
            IHeroService heroService,
            ISkillService skillService,
            BuildValidator buildValidator,
            IPlayerService playerService,
            ITalentService? talentService = null)
        {
            _buildRepository = buildRepository;
            _heroService = heroService;
            _skillService = skillService;
            _buildValidator = buildValidator;
            _playerService = playerService;
            _talentService = talentService;
        }
        
        public Build CreateBuild(string steamId, int buildSlot, string heroId, 
            List<string> activeSkillIds, string? ultimateSkillId, List<string> passiveSkillIds, string buildName)
        {
            // Validate Hero
            var hero = _heroService.GetHero(heroId);
            if (hero == null)
            {
                throw new ArgumentException($"Hero {heroId} not found");
            }
            
            // Validate Active Skills
            var activeSkills = new List<ISkill>();
            foreach (var skillId in activeSkillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null)
                {
                    throw new ArgumentException($"Active Skill {skillId} not found");
                }
                activeSkills.Add(skill);
            }
            
            // Validate Ultimate Skill
            ISkill? ultimateSkill = null;
            if (!string.IsNullOrEmpty(ultimateSkillId))
            {
                ultimateSkill = _skillService.GetSkill(ultimateSkillId);
                if (ultimateSkill == null)
                {
                    throw new ArgumentException($"Ultimate Skill {ultimateSkillId} not found");
                }
            }
            
            // Validate Passive Skills
            var passiveSkills = new List<ISkill>();
            foreach (var skillId in passiveSkillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null)
                {
                    throw new ArgumentException($"Passive Skill {skillId} not found");
                }
                passiveSkills.Add(skill);
            }
            
            // Calculate slot limits (level + talents)
            var player = _playerService.GetPlayer(steamId);
            var heroLevel = player?.HeroLevel ?? 1;
            var slotLimits = _talentService?.CalculateSlotLimits(steamId, heroLevel) ?? Domain.Builds.BuildSlotLimits.CalculateBaseSlots(heroLevel);
            
            // Validate Build with dynamic slot limits
            var validation = _buildValidator.ValidateBuild(hero, activeSkills, ultimateSkill, passiveSkills, slotLimits);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException($"Build validation failed: {string.Join(", ", validation.Errors)}");
            }
            
            // Create Build
            var build = new Build
            {
                SteamId = steamId,
                BuildSlot = buildSlot,
                HeroCoreId = heroId,
                ActiveSkillIds = activeSkillIds,
                UltimateSkillId = ultimateSkillId,
                PassiveSkillIds = passiveSkillIds,
                BuildName = buildName,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                IsValid = true
            };
            
            // Save to Database
            _buildRepository.SaveBuild(build);
            
            return build;
        }
        
        public Build? GetBuild(string steamId, int buildSlot)
        {
            return _buildRepository.GetBuild(steamId, buildSlot);
        }
        
        public List<Build> GetPlayerBuilds(string steamId)
        {
            return _buildRepository.GetPlayerBuilds(steamId);
        }
        
        public void SaveBuild(Build build)
        {
            // Re-validate before saving
            var hero = _heroService.GetHero(build.HeroCoreId);
            if (hero != null)
            {
                var activeSkills = build.ActiveSkillIds.Select(id => _skillService.GetSkill(id)).Where(s => s != null).Cast<ISkill>().ToList();
                var ultimateSkill = !string.IsNullOrEmpty(build.UltimateSkillId) ? _skillService.GetSkill(build.UltimateSkillId) : null;
                var passiveSkills = build.PassiveSkillIds.Select(id => _skillService.GetSkill(id)).Where(s => s != null).Cast<ISkill>().ToList();
                
                // Calculate slot limits
                var player = _playerService.GetPlayer(build.SteamId);
                var heroLevel = player?.HeroLevel ?? 1;
                var slotLimits = _talentService?.CalculateSlotLimits(build.SteamId, heroLevel) ?? Domain.Builds.BuildSlotLimits.CalculateBaseSlots(heroLevel);
                
                var validation = _buildValidator.ValidateBuild(hero, activeSkills, ultimateSkill, passiveSkills, slotLimits);
                build.IsValid = validation.IsValid;
                build.ValidationErrors = validation.Errors;
            }
            
            build.LastUsedAt = DateTime.UtcNow;
            _buildRepository.SaveBuild(build);
        }
        
        public void DeleteBuild(string steamId, int buildSlot)
        {
            _buildRepository.DeleteBuild(steamId, buildSlot);
        }
        
        public void ActivateBuild(string steamId, int buildSlot, CCSPlayerController player)
        {
            var build = GetBuild(steamId, buildSlot);
            if (build == null)
            {
                throw new ArgumentException($"Build {buildSlot} not found");
            }
            
            if (!build.IsValid)
            {
                throw new InvalidOperationException($"Build {buildSlot} is not valid");
            }
            
            // Deactivate other builds
            var playerBuilds = GetPlayerBuilds(steamId);
            foreach (var b in playerBuilds)
            {
                if (b.IsActive && b.BuildSlot != buildSlot)
                {
                    b.IsActive = false;
                    _buildRepository.SaveBuild(b);
                }
            }
            
            // Activate this build
            build.MarkAsActive();
            _buildRepository.SaveBuild(build);
            
            // Activate for player
            var hero = _heroService.GetHero(build.HeroCoreId);
            var activeSkills = build.ActiveSkillIds.Select(id => _skillService.GetSkill(id)).Where(s => s != null).Cast<ISkill>().ToList();
            var ultimateSkill = !string.IsNullOrEmpty(build.UltimateSkillId) ? _skillService.GetSkill(build.UltimateSkillId) : null;
            var passiveSkills = build.PassiveSkillIds.Select(id => _skillService.GetSkill(id)).Where(s => s != null).Cast<ISkill>().ToList();
            
            // Combine all skills for ActivateBuild
            var allSkills = new List<ISkill>(activeSkills);
            if (ultimateSkill != null) allSkills.Add(ultimateSkill);
            allSkills.AddRange(passiveSkills);
            
            if (hero != null)
            {
                var playerState = _playerService.GetPlayer(steamId);
                if (playerState != null)
                {
                    playerState.ActivateBuild(build, hero, allSkills);
                }
            }
        }
        
        public Build? GetActiveBuild(string steamId)
        {
            return _buildRepository.GetActiveBuild(steamId);
        }
        
        public ValidationResult ValidateBuild(string heroId, 
            List<string> activeSkillIds, string? ultimateSkillId, List<string> passiveSkillIds)
        {
            var hero = _heroService.GetHero(heroId);
            if (hero == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { $"Hero {heroId} not found" }
                };
            }
            
            var activeSkills = new List<ISkill>();
            foreach (var skillId in activeSkillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Errors = new List<string> { $"Active Skill {skillId} not found" }
                    };
                }
                activeSkills.Add(skill);
            }
            
            ISkill? ultimateSkill = null;
            if (!string.IsNullOrEmpty(ultimateSkillId))
            {
                ultimateSkill = _skillService.GetSkill(ultimateSkillId);
                if (ultimateSkill == null)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Errors = new List<string> { $"Ultimate Skill {ultimateSkillId} not found" }
                    };
                }
            }
            
            var passiveSkills = new List<ISkill>();
            foreach (var skillId in passiveSkillIds)
            {
                var skill = _skillService.GetSkill(skillId);
                if (skill == null)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Errors = new List<string> { $"Passive Skill {skillId} not found" }
                    };
                }
                passiveSkills.Add(skill);
            }
            
            // Calculate slot limits (need steamId for talents, but we don't have it here)
            // For validation without player context, use default limits
            var slotLimits = Domain.Builds.BuildSlotLimits.CalculateBaseSlots(1);
            
            return _buildValidator.ValidateBuild(hero, activeSkills, ultimateSkill, passiveSkills, slotLimits);
        }
        
        public List<int> GetUnlockedSlots(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return new List<int> { 1, 2, 3 }; // Default: 3 Slots
            
            var level = player.HeroLevel;
            return BuildSlot.GetUnlockedSlots(level);
        }
        
        public bool IsSlotUnlocked(string steamId, int slot)
        {
            return GetUnlockedSlots(steamId).Contains(slot);
        }
    }
}
