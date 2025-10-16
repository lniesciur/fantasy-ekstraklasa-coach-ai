/*
 * Add health_status tracking to player_stats table
 * 
 * Purpose: Enable analysis of player health status per gameweek
 * 
 * Changes:
 * - Add health_status column to player_stats table
 * - Add index for efficient health status queries
 * - Update RLS policies if needed
 * 
 * This allows tracking how player health status changes over time
 * and enables analysis of health trends per gameweek.
 */

-- Add health_status column to player_stats table
ALTER TABLE player_stats 
ADD COLUMN health_status varchar(10) check (health_status in ('Pewny','Wątpliwy','Nie zagra'));

-- Add comment to document the purpose
COMMENT ON COLUMN player_stats.health_status IS 'Player health status for this specific gameweek, allowing historical health analysis';

-- Add index for efficient health status queries
CREATE INDEX idx_player_stats_health_status ON player_stats(health_status);

-- Add composite index for health status analysis per gameweek
CREATE INDEX idx_player_stats_gameweek_health ON player_stats(gameweek_id, health_status);

-- Add composite index for player health history analysis
CREATE INDEX idx_player_stats_player_health ON player_stats(player_id, gameweek_id, health_status);
