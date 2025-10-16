-- Add performance indexes for common queries in API endpoints

-- Index for teams filtering/sorting (name, short_code unique lookups)
CREATE INDEX IF NOT EXISTS idx_teams_name ON teams (lower(name));
CREATE INDEX IF NOT EXISTS idx_teams_short_code ON teams (short_code);
CREATE INDEX IF NOT EXISTS idx_teams_is_active ON teams (is_active);

-- Index for gameweeks filtering/sorting (number, dates, status)
CREATE INDEX IF NOT EXISTS idx_gameweeks_number ON gameweeks (number);
CREATE INDEX IF NOT EXISTS idx_gameweeks_start_date ON gameweeks (start_date);
CREATE INDEX IF NOT EXISTS idx_gameweeks_status ON gameweeks (status);

-- Index for matches (core filtering: date, gameweek, teams, status, pagination)
CREATE INDEX IF NOT EXISTS idx_matches_match_date ON matches (match_date);
CREATE INDEX IF NOT EXISTS idx_matches_gameweek_id ON matches (gameweek_id);
CREATE INDEX IF NOT EXISTS idx_matches_home_team_id ON matches (home_team_id);
CREATE INDEX IF NOT EXISTS idx_matches_away_team_id ON matches (away_team_id);
CREATE INDEX IF NOT EXISTS idx_matches_status ON matches (status);
-- Composite for common list queries
CREATE INDEX IF NOT EXISTS idx_matches_gameweek_status_date ON matches (gameweek_id, status, match_date);

-- Index for players (future endpoints: team, position, price filtering/sorting)
CREATE INDEX IF NOT EXISTS idx_players_team_id ON players (team_id);
CREATE INDEX IF NOT EXISTS idx_players_position ON players (position);
CREATE INDEX IF NOT EXISTS idx_players_price ON players (price);
CREATE INDEX IF NOT EXISTS idx_players_health_status ON players (health_status);

-- Additional for joins (e.g., player_stats with gameweek/player)
CREATE INDEX IF NOT EXISTS idx_player_stats_gameweek_id ON player_stats (gameweek_id);
CREATE INDEX IF NOT EXISTS idx_player_stats_player_id ON player_stats (player_id);

-- Note: These indexes improve query performance for API endpoints like /api/matches, /api/teams, /api/gameweeks.
-- Run: supabase db diff --local > new_migration.sql to verify.
