-- Add RLS policies for teams table to allow public CRUD operations
-- This is needed for the teams API endpoint to work properly

-- Policy for reading teams (public access)
CREATE POLICY teams_select_policy ON teams
  FOR SELECT USING (true);

-- Policy for inserting teams (public access - needed for API)
CREATE POLICY teams_insert_policy ON teams
  FOR INSERT WITH CHECK (true);

-- Policy for updating teams (public access)
CREATE POLICY teams_update_policy ON teams
  FOR UPDATE USING (true);

-- Policy for deleting teams (public access)  
CREATE POLICY teams_delete_policy ON teams
  FOR DELETE USING (true);
