-- Add RLS policies for gameweeks table to allow public CRUD operations
-- This is needed for the gameweeks API endpoint to work properly

-- Policy for reading gameweeks (public access)
CREATE POLICY gameweeks_select_policy ON gameweeks
  FOR SELECT USING (true);

-- Policy for inserting gameweeks (public access - needed for API)
CREATE POLICY gameweeks_insert_policy ON gameweeks
  FOR INSERT WITH CHECK (true);

-- Policy for updating gameweeks (public access)
CREATE POLICY gameweeks_update_policy ON gameweeks
  FOR UPDATE USING (true);

-- Policy for deleting gameweeks (public access)  
CREATE POLICY gameweeks_delete_policy ON gameweeks
  FOR DELETE USING (true);
