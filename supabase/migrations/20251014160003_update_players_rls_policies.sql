-- Nowa migracja: 20251014170000_update_players_rls_policies.sql
-- Usuń istniejące polityki admin
DROP POLICY IF EXISTS "players_insert_admin" ON players;
DROP POLICY IF EXISTS "players_update_admin" ON players;
DROP POLICY IF EXISTS "players_delete_admin" ON players;

-- Dodaj publiczne polityki (jak w teams)
CREATE POLICY players_select_policy ON players
  FOR SELECT USING (true);

CREATE POLICY players_insert_policy ON players
  FOR INSERT WITH CHECK (true);

CREATE POLICY players_update_policy ON players
  FOR UPDATE USING (true);

CREATE POLICY players_delete_policy ON players
  FOR DELETE USING (true);