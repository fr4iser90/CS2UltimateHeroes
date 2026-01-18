# 💾 Plan: Database & Repositories

## 📋 Zweck

Database Layer verwaltet:
- SQLite Connection
- Database Schema
- Repository Pattern für Data Access

## 🔗 Abhängigkeiten

- `Microsoft.Data.Sqlite` (NuGet Package) ✅
- `Dapper` (NuGet Package) ✅

## 📐 Database Schema

### Schema.sql

```sql
-- Players Table
CREATE TABLE IF NOT EXISTS players (
    steamid TEXT PRIMARY KEY,
    hero_core_id TEXT,
    hero_level INTEGER DEFAULT 1,
    current_xp REAL DEFAULT 0,
    xp_to_next_level REAL DEFAULT 100,
    current_role TEXT,
    last_updated TEXT DEFAULT CURRENT_TIMESTAMP
);

-- Builds Table
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

-- Player Skills Table
CREATE TABLE IF NOT EXISTS player_skills (
    steamid TEXT NOT NULL,
    skill_id TEXT NOT NULL,
    skill_level INTEGER DEFAULT 1,
    PRIMARY KEY (steamid, skill_id),
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

-- Talents Table
CREATE TABLE IF NOT EXISTS talents (
    steamid TEXT NOT NULL,
    talent_id TEXT NOT NULL,
    talent_level INTEGER DEFAULT 1,
    PRIMARY KEY (steamid, talent_id),
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

-- Talent Points Table
CREATE TABLE IF NOT EXISTS talent_points (
    steamid TEXT PRIMARY KEY,
    available_points INTEGER DEFAULT 0,
    total_earned INTEGER DEFAULT 0,
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

-- XP History Table
CREATE TABLE IF NOT EXISTS xp_history (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    steamid TEXT NOT NULL,
    xp_source TEXT NOT NULL,
    amount REAL NOT NULL,
    timestamp TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

-- Skill Mastery Table
CREATE TABLE IF NOT EXISTS skill_mastery (
    steamid TEXT NOT NULL,
    skill_id TEXT NOT NULL,
    kills INTEGER DEFAULT 0,
    uses INTEGER DEFAULT 0,
    total_damage REAL DEFAULT 0,
    escapes INTEGER DEFAULT 0,
    mastery_level INTEGER DEFAULT 0,
    PRIMARY KEY (steamid, skill_id),
    FOREIGN KEY (steamid) REFERENCES players(steamid)
);

-- Indexes
CREATE INDEX IF NOT EXISTS idx_builds_steamid ON builds(steamid);
CREATE INDEX IF NOT EXISTS idx_builds_active ON builds(steamid, is_active) WHERE is_active = 1;
CREATE INDEX IF NOT EXISTS idx_xp_history_steamid ON xp_history(steamid);
CREATE INDEX IF NOT EXISTS idx_xp_history_timestamp ON xp_history(timestamp);
```

## 🎯 Implementierung

### Datei: `Infrastructure/Database/Database.cs`

```csharp
namespace UltimateHeroes.Infrastructure.Database
{
    public class Database
    {
        private readonly string _connectionString;
        
        public Database(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }
        
        public IDbConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }
        
        private void InitializeDatabase()
        {
            using var connection = GetConnection();
            connection.Open();
            
            // Execute Schema
            var schema = File.ReadAllText("Infrastructure/Database/Schema.sql");
            connection.Execute(schema);
        }
    }
}
```

### Repository Interfaces

```csharp
namespace UltimateHeroes.Infrastructure.Database
{
    public interface IPlayerRepository
    {
        UltimatePlayer? GetPlayer(string steamId);
        void SavePlayer(UltimatePlayer player);
        void DeletePlayer(string steamId);
    }
    
    public interface IBuildRepository
    {
        Build? GetBuild(string steamId, int buildSlot);
        List<Build> GetPlayerBuilds(string steamId);
        Build? GetActiveBuild(string steamId);
        void SaveBuild(Build build);
        void DeleteBuild(string steamId, int buildSlot);
    }
    
    public interface ITalentRepository
    {
        PlayerTalents? GetPlayerTalents(string steamId);
        void SavePlayerTalents(PlayerTalents playerTalents);
    }
    
    public interface IXpRepository
    {
        void SaveXpHistory(XpHistory history);
        List<XpHistory> GetXpHistory(string steamId, int limit = 50);
    }
    
    public interface IMasteryRepository
    {
        SkillMastery? GetSkillMastery(string steamId, string skillId);
        List<SkillMastery> GetPlayerMasteries(string steamId);
        void SaveSkillMastery(SkillMastery mastery);
    }
}
```

### Repository Implementations

**Datei**: `Infrastructure/Database/Repositories/PlayerRepository.cs`

```csharp
namespace UltimateHeroes.Infrastructure.Database.Repositories
{
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
            
            connection.Execute(@"
                INSERT OR REPLACE INTO players 
                (steamid, hero_core_id, hero_level, current_xp, xp_to_next_level, current_role, last_updated)
                VALUES 
                (@SteamId, @HeroCoreId, @HeroLevel, @CurrentXp, @XpToNextLevel, @CurrentRole, @LastUpdated)",
                new
                {
                    player.SteamId,
                    HeroCoreId = player.CurrentHero?.Id,
                    player.HeroLevel,
                    player.CurrentXp,
                    player.XpToNextLevel,
                    CurrentRole = player.CurrentRole.ToString(),
                    LastUpdated = DateTime.UtcNow
                }
            );
            
            // Save Skills
            foreach (var skillLevel in player.SkillLevels)
            {
                connection.Execute(@"
                    INSERT OR REPLACE INTO player_skills 
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
            connection.Execute("DELETE FROM players WHERE steamid = @SteamId", new { SteamId = steamId });
        }
    }
}
```

**Datei**: `Infrastructure/Database/Repositories/BuildRepository.cs`

```csharp
namespace UltimateHeroes.Infrastructure.Database.Repositories
{
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
            
            var build = connection.QueryFirstOrDefault<Build>(
                "SELECT * FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                new { SteamId = steamId, BuildSlot = buildSlot }
            );
            
            if (build != null)
            {
                // Parse SkillIds
                build.SkillIds = new List<string>();
                if (!string.IsNullOrEmpty(build.Skill1Id)) build.SkillIds.Add(build.Skill1Id);
                if (!string.IsNullOrEmpty(build.Skill2Id)) build.SkillIds.Add(build.Skill2Id);
                if (!string.IsNullOrEmpty(build.Skill3Id)) build.SkillIds.Add(build.Skill3Id);
            }
            
            return build;
        }
        
        public List<Build> GetPlayerBuilds(string steamId)
        {
            using var connection = _database.GetConnection();
            
            var builds = connection.Query<Build>(
                "SELECT * FROM builds WHERE steamid = @SteamId ORDER BY build_slot",
                new { SteamId = steamId }
            ).ToList();
            
            foreach (var build in builds)
            {
                build.SkillIds = new List<string>();
                if (!string.IsNullOrEmpty(build.Skill1Id)) build.SkillIds.Add(build.Skill1Id);
                if (!string.IsNullOrEmpty(build.Skill2Id)) build.SkillIds.Add(build.Skill2Id);
                if (!string.IsNullOrEmpty(build.Skill3Id)) build.SkillIds.Add(build.Skill3Id);
            }
            
            return builds;
        }
        
        public Build? GetActiveBuild(string steamId)
        {
            using var connection = _database.GetConnection();
            
            var build = connection.QueryFirstOrDefault<Build>(
                "SELECT * FROM builds WHERE steamid = @SteamId AND is_active = 1",
                new { SteamId = steamId }
            );
            
            if (build != null)
            {
                build.SkillIds = new List<string>();
                if (!string.IsNullOrEmpty(build.Skill1Id)) build.SkillIds.Add(build.Skill1Id);
                if (!string.IsNullOrEmpty(build.Skill2Id)) build.SkillIds.Add(build.Skill2Id);
                if (!string.IsNullOrEmpty(build.Skill3Id)) build.SkillIds.Add(build.Skill3Id);
            }
            
            return build;
        }
        
        public void SaveBuild(Build build)
        {
            using var connection = _database.GetConnection();
            
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
            connection.Execute(
                "DELETE FROM builds WHERE steamid = @SteamId AND build_slot = @BuildSlot",
                new { SteamId = steamId, BuildSlot = buildSlot }
            );
        }
    }
}
```

## 🔄 Integration

1. **Plugin Load**: Initialisiert Database
2. **Services**: Nutzen Repositories für Data Access
3. **Event Handlers**: Speichern Player States

## ✅ Tests

- Database wird korrekt initialisiert
- Repositories funktionieren
- Data wird korrekt gespeichert/geladen
- Transactions funktionieren

## 📝 Nächste Schritte

1. ✅ Database.cs implementieren
2. ✅ Schema.sql erstellen
3. ✅ Repository Interfaces definieren
4. ✅ Repository Implementations
5. ✅ Integration mit Services
