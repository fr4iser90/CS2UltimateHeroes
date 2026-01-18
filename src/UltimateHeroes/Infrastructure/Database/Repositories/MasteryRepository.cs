using System.Collections.Generic;
using System.Linq;
using Dapper;
using UltimateHeroes.Domain.Progression;
using UltimateHeroes.Infrastructure.Database;

namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// Repository für Skill Mastery Daten
    /// </summary>
    public class MasteryRepository : IMasteryRepository
    {
        private readonly Database _database;
        
        public MasteryRepository(Database database)
        {
            _database = database;
        }
        
        public SkillMastery? GetSkillMastery(string steamId, string skillId)
        {
            using var connection = _database.GetConnection();
            
            var mastery = connection.QueryFirstOrDefault<(int kills, int uses, float damage, int escapes, int level)>(
                @"SELECT kills, uses, total_damage, escapes, mastery_level 
                  FROM skill_mastery 
                  WHERE steamid = @SteamId AND skill_id = @SkillId",
                new { SteamId = steamId, SkillId = skillId }
            );
            
            if (mastery.kills == 0 && mastery.uses == 0 && mastery.damage == 0 && mastery.escapes == 0 && mastery.level == 0)
            {
                return null;
            }
            
            return new SkillMastery
            {
                SteamId = steamId,
                SkillId = skillId,
                Kills = mastery.kills,
                Uses = mastery.uses,
                TotalDamage = mastery.damage,
                Escapes = mastery.escapes,
                MasteryLevel = mastery.level
            };
        }
        
        public List<SkillMastery> GetPlayerMasteries(string steamId)
        {
            using var connection = _database.GetConnection();
            
            var masteries = connection.Query<(string skillId, int kills, int uses, float damage, int escapes, int level)>(
                @"SELECT skill_id, kills, uses, total_damage, escapes, mastery_level 
                  FROM skill_mastery 
                  WHERE steamid = @SteamId",
                new { SteamId = steamId }
            ).ToList();
            
            return masteries.Select(m => new SkillMastery
            {
                SteamId = steamId,
                SkillId = m.skillId,
                Kills = m.kills,
                Uses = m.uses,
                TotalDamage = m.damage,
                Escapes = m.escapes,
                MasteryLevel = m.level
            }).ToList();
        }
        
        public void SaveSkillMastery(SkillMastery mastery)
        {
            using var connection = _database.GetConnection();
            
            connection.Execute(
                @"INSERT OR REPLACE INTO skill_mastery 
                  (steamid, skill_id, kills, uses, total_damage, escapes, mastery_level) 
                  VALUES (@SteamId, @SkillId, @Kills, @Uses, @Damage, @Escapes, @Level)",
                new
                {
                    SteamId = mastery.SteamId,
                    SkillId = mastery.SkillId,
                    Kills = mastery.Kills,
                    Uses = mastery.Uses,
                    Damage = mastery.TotalDamage,
                    Escapes = mastery.Escapes,
                    Level = mastery.MasteryLevel
                }
            );
        }
    }
}
