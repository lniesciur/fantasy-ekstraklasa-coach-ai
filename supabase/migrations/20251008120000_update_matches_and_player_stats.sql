-- Add status and reschedule_reason to matches
ALTER TABLE matches
  ADD COLUMN status VARCHAR(12) NOT NULL DEFAULT 'scheduled' CHECK (status IN ('scheduled','postponed','cancelled','played')),
  ADD COLUMN reschedule_reason TEXT;

-- Add new statistic fields to player_stats
ALTER TABLE player_stats
  ADD COLUMN lotto_assists   SMALLINT NOT NULL DEFAULT 0,
  ADD COLUMN own_goals       SMALLINT NOT NULL DEFAULT 0,
  ADD COLUMN penalties_saved SMALLINT NOT NULL DEFAULT 0,
  ADD COLUMN penalties_won   SMALLINT NOT NULL DEFAULT 0,
  ADD COLUMN in_team_of_week BOOLEAN   NOT NULL DEFAULT FALSE,
  ADD COLUMN saves           SMALLINT   NOT NULL DEFAULT 0;
