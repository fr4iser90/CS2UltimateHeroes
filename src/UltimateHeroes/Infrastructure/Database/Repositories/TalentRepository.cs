using System.Collections.Generic;
using System.Linq;
using Dapper;
using UltimateHeroes.Domain.Talents;
using UltimateHeroes.Infrastructure.Database;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository für Talent-Daten
    /// </summary>
    public class TalentRepository : ITalentRepository
    {
        private readonly Database _database;
        
        public TalentRepository(Database database)
        {
            _database = database;
        }
        
        public PlayerTalents? GetPlayerTalents(string steamId)
        {
            using var connection = _database.GetConnection();
            
            // Get unlocked talents
            var talents = connection.Query<(string talentId, int level)>(
                "SELECT talent_id, talent_level FROM talents WHERE steamid = @SteamId",
                new { SteamId = steamId }
            ).ToList();
            
            // Get talent points
            var points = connection.QueryFirstOrDefault<(int available, int total)>(
                "SELECT available_points, total_earned FROM talent_points WHERE steamid = @SteamId",
                new { SteamId = steamId }
            );
            
            if (talents.Count == 0 && points.available == 0 && points.total == 0)
            {
                return null;
            }
            
            var playerTalents = new PlayerTalents
            {
                SteamId = steamId,
                UnlockedTalents = talents.Select(t => t.talentId).ToList(),
                TalentLevels = talents.ToDictionary(t => t.talentId, t => t.level),
                AvailableTalentPoints = points.available
            };
            
            return playerTalents;
        }
        
        public void SavePlayerTalents(PlayerTalents playerTalents)
        {
            using var connection = _database.GetConnection();
            
            // Delete existing talents
            connection.Execute(
                "DELETE FROM talents WHERE steamid = @SteamId",
                new { SteamId = playerTalents.SteamId }
            );
            
            // Insert/Update talents
            foreach (var talent in playerTalents.UnlockedTalents)
            {
                var level = playerTalents.GetTalentLevel(talent);
                connection.Execute(
                    @"INSERT OR REPLACE INTO talents (steamid, talent_id, talent_level) 
                      VALUES (@SteamId, @TalentId, @Level)",
                    new { SteamId = playerTalents.SteamId, TalentId = talent, Level = level }
                );
            }
            
            // Update talent points
            connection.Execute(
                @"INSERT OR REPLACE INTO talent_points (steamid, available_points, total_earned) 
                  VALUES (@SteamId, @Available, @Total)",
                new 
                { 
                    SteamId = playerTalents.SteamId, 
                    Available = playerTalents.AvailableTalentPoints,
                    Total = playerTalents.AvailableTalentPoints // TODO: Track total separately
                }
            );
        }
    }
}
