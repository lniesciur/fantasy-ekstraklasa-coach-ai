-- Add penalty statistics to player_stats table
-- This migration adds support for detailed penalty tracking:
--   - penalties_scored: successful penalty conversions
--   - penalties_caused: penalties awarded/caused by the player  
--   - penalties_missed: unsuccessful penalty attempts

-- Add new penalty statistic fields to player_stats
ALTER TABLE player_stats
  ADD COLUMN penalties_scored SMALLINT NOT NULL DEFAULT 0,
  ADD COLUMN penalties_caused SMALLINT NOT NULL DEFAULT 0,
  ADD COLUMN penalties_missed SMALLINT NOT NULL DEFAULT 0;

-- Add check constraints to ensure non-negative values
ALTER TABLE player_stats
  ADD CONSTRAINT check_penalties_scored_non_negative CHECK (penalties_scored >= 0),
  ADD CONSTRAINT check_penalties_caused_non_negative CHECK (penalties_caused >= 0),
  ADD CONSTRAINT check_penalties_missed_non_negative CHECK (penalties_missed >= 0);
