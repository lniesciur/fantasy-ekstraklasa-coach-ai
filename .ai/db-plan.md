# Database Schema for Fantasy Ekstraklasa Optimizer

## 1. Tables

### 1.1 teams

- **id** SERIAL PRIMARY KEY
- **name** TEXT NOT NULL UNIQUE
- **short_code** VARCHAR(10) NOT NULL UNIQUE
- **crest_url** TEXT
- **is_active** BOOLEAN NOT NULL DEFAULT TRUE

### 1.2 players

- **id** SERIAL PRIMARY KEY
- **name** TEXT NOT NULL
- **team_id** INTEGER NOT NULL REFERENCES teams(id) ON DELETE RESTRICT
- **position** VARCHAR(3) NOT NULL CHECK (position IN ('GK','DEF','MID','FWD'))
- **price** NUMERIC(6,2) NOT NULL CHECK (price > 0 AND price <= 50000000)
- **health_status** VARCHAR(10) NOT NULL CHECK (health_status IN ('Pewny','Wątpliwy','Nie zagra'))
- **created_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **updated_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **UNIQUE** (name, team_id)
- **CHECK** (length(trim(name)) > 0)

### 1.3 gameweeks

- **id** SERIAL PRIMARY KEY
- **number** INTEGER NOT NULL UNIQUE
- **start_date** DATE NOT NULL
- **end_date** DATE NOT NULL

### 1.4 matches

- **id** SERIAL PRIMARY KEY
- **gameweek_id** INTEGER NOT NULL REFERENCES gameweeks(id) ON DELETE CASCADE
- **home_team_id** INTEGER NOT NULL REFERENCES teams(id) ON DELETE RESTRICT
- **away_team_id** INTEGER NOT NULL REFERENCES teams(id) ON DELETE RESTRICT
- **match_date** TIMESTAMP WITH TIME ZONE NOT NULL
- **home_score** SMALLINT
- **away_score** SMALLINT
- **status** VARCHAR(12) NOT NULL DEFAULT 'scheduled' CHECK (status IN ('scheduled','postponed','cancelled','played'))
- **reschedule_reason** TEXT

### 1.5 player_stats

- **id** SERIAL PRIMARY KEY
- **player_id** INTEGER NOT NULL REFERENCES players(id) ON DELETE CASCADE
- **gameweek_id** INTEGER NOT NULL REFERENCES gameweeks(id) ON DELETE CASCADE
- **match_id** INTEGER REFERENCES matches(id) ON DELETE SET NULL
- **fantasy_points** SMALLINT NOT NULL
- **minutes_played** SMALLINT NOT NULL
- **goals** SMALLINT NOT NULL
- **assists** SMALLINT NOT NULL
- **yellow_cards** SMALLINT NOT NULL
- **red_cards** SMALLINT NOT NULL
- **price** NUMERIC(6,2) NOT NULL
- **predicted_start** BOOLEAN DEFAULT FALSE
- **lotto_assists** SMALLINT NOT NULL DEFAULT 0
- **own_goals** SMALLINT NOT NULL DEFAULT 0
- **penalties_saved** SMALLINT NOT NULL DEFAULT 0
- **penalties_won** SMALLINT NOT NULL DEFAULT 0
- **penalties_scored** SMALLINT NOT NULL DEFAULT 0
- **penalties_caused** SMALLINT NOT NULL DEFAULT 0
- **penalties_missed** SMALLINT NOT NULL DEFAULT 0
- **in_team_of_week** BOOLEAN NOT NULL DEFAULT FALSE
- **saves** SMALLINT NOT NULL DEFAULT 0
- **health_status** VARCHAR(10) CHECK (health_status IN ('Pewny','Wątpliwy','Nie zagra'))
- **created_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **updated_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **UNIQUE** (player_id, gameweek_id)

### 1.6 users

_Supabase Auth handles core user record; this table holds profile metadata._

- **id** UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE
- **created_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **updated_at** TIMESTAMP WITH TIME ZONE DEFAULT now()

### 1.7 lineups

- **id** UUID PRIMARY KEY DEFAULT gen_random_uuid()
- **user_id** UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE
- **gameweek_id** INTEGER NOT NULL REFERENCES gameweeks(id) ON DELETE CASCADE
- **name** VARCHAR(50) NOT NULL
- **is_active** BOOLEAN NOT NULL DEFAULT TRUE
- **total_cost** NUMERIC(6,2) NOT NULL
- **created_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **updated_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **UNIQUE** (user_id, gameweek_id, name)

### 1.8 lineup_players

- **lineup_id** UUID NOT NULL REFERENCES lineups(id) ON DELETE CASCADE
- **player_id** INTEGER NOT NULL REFERENCES players(id) ON DELETE RESTRICT
- **role** VARCHAR(10) NOT NULL CHECK (role IN ('starting','bench'))
- **is_captain** BOOLEAN NOT NULL DEFAULT FALSE
- **is_vice** BOOLEAN NOT NULL DEFAULT FALSE
- **is_locked** BOOLEAN NOT NULL DEFAULT FALSE
- **PRIMARY KEY** (lineup_id, player_id)

### 1.9 bonuses

- **id** SERIAL PRIMARY KEY
- **name** VARCHAR(30) NOT NULL UNIQUE
- **description** TEXT

### 1.10 lineup_bonuses

- **id** SERIAL PRIMARY KEY
- **lineup_id** UUID NOT NULL REFERENCES lineups(id) ON DELETE CASCADE
- **bonus_id** INTEGER NOT NULL REFERENCES bonuses(id) ON DELETE RESTRICT
- **applied_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **UNIQUE** (lineup_id, bonus_id)

### 1.11 transfer_tips

- **id** SERIAL PRIMARY KEY
- **user_id** UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE
- **gameweek_id** INTEGER NOT NULL REFERENCES gameweeks(id) ON DELETE CASCADE
- **out_player_id** INTEGER NOT NULL REFERENCES players(id) ON DELETE RESTRICT
- **in_player_id** INTEGER NOT NULL REFERENCES players(id) ON DELETE RESTRICT
- **reason** TEXT NOT NULL
- **status** VARCHAR(10) NOT NULL DEFAULT 'pending' CHECK (status IN ('pending','applied','rejected'))
- **created_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **applied_at** TIMESTAMP WITH TIME ZONE

### 1.12 tutorial_status

- **user_id** UUID PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE
- **last_step** SMALLINT NOT NULL DEFAULT 0
- **skipped** BOOLEAN NOT NULL DEFAULT FALSE
- **updated_at** TIMESTAMP WITH TIME ZONE DEFAULT now()

### 1.13 generation_logs

- **id** SERIAL PRIMARY KEY
- **user_id** UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE
- **lineup_id** UUID REFERENCES lineups(id) ON DELETE SET NULL
- **gameweek_id** INTEGER REFERENCES gameweeks(id) ON DELETE SET NULL
- **model** TEXT NOT NULL
- **generation_time** INTEGER NOT NULL -- seconds
- **success** BOOLEAN NOT NULL
- **error_message** TEXT
- **created_at** TIMESTAMP WITH TIME ZONE DEFAULT now()

### 1.14 scrape_runs

- **id** SERIAL PRIMARY KEY
- **run_type** VARCHAR(20) NOT NULL CHECK (run_type IN ('daily_main','pre_gameweek','post_gameweek','manual'))
- **gameweek_id** INTEGER REFERENCES gameweeks(id) ON DELETE SET NULL
- **started_at** TIMESTAMP WITH TIME ZONE NOT NULL
- **finished_at** TIMESTAMP WITH TIME ZONE
- **success** BOOLEAN NOT NULL
- **error_message** TEXT

### 1.15 import_logs

- **id** UUID PRIMARY KEY DEFAULT gen_random_uuid()
- **user_id** UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE
- **validation_id** UUID
- **filename** VARCHAR(255) NOT NULL
- **file_size** INTEGER NOT NULL -- bytes
- **import_type** VARCHAR(20) NOT NULL DEFAULT 'excel' CHECK (import_type IN ('excel','csv','json'))
- **status** VARCHAR(20) NOT NULL CHECK (status IN ('validating','validation_failed','validated','importing','completed','failed'))
- **overwrite_mode** VARCHAR(20) NOT NULL CHECK (overwrite_mode IN ('replace_existing','update_only','create_only'))
- **gameweek_id** INTEGER REFERENCES gameweeks(id) ON DELETE SET NULL
- **started_at** TIMESTAMP WITH TIME ZONE DEFAULT now()
- **completed_at** TIMESTAMP WITH TIME ZONE
- **validation_results** JSONB -- stores validation errors/warnings
- **import_results** JSONB -- stores import statistics
- **error_message** TEXT
- **players_total** INTEGER DEFAULT 0
- **players_imported** INTEGER DEFAULT 0
- **players_updated** INTEGER DEFAULT 0
- **players_created** INTEGER DEFAULT 0
- **warnings_count** INTEGER DEFAULT 0
- **errors_count** INTEGER DEFAULT 0

---

## 2. Relationships

- **teams** 1—\* **players**
- **teams** 1—\* **matches** (home_team_id, away_team_id)
- **gameweeks** 1—\* **matches**
- **gameweeks** 1—\* **player_stats**
- **gameweeks** 1—\* **lineups**
- **gameweeks** 1—\* **transfer_tips**
- **gameweeks** 1—\* **import_logs**
- **players** 1—\* **player_stats**
- **users** 1—\* **lineups**
- **users** 1—\* **transfer_tips**
- **users** 1—\* **import_logs**
- **lineups** 1—\* **lineup_players**
- **lineups** 1—\* **lineup_bonuses**
- **users** 1—1 **tutorial_status**
- **users** 1—\* **generation_logs**

Many-to-many is resolved by **lineup_players** (lineups ↔ players).

---

## 3. Indexes

- B-TREE on all foreign keys:
  - `players(team_id)`, `player_stats(player_id)`, `player_stats(gameweek_id)`, …
- UNIQUE B-TREE:
  - `gameweeks(number)`, `(player_stats.player_id, player_stats.gameweek_id)`, `(lineups.user_id, lineups.gameweek_id, lineups.name)`, `(tutorial_status.user_id)`
- GIN TRIGRAM on `players(name)` for fast search:
  ```sql
  CREATE EXTENSION IF NOT EXISTS pg_trgm;
  CREATE INDEX idx_players_name_trgm ON players USING GIN (name gin_trgm_ops);
  ```
- Composite index on `transfer_tips(user_id, gameweek_id)`
- Composite index on `import_logs(user_id, status, started_at)`
- Index on `import_logs(validation_id)` for quick validation lookup
- Index on `import_logs(user_id, gameweek_id)` for gameweek-specific imports
- Teams indexes:
  - `idx_teams_name` on `teams(name)` for uniqueness checks
  - `idx_teams_active` on `teams(is_active)` for active teams filtering
- Players indexes:
  - `idx_players_name_team` on `players(name, team_id)` for uniqueness checks
  - `idx_players_position` on `players(position)` for position filtering
  - `idx_players_price` on `players(price)` for price sorting
  - `idx_players_health_status` on `players(health_status)` for availability filtering
  - `idx_players_team_position` on `players(team_id, position)` for team position queries
  - `idx_players_team_price` on `players(team_id, price)` for price-based queries
- Matches indexes:
  - `idx_matches_status` on `matches(status)` for status filtering
  - `idx_matches_gameweek_status` on `matches(gameweek_id, status)` for gameweek status queries
- Health status analysis indexes:
  - `idx_player_stats_health_status` on `player_stats(health_status)`
  - `idx_player_stats_gameweek_health` on `player_stats(gameweek_id, health_status)`
  - `idx_player_stats_player_health` on `player_stats(player_id, gameweek_id, health_status)`
- Materialized views (refreshed on schedule) for dashboards, e.g. `mv_player_leaderboard`, `mv_user_history`

---

## 4. Row-Level Security (RLS) Policies

Enable RLS on all tables with appropriate policies:

```sql
-- Core tables (public access)
ALTER TABLE teams           ENABLE ROW LEVEL SECURITY;
ALTER TABLE gameweeks       ENABLE ROW LEVEL SECURITY;
ALTER TABLE bonuses         ENABLE ROW LEVEL SECURITY;
ALTER TABLE scrape_runs     ENABLE ROW LEVEL SECURITY;
ALTER TABLE players         ENABLE ROW LEVEL SECURITY;
ALTER TABLE matches         ENABLE ROW LEVEL SECURITY;
ALTER TABLE player_stats    ENABLE ROW LEVEL SECURITY;

-- User-specific tables  
ALTER TABLE users           ENABLE ROW LEVEL SECURITY;
ALTER TABLE lineups         ENABLE ROW LEVEL SECURITY;
ALTER TABLE lineup_players  ENABLE ROW LEVEL SECURITY;
ALTER TABLE lineup_bonuses  ENABLE ROW LEVEL SECURITY;
ALTER TABLE transfer_tips   ENABLE ROW LEVEL SECURITY;
ALTER TABLE tutorial_status ENABLE ROW LEVEL SECURITY;
ALTER TABLE generation_logs ENABLE ROW LEVEL SECURITY;
ALTER TABLE import_logs     ENABLE ROW LEVEL SECURITY;
```

**Public Access Tables:** teams, gameweeks, bonuses, players, matches, player_stats have policies allowing public read access and API operations.

**Admin-Only Tables:** scrape_runs has no public policies (admin/system access only).

**User-Specific Tables:** users, lineups, lineup_players, lineup_bonuses, transfer_tips, tutorial_status, generation_logs, import_logs have policies restricting access to the owning user only.

Policy example (only owner can access):

```sql
CREATE POLICY user_is_owner ON lineups
  FOR ALL USING (user_id = auth.uid());
-- Repeat analogous policies on other tables, replacing table_name and user_id column.
```

Admin role may have separate policies granting read on analytic tables.

---

## 5. Additional Notes

- **Normalization**: 3NF; all multi-value and lookup relationships abstracted.
- **Transactions**: Wrap creation of a lineup + its children (lineup_players, lineup_bonuses) inside a single transaction.
- **Partitioning**: In future, partition `player_stats` by `gameweek_id` for scale.
- **Computed fields**: "form" and "match difficulty" calculated on-the-fly or via materialized views rather than stored.
- **Health Status Tracking**: Player health status is now tracked both in `players` table (current status) and per gameweek in `player_stats.health_status`, enabling historical health analysis and trend identification.
- **Enhanced Statistics**: Extended player statistics including penalty details (scored, caused, missed, saved, won), lotto assists, own goals, saves, and team of the week tracking.
- **Data Integrity**: Comprehensive constraints including unique player names per team, price range validation (0 < price ≤ 50M), non-empty name validation, and proper health status checks.
- **Import Management**: Full import lifecycle tracking via `import_logs` table with validation, progress monitoring, and detailed result statistics.
- **Team Management**: Team activation/deactivation support with `is_active` flag and unique name constraints for data consistency.
- **Match Status Tracking**: Match status progression (scheduled → postponed/cancelled → played) with reschedule reason tracking.
- **Performance Optimization**: Extensive indexing strategy covering all common query patterns including team filtering, player searches, price ranges, health status, and composite indexes for complex queries.
- **Audit & Logging**: `scrape_runs`, `generation_logs`, and `import_logs` tables provide comprehensive operational visibility and user activity tracking.
- **Supabase Integration**: Full RLS implementation with public access for core data and user-restricted access for personal data, leveraging built-in auth.users system.

---
