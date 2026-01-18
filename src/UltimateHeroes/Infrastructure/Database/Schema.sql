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
    -- Active Skills (Max 3)
    active_skill1_id TEXT,
    active_skill2_id TEXT,
    active_skill3_id TEXT,
    -- Ultimate Skill (Max 1, optional)
    ultimate_skill_id TEXT,
    -- Passive Skills (Max 2)
    passive_skill1_id TEXT,
    passive_skill2_id TEXT,
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
