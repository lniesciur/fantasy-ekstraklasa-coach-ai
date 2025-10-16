-- Add RLS policies for matches table to allow public CRUD operations
-- This is needed for the matches API endpoint to work properly

-- Policy for reading matches (public access)
CREATE POLICY matches_select_policy ON matches
  FOR SELECT USING (true);

-- Policy for inserting matches (public access - needed for API)
CREATE POLICY matches_insert_policy ON matches
  FOR INSERT WITH CHECK (true);

-- Policy for updating matches (public access)
CREATE POLICY matches_update_policy ON matches
  FOR UPDATE USING (true);

-- Policy for deleting matches (public access)  
CREATE POLICY matches_delete_policy ON matches
  FOR DELETE USING (true);
