using System.Data;
using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;

namespace UltimateHeroes.Infrastructure.Database
{
    /// <summary>
    /// Database Connection und Initialisierung
    /// </summary>
    public class Database
    {
        private readonly string _connectionString;
        private readonly string _dbPath;
        
        public Database(string dbPath)
        {
            _dbPath = dbPath;
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }
        
        public IDbConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }
        
        private void InitializeDatabase()
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            using var connection = GetConnection();
            connection.Open();
            
            // Execute Schema (always use embedded schema for reliability)
            var schema = GetEmbeddedSchema();
            connection.Execute(schema);
        }
        
        private string GetEmbeddedSchema()
        {
            // Complete schema embedded in code
            return @"
                CREATE TABLE IF NOT EXISTS players (
                    steamid TEXT PRIMARY KEY,
                    hero_core_id TEXT,
                    hero_level INTEGER DEFAULT 1,
                    current_xp REAL DEFAULT 0,
                    xp_to_next_level REAL DEFAULT 100,
                    current_role TEXT,
                    last_updated TEXT DEFAULT CURRENT_TIMESTAMP
                );
                
                CREATE TABLE IF NOT EXISTS builds (
                    steamid TEXT NOT NULL,
                    build_slot INTEGER NOT NULL,
                    hero_core_id TEXT NOT NULL,
                    skill1_id TEXT,
                    skill2_id TEXT,
                    skill3_id TEXT,
                    build_name TEXT,
                    is_active INTEGER DEFAULT 0,
                    created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                    last_used_at TEXT,
                    PRIMARY KEY (steamid, build_slot)
                );
                
                CREATE TABLE IF NOT EXISTS player_skills (
                    steamid TEXT NOT NULL,
                    skill_id TEXT NOT NULL,
                    skill_level INTEGER DEFAULT 1,
                    PRIMARY KEY (steamid, skill_id)
                );
                
                CREATE TABLE IF NOT EXISTS talents (
                    steamid TEXT NOT NULL,
                    talent_id TEXT NOT NULL,
                    talent_level INTEGER DEFAULT 1,
                    PRIMARY KEY (steamid, talent_id)
                );
                
                CREATE TABLE IF NOT EXISTS talent_points (
                    steamid TEXT PRIMARY KEY,
                    available_points INTEGER DEFAULT 0,
                    total_earned INTEGER DEFAULT 0
                );
                
                CREATE TABLE IF NOT EXISTS xp_history (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    steamid TEXT NOT NULL,
                    xp_source TEXT NOT NULL,
                    amount REAL NOT NULL,
                    timestamp TEXT DEFAULT CURRENT_TIMESTAMP
                );
                
                CREATE TABLE IF NOT EXISTS skill_mastery (
                    steamid TEXT NOT NULL,
                    skill_id TEXT NOT NULL,
                    kills INTEGER DEFAULT 0,
                    uses INTEGER DEFAULT 0,
                    total_damage REAL DEFAULT 0,
                    escapes INTEGER DEFAULT 0,
                    mastery_level INTEGER DEFAULT 0,
                    PRIMARY KEY (steamid, skill_id)
                );
                
                CREATE INDEX IF NOT EXISTS idx_builds_steamid ON builds(steamid);
                CREATE INDEX IF NOT EXISTS idx_builds_active ON builds(steamid, is_active) WHERE is_active = 1;
                CREATE INDEX IF NOT EXISTS idx_xp_history_steamid ON xp_history(steamid);
                CREATE INDEX IF NOT EXISTS idx_xp_history_timestamp ON xp_history(timestamp);
            ";
        }
    }
}
