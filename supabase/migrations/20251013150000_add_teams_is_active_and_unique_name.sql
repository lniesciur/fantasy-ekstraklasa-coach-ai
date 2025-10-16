/*
 * Add is_active field and unique constraint on name to teams table
 * 
 * Purpose: Support for team creation API endpoint with proper validation
 * - Adds is_active boolean field with default true
 * - Adds unique constraint on team name for data integrity
 * 
 * Changes:
 * - ALTER teams table: add is_active column
 * - ALTER teams table: add unique constraint on name
 */

-- Add is_active column to teams table
ALTER TABLE teams 
ADD COLUMN is_active boolean NOT NULL DEFAULT true;

-- Add unique constraint on team name
ALTER TABLE teams 
ADD CONSTRAINT teams_name_unique UNIQUE (name);

-- Create index on name for faster uniqueness checks during team creation
CREATE INDEX IF NOT EXISTS idx_teams_name ON teams(name);

-- Create index on is_active for filtering active teams
CREATE INDEX IF NOT EXISTS idx_teams_active ON teams(is_active) WHERE is_active = true;
