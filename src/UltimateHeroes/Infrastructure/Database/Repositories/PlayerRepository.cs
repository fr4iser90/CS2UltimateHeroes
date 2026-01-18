using System;
using System.Linq;
using Dapper;
using UltimateHeroes.Domain.Players;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository Implementation für Player Data Access
    /// </summary>
    public class PlayerRepository : IPlayerRepository
    {
        private readonly Database _database;
        
        public PlayerRepository(Database database)
        {
            _database = database;
        }
        
        public UltimatePlayer? GetPlayer(string steamId)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            var player = connection.QueryFirstOrDefault<UltimatePlayer>(
                "SELECT * FROM players WHERE steamid = @SteamId",
                new { SteamId = steamId }
            );
            
            if (player != null)
            {
                // Load Skills
                var skills = connection.Query<PlayerSkill>(
                    "SELECT * FROM player_skills WHERE steamid = @SteamId",
                    new { SteamId = steamId }
                );
                
                player.SkillLevels = skills.ToDictionary(s => s.SkillId, s => s.SkillLevel);
            }
            
            return player;
        }
        
        public void SavePlayer(UltimatePlayer player)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            
            connection.Execute(@"
                INSERT OR REPLACE INTO players 
                (steamid, hero_core_id, hero_level, current_xp, xp_to_next_level, current_role, last_updated)
                VALUES 
                (@SteamId, @HeroCoreId, @HeroLevel, @CurrentXp, @XpToNextLevel, @CurrentRole, @LastUpdated)",
                new
                {
                    player.SteamId,
                    HeroCoreId = player.CurrentHero?.Id ?? "",
                    player.HeroLevel,
                    player.CurrentXp,
                    player.XpToNextLevel,
                    CurrentRole = player.CurrentRole.ToString(),
                    LastUpdated = DateTime.UtcNow
                }
            );
            
            // Delete old skills
            connection.Execute(
                "DELETE FROM player_skills WHERE steamid = @SteamId",
                new { player.SteamId }
            );
            
            // Save Skills
            foreach (var skillLevel in player.SkillLevels)
            {
                connection.Execute(@"
                    INSERT INTO player_skills 
                    (steamid, skill_id, skill_level)
                    VALUES 
                    (@SteamId, @SkillId, @SkillLevel)",
                    new
                    {
                        player.SteamId,
                        SkillId = skillLevel.Key,
                        SkillLevel = skillLevel.Value
                    }
                );
            }
        }
        
        public void DeletePlayer(string steamId)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            connection.Execute("DELETE FROM players WHERE steamid = @SteamId", new { SteamId = steamId });
        }
        
        public string? GetHeroCoreId(string steamId)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            return connection.QueryFirstOrDefault<string>(
                "SELECT hero_core_id FROM players WHERE steamid = @SteamId",
                new { SteamId = steamId }
            );
        }
    }
}
