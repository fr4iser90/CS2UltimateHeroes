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
                // Parse SkillIds
                build.SkillIds = new List<string>();
                var skill1 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                var skill2 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                var skill3 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill3_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = buildSlot }
                );
                
                if (!string.IsNullOrEmpty(skill1)) build.SkillIds.Add(skill1);
                if (!string.IsNullOrEmpty(skill2)) build.SkillIds.Add(skill2);
                if (!string.IsNullOrEmpty(skill3)) build.SkillIds.Add(skill3);
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
                build.SkillIds = new List<string>();
                var skill1 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var skill2 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var skill3 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill3_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                
                if (!string.IsNullOrEmpty(skill1)) build.SkillIds.Add(skill1);
                if (!string.IsNullOrEmpty(skill2)) build.SkillIds.Add(skill2);
                if (!string.IsNullOrEmpty(skill3)) build.SkillIds.Add(skill3);
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
                build.SkillIds = new List<string>();
                var skill1 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill1_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var skill2 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill2_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                var skill3 = connection.QueryFirstOrDefault<string>(
                    "SELECT skill3_id FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                    new { SteamId = steamId, BuildSlot = build.BuildSlot }
                );
                
                if (!string.IsNullOrEmpty(skill1)) build.SkillIds.Add(skill1);
                if (!string.IsNullOrEmpty(skill2)) build.SkillIds.Add(skill2);
                if (!string.IsNullOrEmpty(skill3)) build.SkillIds.Add(skill3);
            }
            
            return build;
        }
        
        public void SaveBuild(Build build)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            connection.Execute(@"
                INSERT OR REPLACE INTO builds 
                (steamid, build_slot, hero_core_id, skill1_id, skill2_id, skill3_id, build_name, is_active, created_at, last_used_at)
                VALUES 
                (@SteamId, @BuildSlot, @HeroCoreId, @Skill1Id, @Skill2Id, @Skill3Id, @BuildName, @IsActive, @CreatedAt, @LastUsedAt)",
                new
                {
                    build.SteamId,
                    build.BuildSlot,
                    build.HeroCoreId,
                    Skill1Id = build.SkillIds.Count > 0 ? build.SkillIds[0] : null,
                    Skill2Id = build.SkillIds.Count > 1 ? build.SkillIds[1] : null,
                    Skill3Id = build.SkillIds.Count > 2 ? build.SkillIds[2] : null,
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
