/*
 * Add constraints for players table to support Create Player API endpoint
 * 
 * Purpose: Ensure data integrity for player creation with proper validation
 * - Adds unique constraint on (name, team_id) to prevent duplicate players in same team
 * - Adds check constraints for price range validation
 * - Adds indexes for performance optimization
 * 
 * Changes:
 * - ALTER players table: add unique constraint on (name, team_id)
 * - ALTER players table: add check constraint for price range
 * - CREATE indexes for performance optimization
 */

-- Add unique constraint to prevent duplicate player names within the same team
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'players_name_team_unique' 
        AND table_name = 'players'
    ) THEN
        ALTER TABLE players 
        ADD CONSTRAINT players_name_team_unique UNIQUE (name, team_id);
    END IF;
END $$;

-- Add check constraint for price range (0 < price <= 50,000,000)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'players_price_range' 
        AND table_name = 'players'
    ) THEN
        ALTER TABLE players 
        ADD CONSTRAINT players_price_range CHECK (price > 0 AND price <= 50000000);
    END IF;
END $$;

-- Add check constraint to ensure name is not empty (additional safety)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'players_name_not_empty' 
        AND table_name = 'players'
    ) THEN
        ALTER TABLE players 
        ADD CONSTRAINT players_name_not_empty CHECK (length(trim(name)) > 0);
    END IF;
END $$;

-- Create index on (name, team_id) for faster uniqueness checks during player creation
CREATE INDEX IF NOT EXISTS idx_players_name_team ON players(name, team_id);

-- Create index on team_id for faster team existence checks
CREATE INDEX IF NOT EXISTS idx_players_team_id ON players(team_id);

-- Create index on position for filtering players by position
CREATE INDEX IF NOT EXISTS idx_players_position ON players(position);

-- Create index on price for sorting and filtering by price range
CREATE INDEX IF NOT EXISTS idx_players_price ON players(price);

-- Create index on health_status for filtering by player availability
CREATE INDEX IF NOT EXISTS idx_players_health_status ON players(health_status);

-- Create composite index for common player queries (team + position)
CREATE INDEX IF NOT EXISTS idx_players_team_position ON players(team_id, position);

-- Create composite index for price-based queries (team + price range)
CREATE INDEX IF NOT EXISTS idx_players_team_price ON players(team_id, price);

-- Add comment to document the constraints
COMMENT ON CONSTRAINT players_name_team_unique ON players IS 'Ensures player names are unique within each team';
COMMENT ON CONSTRAINT players_price_range ON players IS 'Ensures player price is within valid range (0 < price <= 50M)';
COMMENT ON CONSTRAINT players_name_not_empty ON players IS 'Ensures player name is not empty or whitespace only';
