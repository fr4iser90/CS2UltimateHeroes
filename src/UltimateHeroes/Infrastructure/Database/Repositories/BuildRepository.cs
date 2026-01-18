using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using UltimateHeroes.Domain.Builds;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository Implementation für Build Data Access
    /// </summary>
    public class BuildRepository : IBuildRepository
    {
        private readonly Database _database;
        
        public BuildRepository(Database database)
        {
            _database = database;
        }
        
        public Build? GetBuild(string steamId, int buildSlot)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            var build = connection.QueryFirstOrDefault<Build>(
                "SELECT * FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                new { SteamId = steamId, BuildSlot = buildSlot }
            );
            
            if (build != null)
            {
                // Load separate skill slots
                build.ActiveSkillIds = new List<string>();
                build.PassiveSkillIds = new List<string>();
                
                // Active Skills
                var active1 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                var active2 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                var active3 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill3_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                
                if (!string.IsNullOrEmpty(active1)) build.ActiveSkillIds.Add(active1);
                if (!string.IsNullOrEmpty(active2)) build.ActiveSkillIds.Add(active2);
                if (!string.IsNullOrEmpty(active3)) build.ActiveSkillIds.Add(active3);
                
                // Ultimate Skill
                build.UltimateSkillId = connection.QueryFirstOrDefault<string>(
                    "SELECT ultimate_skill_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                
                // Passive Skills
                var passive1 = connection.QueryFirstOrDefault<string>(
                    "SELECT passive_skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                var passive2 = connection.QueryFirstOrDefault<string>(
                    "SELECT passive_skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                
                if (!string.IsNullOrEmpty(passive1)) build.PassiveSkillIds.Add(passive1);
                if (!string.IsNullOrEmpty(passive2)) build.PassiveSkillIds.Add(passive2);
            }
            
            return build;
        }
        
        public List<Build> GetPlayerBuilds(string steamId)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            var builds = connection.Query<Build>(
                "SELECT * FROM builds WHERE steamid = @SteamId ORDER BY build_slot",
                new { SteamId = steamId }
            ).ToList();
            
            foreach (var build in builds)
            {
                // Load separate skill slots (gleiche Logik wie GetBuild)
                build.ActiveSkillIds = new List<string>();
                build.PassiveSkillIds = new List<string>();
                
                var active1 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var active2 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var active3 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill3_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                
                if (!string.IsNullOrEmpty(active1)) build.ActiveSkillIds.Add(active1);
                if (!string.IsNullOrEmpty(active2)) build.ActiveSkillIds.Add(active2);
                if (!string.IsNullOrEmpty(active3)) build.ActiveSkillIds.Add(active3);
                
                build.UltimateSkillId = connection.QueryFirstOrDefault<string>(
                    "SELECT ultimate_skill_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                
                var passive1 = connection.QueryFirstOrDefault<string>(
                    "SELECT passive_skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var passive2 = connection.QueryFirstOrDefault<string>(
                    "SELECT passive_skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                
                if (!string.IsNullOrEmpty(passive1)) build.PassiveSkillIds.Add(passive1);
                if (!string.IsNullOrEmpty(passive2)) build.PassiveSkillIds.Add(passive2);
            }
            
            return builds;
        }
        
        public Build? GetActiveBuild(string steamId)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            var build = connection.QueryFirstOrDefault<Build>(
                "SELECT * FROM builds WHERE steamid = @SteamId AND is_active = 1",
                new { SteamId = steamId }
            );
            
            if (build != null)
            {
                // Load separate skill slots (gleiche Logik wie GetBuild)
                build.ActiveSkillIds = new List<string>();
                build.PassiveSkillIds = new List<string>();
                
                var active1 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var active2 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var active3 = connection.QueryFirstOrDefault<string>(
                    "SELECT active_skill3_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                
                if (!string.IsNullOrEmpty(active1)) build.ActiveSkillIds.Add(active1);
                if (!string.IsNullOrEmpty(active2)) build.ActiveSkillIds.Add(active2);
                if (!string.IsNullOrEmpty(active3)) build.ActiveSkillIds.Add(active3);
                
                build.UltimateSkillId = connection.QueryFirstOrDefault<string>(
                    "SELECT ultimate_skill_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                
                var passive1 = connection.QueryFirstOrDefault<string>(
                    "SELECT passive_skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var passive2 = connection.QueryFirstOrDefault<string>(
                    "SELECT passive_skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                
                if (!string.IsNullOrEmpty(passive1)) build.PassiveSkillIds.Add(passive1);
                if (!string.IsNullOrEmpty(passive2)) build.PassiveSkillIds.Add(passive2);
            }
            
            return build;
        }
        
        public void SaveBuild(Build build)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            connection.Execute(@"
                INSERT OR REPLACE INTO builds 
                (steamid, build_slot, hero_core_id, 
                 active_skill1_id, active_skill2_id, active_skill3_id,
                 ultimate_skill_id,
                 passive_skill1_id, passive_skill2_id,
                 build_name, is_active, created_at, last_used_at)
                VALUES 
                (@SteamId, @BuildSlot, @HeroCoreId, 
                 @ActiveSkill1Id, @ActiveSkill2Id, @ActiveSkill3Id,
                 @UltimateSkillId,
                 @PassiveSkill1Id, @PassiveSkill2Id,
                 @BuildName, @IsActive, @CreatedAt, @LastUsedAt)",
                new
                {
                    build.SteamId,
                    build.BuildSlot,
                    build.HeroCoreId,
                    ActiveSkill1Id = build.ActiveSkillIds.Count > 0 ? build.ActiveSkillIds[0] : null,
                    ActiveSkill2Id = build.ActiveSkillIds.Count > 1 ? build.ActiveSkillIds[1] : null,
                    ActiveSkill3Id = build.ActiveSkillIds.Count > 2 ? build.ActiveSkillIds[2] : null,
                    UltimateSkillId = build.UltimateSkillId,
                    PassiveSkill1Id = build.PassiveSkillIds.Count > 0 ? build.PassiveSkillIds[0] : null,
                    PassiveSkill2Id = build.PassiveSkillIds.Count > 1 ? build.PassiveSkillIds[1] : null,
                    build.BuildName,
                    IsActive = build.IsActive ? 1 : 0,
                    build.CreatedAt,
                    build.LastUsedAt
                }
            );
        }
        
        public void DeleteBuild(string steamId, int buildSlot)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            connection.Execute(
                "DELETE FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                new { SteamId = steamId, BuildSlot = buildSlot }
            );
        }
    }
}
