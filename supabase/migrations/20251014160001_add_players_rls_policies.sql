/*
 * Add Row Level Security policies for players table admin operations
 * 
 * Purpose: Secure player creation and management with proper authorization
 * - Allows admin users to create, update, and delete players
 * - Maintains public read access for all users
 * - Prevents unauthorized player modifications
 * 
 * Changes:
 * - CREATE policy for admin player insert operations
 * - CREATE policy for admin player update operations  
 * - CREATE policy for admin player delete operations
 * - Maintain existing public read policies
 */

-- Policy for admin users to insert new players
-- This allows authenticated users with admin role to create players
CREATE POLICY "players_insert_admin" ON players 
FOR INSERT TO authenticated 
WITH CHECK (
  EXISTS (
    SELECT 1 FROM auth.users 
    WHERE auth.users.id = auth.uid() 
    AND auth.users.raw_user_meta_data->>'role' = 'admin'
  )
);

-- Policy for admin users to update existing players
-- This allows authenticated users with admin role to modify player data
CREATE POLICY "players_update_admin" ON players 
FOR UPDATE TO authenticated 
USING (
  EXISTS (
    SELECT 1 FROM auth.users 
    WHERE auth.users.id = auth.uid() 
    AND auth.users.raw_user_meta_data->>'role' = 'admin'
  )
)
WITH CHECK (
  EXISTS (
    SELECT 1 FROM auth.users 
    WHERE auth.users.id = auth.uid() 
    AND auth.users.raw_user_meta_data->>'role' = 'admin'
  )
);

-- Policy for admin users to delete players
-- This allows authenticated users with admin role to remove players
CREATE POLICY "players_delete_admin" ON players 
FOR DELETE TO authenticated 
USING (
  EXISTS (
    SELECT 1 FROM auth.users 
    WHERE auth.users.id = auth.uid() 
    AND auth.users.raw_user_meta_data->>'role' = 'admin'
  )
);

-- Add comments to document the policies
COMMENT ON POLICY "players_insert_admin" ON players IS 'Allows admin users to create new players';
COMMENT ON POLICY "players_update_admin" ON players IS 'Allows admin users to update existing players';
COMMENT ON POLICY "players_delete_admin" ON players IS 'Allows admin users to delete players';
