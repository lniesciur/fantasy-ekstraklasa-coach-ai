-- Migration: Add security features for player stats
-- This migration adds logging table, safe function, and trigger for player stats

-- Create logging table for missing players
CREATE TABLE IF NOT EXISTS missing_players_log (
    id SERIAL PRIMARY KEY,
    player_name TEXT NOT NULL,
    attempted_at TIMESTAMP DEFAULT NOW(),
    reason TEXT DEFAULT 'Player not found in database'
);

-- Function to safely insert player stats with player existence check
CREATE OR REPLACE FUNCTION insert_player_stats_safe(
    p_player_name TEXT,
    p_minutes_played INTEGER,
    p_goals INTEGER,
    p_assists INTEGER,
    p_yellow_cards INTEGER,
    p_red_cards INTEGER,
    p_predicted_start BOOLEAN,
    p_lotto_assists INTEGER,
    p_own_goals INTEGER,
    p_penalties_saved INTEGER,
    p_penalties_won INTEGER,
    p_in_team_of_week BOOLEAN,
    p_saves INTEGER,
    p_penalties_scored INTEGER,
    p_penalties_caused INTEGER,
    p_penalties_missed INTEGER,
    p_health_status TEXT,
    p_fantasy_points INTEGER
) RETURNS VOID AS $$
DECLARE
    player_exists BOOLEAN;
    player_id_val INTEGER;
    player_price_val DECIMAL;
BEGIN
    -- Check if player exists
    SELECT EXISTS(SELECT 1 FROM players WHERE name LIKE '%' || p_player_name || '%'), 
           (SELECT id FROM players WHERE name LIKE '%' || p_player_name || '%' LIMIT 1),
           (SELECT price FROM players WHERE name LIKE '%' || p_player_name || '%' LIMIT 1)
    INTO player_exists, player_id_val, player_price_val;
    
    IF player_exists AND player_id_val IS NOT NULL THEN
        -- Insert player stats
        INSERT INTO player_stats (
            player_id, 
            minutes_played, 
            goals, assists,
            yellow_cards, 
            red_cards, 
            price, 
            predicted_start, 
            lotto_assists, 
            own_goals,
            penalties_saved, 
            penalties_won, 
            in_team_of_week, 
            saves, 
            penalties_scored,
            penalties_caused, 
            penalties_missed, 
            health_status,
            fantasy_points
        ) VALUES (
            player_id_val,
            p_minutes_played,
            p_goals,
            p_assists,
            p_yellow_cards,
            p_red_cards,
            player_price_val,
            p_predicted_start,
            p_lotto_assists,
            p_own_goals,
            p_penalties_saved,
            p_penalties_won,
            p_in_team_of_week,
            p_saves,
            p_penalties_scored,
            p_penalties_caused,
            p_penalties_missed,
            p_health_status,
            p_fantasy_points
        );
    ELSE
        -- Log missing player
        INSERT INTO missing_players_log (player_name, reason) 
        VALUES (p_player_name, 'Player not found in database');
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Create a trigger function to prevent inserting player stats for non-existent players
CREATE OR REPLACE FUNCTION check_player_exists_before_insert()
RETURNS TRIGGER AS $$
BEGIN
    -- Check if the player exists
    IF NOT EXISTS (SELECT 1 FROM players WHERE id = NEW.player_id) THEN
        -- Log the missing player
        INSERT INTO missing_players_log (player_name, reason) 
        VALUES ('Player ID: ' || NEW.player_id, 'Player not found in database');
        
        -- Return NULL to prevent the insert
        RETURN NULL;
    END IF;
    
    -- If player exists, allow the insert
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create the trigger
DROP TRIGGER IF EXISTS check_player_exists_trigger ON player_stats;
CREATE TRIGGER check_player_exists_trigger
    BEFORE INSERT ON player_stats
    FOR EACH ROW
    EXECUTE FUNCTION check_player_exists_before_insert();
