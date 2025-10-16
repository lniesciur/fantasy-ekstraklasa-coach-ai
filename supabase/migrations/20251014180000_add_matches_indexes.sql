-- Add indexes for matches table to improve query performance
-- This migration adds indexes for status filtering and gameweek queries

-- Create index on status for efficient filtering
CREATE INDEX IF NOT EXISTS idx_matches_status ON matches(status);

-- Create composite index for gameweek and status queries
CREATE INDEX IF NOT EXISTS idx_matches_gameweek_status ON matches(gameweek_id, status);

-- Add comments to document the indexes
COMMENT ON INDEX idx_matches_status IS 'Index for filtering matches by status';
COMMENT ON INDEX idx_matches_gameweek_status IS 'Composite index for gameweek and status queries';
