-- Migration: Add import_logs table for manual data import tracking
-- Created: 2025-10-09

-- Create import_logs table
CREATE TABLE import_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    validation_id UUID,
    filename VARCHAR(255) NOT NULL,
    file_size INTEGER NOT NULL, -- bytes
    import_type VARCHAR(20) NOT NULL DEFAULT 'excel' CHECK (import_type IN ('excel','csv','json')),
    status VARCHAR(20) NOT NULL CHECK (status IN ('validating','validation_failed','validated','importing','completed','failed')),
    overwrite_mode VARCHAR(20) NOT NULL CHECK (overwrite_mode IN ('replace_existing','update_only','create_only')),
    gameweek_id INTEGER REFERENCES gameweeks(id) ON DELETE SET NULL,
    started_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    completed_at TIMESTAMP WITH TIME ZONE,
    validation_results JSONB, -- stores validation errors/warnings
    import_results JSONB, -- stores import statistics
    error_message TEXT,
    players_total INTEGER DEFAULT 0,
    players_imported INTEGER DEFAULT 0,
    players_updated INTEGER DEFAULT 0,
    players_created INTEGER DEFAULT 0,
    warnings_count INTEGER DEFAULT 0,
    errors_count INTEGER DEFAULT 0
);

-- Create indexes
CREATE INDEX idx_import_logs_user_id_status_started_at 
    ON import_logs(user_id, status, started_at);

CREATE INDEX idx_import_logs_validation_id 
    ON import_logs(validation_id) 
    WHERE validation_id IS NOT NULL;

CREATE INDEX idx_import_logs_user_id_gameweek_id 
    ON import_logs(user_id, gameweek_id) 
    WHERE gameweek_id IS NOT NULL;

-- Enable Row Level Security
ALTER TABLE import_logs ENABLE ROW LEVEL SECURITY;

-- Create RLS policy - users can only access their own import logs
CREATE POLICY user_import_logs_policy ON import_logs
    FOR ALL USING (user_id = auth.uid());

-- Add comment
COMMENT ON TABLE import_logs IS 'Tracks manual data imports by users, including validation and execution status';
COMMENT ON COLUMN import_logs.validation_results IS 'JSON containing validation errors and warnings from file upload';
COMMENT ON COLUMN import_logs.import_results IS 'JSON containing detailed import statistics and results';
COMMENT ON COLUMN import_logs.status IS 'Import workflow status: validating -> validated -> importing -> completed/failed';
