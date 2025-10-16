/*
 * Fantasy Ekstraklasa Database Schema Migration
 * 
 * Purpose: Creates the complete database schema for the Fantasy Ekstraklasa Optimizer application
 * 
 * Affected Tables: 
 * - Core tables: teams, gameweeks, bonuses, scrape_runs
 * - Player tables: players, matches, player_stats  
 * - User tables: users, lineups, lineup_players, lineup_bonuses, transfer_tips, tutorial_status, generation_logs
 * 
 * Security: Row Level Security (RLS) enabled on all tables with granular policies
 * 
 * Special Considerations:
 * - Uses pg_trgm extension for fast player name search
 * - Implements proper foreign key relationships with appropriate CASCADE/RESTRICT actions
 * - Includes comprehensive constraints and checks for data integrity
 * - RLS policies differentiate between public data and user-specific data
 */

-- enable required extensions for trigram search
create extension if not exists "pg_trgm";

-- =============================================================================
-- CORE LOOKUP TABLES
-- =============================================================================

-- teams table: stores ekstraklasa team information
create table teams (
    id serial primary key,
    name text not null,
    short_code varchar(10) not null unique,
    crest_url text
);

-- enable rls on teams table
alter table teams enable row level security;

-- gameweeks table: stores gameweek information for the season
create table gameweeks (
    id serial primary key,
    number integer not null unique,
    start_date date not null,
    end_date date not null
);

-- enable rls on gameweeks table
alter table gameweeks enable row level security;

-- bonuses table: stores available lineup bonuses
create table bonuses (
    id serial primary key,
    name varchar(30) not null unique,
    description text
);

-- enable rls on bonuses table
alter table bonuses enable row level security;

-- scrape_runs table: tracks data scraping operations for operational visibility
create table scrape_runs (
    id serial primary key,
    run_type varchar(20) not null check (run_type in ('daily_main','pre_gameweek','post_gameweek','manual')),
    gameweek_id integer references gameweeks(id) on delete set null,
    started_at timestamp with time zone not null,
    finished_at timestamp with time zone,
    success boolean not null,
    error_message text
);

-- enable rls on scrape_runs table
alter table scrape_runs enable row level security;

-- =============================================================================
-- PLAYER-RELATED TABLES
-- =============================================================================

-- players table: stores player information including team association and fantasy price
create table players (
    id serial primary key,
    name text not null,
    team_id integer not null references teams(id) on delete restrict,
    position varchar(3) not null check (position in ('GK','DEF','MID','FWD')),
    price numeric(6,2) not null,
    health_status varchar(10) not null check (health_status in ('Pewny','Wątpliwy','Nie zagra')),
    created_at timestamp with time zone default now(),
    updated_at timestamp with time zone default now()
);

-- enable rls on players table
alter table players enable row level security;

-- matches table: stores match fixtures and results
create table matches (
    id serial primary key,
    gameweek_id integer not null references gameweeks(id) on delete cascade,
    home_team_id integer not null references teams(id) on delete restrict,
    away_team_id integer not null references teams(id) on delete restrict,
    match_date timestamp with time zone not null,
    home_score smallint,
    away_score smallint
);

-- enable rls on matches table
alter table matches enable row level security;

-- player_stats table: stores player performance data per gameweek
-- this is the core table for fantasy points and player analysis
create table player_stats (
    id serial primary key,
    player_id integer not null references players(id) on delete cascade,
    gameweek_id integer not null references gameweeks(id) on delete cascade,
    match_id integer references matches(id) on delete set null,
    fantasy_points smallint not null,
    minutes_played smallint not null,
    goals smallint not null,
    assists smallint not null,
    yellow_cards smallint not null,
    red_cards smallint not null,
    price numeric(6,2) not null,
    predicted_start boolean default false,
    created_at timestamp with time zone default now(),
    updated_at timestamp with time zone default now(),
    unique (player_id, gameweek_id)
);

-- enable rls on player_stats table
alter table player_stats enable row level security;

-- =============================================================================
-- USER-RELATED TABLES
-- =============================================================================

-- users table: extends supabase auth.users with profile metadata
-- supabase auth handles core user record; this table holds additional profile data
create table users (
    id uuid primary key references auth.users(id) on delete cascade,
    created_at timestamp with time zone default now(),
    updated_at timestamp with time zone default now()
);

-- enable rls on users table
alter table users enable row level security;

-- lineups table: stores user-created fantasy lineups for each gameweek
create table lineups (
    id uuid primary key default gen_random_uuid(),
    user_id uuid not null references users(id) on delete cascade,
    gameweek_id integer not null references gameweeks(id) on delete cascade,
    name varchar(50) not null,
    is_active boolean not null default true,
    total_cost numeric(6,2) not null,
    created_at timestamp with time zone default now(),
    updated_at timestamp with time zone default now(),
    unique (user_id, gameweek_id, name)
);

-- enable rls on lineups table
alter table lineups enable row level security;

-- lineup_players table: many-to-many relationship between lineups and players
-- includes role information (starting/bench) and special roles (captain/vice)
create table lineup_players (
    lineup_id uuid not null references lineups(id) on delete cascade,
    player_id integer not null references players(id) on delete restrict,
    role varchar(10) not null check (role in ('starting','bench')),
    is_captain boolean not null default false,
    is_vice boolean not null default false,
    is_locked boolean not null default false,
    primary key (lineup_id, player_id)
);

-- enable rls on lineup_players table
alter table lineup_players enable row level security;

-- lineup_bonuses table: tracks which bonuses are applied to which lineups
create table lineup_bonuses (
    id serial primary key,
    lineup_id uuid not null references lineups(id) on delete cascade,
    bonus_id integer not null references bonuses(id) on delete restrict,
    applied_at timestamp with time zone default now(),
    unique (lineup_id, bonus_id)
);

-- enable rls on lineup_bonuses table
alter table lineup_bonuses enable row level security;

-- transfer_tips table: stores ai-generated transfer suggestions for users
create table transfer_tips (
    id serial primary key,
    user_id uuid not null references users(id) on delete cascade,
    gameweek_id integer not null references gameweeks(id) on delete cascade,
    out_player_id integer not null references players(id) on delete restrict,
    in_player_id integer not null references players(id) on delete restrict,
    reason text not null,
    status varchar(10) not null default 'pending' check (status in ('pending','applied','rejected')),
    created_at timestamp with time zone default now(),
    applied_at timestamp with time zone
);

-- enable rls on transfer_tips table
alter table transfer_tips enable row level security;

-- tutorial_status table: tracks user progress through the onboarding tutorial
create table tutorial_status (
    user_id uuid primary key references users(id) on delete cascade,
    last_step smallint not null default 0,
    skipped boolean not null default false,
    updated_at timestamp with time zone default now()
);

-- enable rls on tutorial_status table
alter table tutorial_status enable row level security;

-- generation_logs table: tracks ai lineup generation requests for analytics and debugging
create table generation_logs (
    id serial primary key,
    user_id uuid not null references users(id) on delete cascade,
    lineup_id uuid references lineups(id) on delete set null,
    gameweek_id integer references gameweeks(id) on delete set null,
    model text not null,
    generation_time integer not null, -- seconds
    success boolean not null,
    error_message text,
    created_at timestamp with time zone default now()
);

-- enable rls on generation_logs table
alter table generation_logs enable row level security;

-- =============================================================================
-- ROW LEVEL SECURITY POLICIES
-- =============================================================================

-- teams policies: public read access for all users and roles
create policy "teams_select_anon" on teams for select to anon using (true);
create policy "teams_select_authenticated" on teams for select to authenticated using (true);

-- gameweeks policies: public read access for all users and roles
create policy "gameweeks_select_anon" on gameweeks for select to anon using (true);
create policy "gameweeks_select_authenticated" on gameweeks for select to authenticated using (true);

-- bonuses policies: public read access for all users and roles
create policy "bonuses_select_anon" on bonuses for select to anon using (true);
create policy "bonuses_select_authenticated" on bonuses for select to authenticated using (true);

-- scrape_runs policies: admin/system access only (no public policies)
-- public users should not see scraping operational data

-- players policies: public read access for all users and roles
create policy "players_select_anon" on players for select to anon using (true);
create policy "players_select_authenticated" on players for select to authenticated using (true);

-- matches policies: public read access for all users and roles
create policy "matches_select_anon" on matches for select to anon using (true);
create policy "matches_select_authenticated" on matches for select to authenticated using (true);

-- player_stats policies: public read access for all users and roles
-- fantasy points and stats should be visible to all users
create policy "player_stats_select_anon" on player_stats for select to anon using (true);
create policy "player_stats_select_authenticated" on player_stats for select to authenticated using (true);

-- users policies: users can only access their own profile
create policy "users_select_own" on users for select to authenticated using (auth.uid() = id);
create policy "users_insert_own" on users for insert to authenticated with check (auth.uid() = id);
create policy "users_update_own" on users for update to authenticated using (auth.uid() = id);

-- lineups policies: users can only access their own lineups
create policy "lineups_select_own" on lineups for select to authenticated using (auth.uid() = user_id);
create policy "lineups_insert_own" on lineups for insert to authenticated with check (auth.uid() = user_id);
create policy "lineups_update_own" on lineups for update to authenticated using (auth.uid() = user_id);
create policy "lineups_delete_own" on lineups for delete to authenticated using (auth.uid() = user_id);

-- lineup_players policies: users can only access lineup_players for their own lineups
create policy "lineup_players_select_own" on lineup_players for select to authenticated 
using (
    lineup_id in (
        select id from lineups where user_id = auth.uid()
    )
);
create policy "lineup_players_insert_own" on lineup_players for insert to authenticated 
with check (
    lineup_id in (
        select id from lineups where user_id = auth.uid()
    )
);
create policy "lineup_players_update_own" on lineup_players for update to authenticated 
using (
    lineup_id in (
        select id from lineups where user_id = auth.uid()
    )
);
create policy "lineup_players_delete_own" on lineup_players for delete to authenticated 
using (
    lineup_id in (
        select id from lineups where user_id = auth.uid()
    )
);

-- lineup_bonuses policies: users can only access lineup_bonuses for their own lineups
create policy "lineup_bonuses_select_own" on lineup_bonuses for select to authenticated 
using (
    lineup_id in (
        select id from lineups where user_id = auth.uid()
    )
);
create policy "lineup_bonuses_insert_own" on lineup_bonuses for insert to authenticated 
with check (
    lineup_id in (
        select id from lineups where user_id = auth.uid()
    )
);
create policy "lineup_bonuses_update_own" on lineup_bonuses for update to authenticated 
using (
    lineup_id in (
        select id from lineups where user_id = auth.uid()
    )
);
create policy "lineup_bonuses_delete_own" on lineup_bonuses for delete to authenticated 
using (
    lineup_id in (
        select id from lineups where user_id = auth.uid()
    )
);

-- transfer_tips policies: users can only access their own transfer tips
create policy "transfer_tips_select_own" on transfer_tips for select to authenticated using (auth.uid() = user_id);
create policy "transfer_tips_insert_own" on transfer_tips for insert to authenticated with check (auth.uid() = user_id);
create policy "transfer_tips_update_own" on transfer_tips for update to authenticated using (auth.uid() = user_id);
create policy "transfer_tips_delete_own" on transfer_tips for delete to authenticated using (auth.uid() = user_id);

-- tutorial_status policies: users can only access their own tutorial status
create policy "tutorial_status_select_own" on tutorial_status for select to authenticated using (auth.uid() = user_id);
create policy "tutorial_status_insert_own" on tutorial_status for insert to authenticated with check (auth.uid() = user_id);
create policy "tutorial_status_update_own" on tutorial_status for update to authenticated using (auth.uid() = user_id);
create policy "tutorial_status_delete_own" on tutorial_status for delete to authenticated using (auth.uid() = user_id);

-- generation_logs policies: users can only access their own generation logs
create policy "generation_logs_select_own" on generation_logs for select to authenticated using (auth.uid() = user_id);
create policy "generation_logs_insert_own" on generation_logs for insert to authenticated with check (auth.uid() = user_id);
create policy "generation_logs_update_own" on generation_logs for update to authenticated using (auth.uid() = user_id);

-- =============================================================================
-- PERFORMANCE INDEXES
-- =============================================================================

-- foreign key indexes for optimal join performance
create index idx_players_team_id on players(team_id);
create index idx_player_stats_player_id on player_stats(player_id);
create index idx_player_stats_gameweek_id on player_stats(gameweek_id);
create index idx_player_stats_match_id on player_stats(match_id);
create index idx_matches_gameweek_id on matches(gameweek_id);
create index idx_matches_home_team_id on matches(home_team_id);
create index idx_matches_away_team_id on matches(away_team_id);
create index idx_lineups_user_id on lineups(user_id);
create index idx_lineups_gameweek_id on lineups(gameweek_id);
create index idx_lineup_players_lineup_id on lineup_players(lineup_id);
create index idx_lineup_players_player_id on lineup_players(player_id);
create index idx_lineup_bonuses_lineup_id on lineup_bonuses(lineup_id);
create index idx_lineup_bonuses_bonus_id on lineup_bonuses(bonus_id);
create index idx_transfer_tips_user_id on transfer_tips(user_id);
create index idx_transfer_tips_gameweek_id on transfer_tips(gameweek_id);
create index idx_generation_logs_user_id on generation_logs(user_id);
create index idx_scrape_runs_gameweek_id on scrape_runs(gameweek_id);

-- composite indexes for common query patterns
create index idx_transfer_tips_user_gameweek on transfer_tips(user_id, gameweek_id);
create index idx_lineups_user_gameweek_active on lineups(user_id, gameweek_id) where is_active = true;

-- trigram index for fast player name search
-- this enables fuzzy text search on player names for autocomplete functionality
create index idx_players_name_trgm on players using gin (name gin_trgm_ops);

-- time-based indexes for analytics queries
create index idx_generation_logs_created_at on generation_logs(created_at);
create index idx_scrape_runs_started_at on scrape_runs(started_at);
create index idx_player_stats_created_at on player_stats(created_at);
