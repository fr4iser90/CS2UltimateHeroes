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
            
            // Execute Schema
            var schemaPath = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "",
                "Infrastructure", "Database", "Schema.sql"
            );
            
            // Fallback: Embedded Schema
            if (!File.Exists(schemaPath))
            {
                // Try to read from embedded resource or use inline schema
                var schema = GetEmbeddedSchema();
                connection.Execute(schema);
            }
            else
            {
                var schema = File.ReadAllText(schemaPath);
                connection.Execute(schema);
            }
        }
        
        private string GetEmbeddedSchema()
        {
            // Inline schema as fallback
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
                
                CREATE INDEX IF NOT EXISTS idx_builds_steamid ON builds(steamid);
                CREATE INDEX IF NOT EXISTS idx_builds_active ON builds(steamid, is_active) WHERE is_active = 1;
            ";
        }
    }
}
