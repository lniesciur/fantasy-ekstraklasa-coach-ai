-- Remove gameweek_id column from player_stats table
-- Gameweek can be determined through match_id -> matches.gameweek_id relationship
-- This eliminates data redundancy and ensures consistency

ALTER TABLE "public"."player_stats" 
DROP COLUMN "gameweek_id";
