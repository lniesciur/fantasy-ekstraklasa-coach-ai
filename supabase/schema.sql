


SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;


CREATE SCHEMA IF NOT EXISTS "public";


ALTER SCHEMA "public" OWNER TO "pg_database_owner";


COMMENT ON SCHEMA "public" IS 'standard public schema';


SET default_tablespace = '';

SET default_table_access_method = "heap";


CREATE TABLE IF NOT EXISTS "public"."bonuses" (
    "id" integer NOT NULL,
    "name" character varying(30) NOT NULL,
    "description" "text"
);


ALTER TABLE "public"."bonuses" OWNER TO "postgres";


CREATE SEQUENCE IF NOT EXISTS "public"."bonuses_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."bonuses_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."bonuses_id_seq" OWNED BY "public"."bonuses"."id";



CREATE TABLE IF NOT EXISTS "public"."gameweeks" (
    "id" integer NOT NULL,
    "number" integer NOT NULL,
    "start_date" "date" NOT NULL,
    "end_date" "date" NOT NULL
);


ALTER TABLE "public"."gameweeks" OWNER TO "postgres";


CREATE SEQUENCE IF NOT EXISTS "public"."gameweeks_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."gameweeks_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."gameweeks_id_seq" OWNED BY "public"."gameweeks"."id";



CREATE TABLE IF NOT EXISTS "public"."generation_logs" (
    "id" integer NOT NULL,
    "user_id" "uuid" NOT NULL,
    "lineup_id" "uuid",
    "gameweek_id" integer,
    "model" "text" NOT NULL,
    "generation_time" integer NOT NULL,
    "success" boolean NOT NULL,
    "error_message" "text",
    "created_at" timestamp with time zone DEFAULT "now"()
);


ALTER TABLE "public"."generation_logs" OWNER TO "postgres";


CREATE SEQUENCE IF NOT EXISTS "public"."generation_logs_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."generation_logs_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."generation_logs_id_seq" OWNED BY "public"."generation_logs"."id";



CREATE TABLE IF NOT EXISTS "public"."import_logs" (
    "id" "uuid" DEFAULT "gen_random_uuid"() NOT NULL,
    "user_id" "uuid" NOT NULL,
    "validation_id" "uuid",
    "filename" character varying(255) NOT NULL,
    "file_size" integer NOT NULL,
    "import_type" character varying(20) DEFAULT 'excel'::character varying NOT NULL,
    "status" character varying(20) NOT NULL,
    "overwrite_mode" character varying(20) NOT NULL,
    "gameweek_id" integer,
    "started_at" timestamp with time zone DEFAULT "now"(),
    "completed_at" timestamp with time zone,
    "validation_results" "jsonb",
    "import_results" "jsonb",
    "error_message" "text",
    "players_total" integer DEFAULT 0,
    "players_imported" integer DEFAULT 0,
    "players_updated" integer DEFAULT 0,
    "players_created" integer DEFAULT 0,
    "warnings_count" integer DEFAULT 0,
    "errors_count" integer DEFAULT 0,
    CONSTRAINT "import_logs_import_type_check" CHECK ((("import_type")::"text" = ANY ((ARRAY['excel'::character varying, 'csv'::character varying, 'json'::character varying])::"text"[]))),
    CONSTRAINT "import_logs_overwrite_mode_check" CHECK ((("overwrite_mode")::"text" = ANY ((ARRAY['replace_existing'::character varying, 'update_only'::character varying, 'create_only'::character varying])::"text"[]))),
    CONSTRAINT "import_logs_status_check" CHECK ((("status")::"text" = ANY ((ARRAY['validating'::character varying, 'validation_failed'::character varying, 'validated'::character varying, 'importing'::character varying, 'completed'::character varying, 'failed'::character varying])::"text"[])))
);


ALTER TABLE "public"."import_logs" OWNER TO "postgres";


COMMENT ON TABLE "public"."import_logs" IS 'Tracks manual data imports by users, including validation and execution status';



COMMENT ON COLUMN "public"."import_logs"."status" IS 'Import workflow status: validating -> validated -> importing -> completed/failed';



COMMENT ON COLUMN "public"."import_logs"."validation_results" IS 'JSON containing validation errors and warnings from file upload';



COMMENT ON COLUMN "public"."import_logs"."import_results" IS 'JSON containing detailed import statistics and results';



CREATE TABLE IF NOT EXISTS "public"."lineup_bonuses" (
    "id" integer NOT NULL,
    "lineup_id" "uuid" NOT NULL,
    "bonus_id" integer NOT NULL,
    "applied_at" timestamp with time zone DEFAULT "now"()
);


ALTER TABLE "public"."lineup_bonuses" OWNER TO "postgres";


CREATE SEQUENCE IF NOT EXISTS "public"."lineup_bonuses_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."lineup_bonuses_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."lineup_bonuses_id_seq" OWNED BY "public"."lineup_bonuses"."id";



CREATE TABLE IF NOT EXISTS "public"."lineup_players" (
    "lineup_id" "uuid" NOT NULL,
    "player_id" integer NOT NULL,
    "role" character varying(10) NOT NULL,
    "is_captain" boolean DEFAULT false NOT NULL,
    "is_vice" boolean DEFAULT false NOT NULL,
    "is_locked" boolean DEFAULT false NOT NULL,
    CONSTRAINT "lineup_players_role_check" CHECK ((("role")::"text" = ANY ((ARRAY['starting'::character varying, 'bench'::character varying])::"text"[])))
);


ALTER TABLE "public"."lineup_players" OWNER TO "postgres";


CREATE TABLE IF NOT EXISTS "public"."lineups" (
    "id" "uuid" DEFAULT "gen_random_uuid"() NOT NULL,
    "user_id" "uuid" NOT NULL,
    "gameweek_id" integer NOT NULL,
    "name" character varying(50) NOT NULL,
    "is_active" boolean DEFAULT true NOT NULL,
    "total_cost" numeric(6,2) NOT NULL,
    "created_at" timestamp with time zone DEFAULT "now"(),
    "updated_at" timestamp with time zone DEFAULT "now"()
);


ALTER TABLE "public"."lineups" OWNER TO "postgres";


CREATE TABLE IF NOT EXISTS "public"."matches" (
    "id" integer NOT NULL,
    "gameweek_id" integer NOT NULL,
    "home_team_id" integer NOT NULL,
    "away_team_id" integer NOT NULL,
    "match_date" timestamp with time zone NOT NULL,
    "home_score" smallint,
    "away_score" smallint,
    "status" character varying(12) DEFAULT 'scheduled'::character varying NOT NULL,
    "reschedule_reason" "text",
    CONSTRAINT "matches_status_check" CHECK ((("status")::"text" = ANY ((ARRAY['scheduled'::character varying, 'postponed'::character varying, 'cancelled'::character varying, 'played'::character varying])::"text"[])))
);


ALTER TABLE "public"."matches" OWNER TO "postgres";


CREATE SEQUENCE IF NOT EXISTS "public"."matches_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."matches_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."matches_id_seq" OWNED BY "public"."matches"."id";



CREATE TABLE IF NOT EXISTS "public"."player_stats" (
    "id" integer NOT NULL,
    "player_id" integer NOT NULL,
    "match_id" integer,
    "fantasy_points" smallint NOT NULL,
    "minutes_played" smallint NOT NULL,
    "goals" smallint NOT NULL,
    "assists" smallint NOT NULL,
    "yellow_cards" smallint NOT NULL,
    "red_cards" smallint NOT NULL,
    "price" numeric(6,2) NOT NULL,
    "predicted_start" boolean DEFAULT false,
    "created_at" timestamp with time zone DEFAULT "now"(),
    "updated_at" timestamp with time zone DEFAULT "now"(),
    "lotto_assists" smallint DEFAULT 0 NOT NULL,
    "own_goals" smallint DEFAULT 0 NOT NULL,
    "penalties_saved" smallint DEFAULT 0 NOT NULL,
    "penalties_won" smallint DEFAULT 0 NOT NULL,
    "in_team_of_week" boolean DEFAULT false NOT NULL,
    "saves" smallint DEFAULT 0 NOT NULL,
    "penalties_scored" smallint DEFAULT 0 NOT NULL,
    "penalties_caused" smallint DEFAULT 0 NOT NULL,
    "penalties_missed" smallint DEFAULT 0 NOT NULL,
    "health_status" character varying(10),
    CONSTRAINT "check_penalties_caused_non_negative" CHECK (("penalties_caused" >= 0)),
    CONSTRAINT "check_penalties_missed_non_negative" CHECK (("penalties_missed" >= 0)),
    CONSTRAINT "check_penalties_scored_non_negative" CHECK (("penalties_scored" >= 0)),
    CONSTRAINT "player_stats_health_status_check" CHECK ((("health_status")::"text" = ANY ((ARRAY['Pewny'::character varying, 'Wątpliwy'::character varying, 'Nie zagra'::character varying])::"text"[])))
);


ALTER TABLE "public"."player_stats" OWNER TO "postgres";


COMMENT ON COLUMN "public"."player_stats"."health_status" IS 'Player health status for this specific gameweek, allowing historical health analysis';



CREATE SEQUENCE IF NOT EXISTS "public"."player_stats_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."player_stats_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."player_stats_id_seq" OWNED BY "public"."player_stats"."id";



CREATE TABLE IF NOT EXISTS "public"."players" (
    "id" integer NOT NULL,
    "name" "text" NOT NULL,
    "team_id" integer NOT NULL,
    "position" character varying(3) NOT NULL,
    "price" numeric(6,2) NOT NULL,
    "health_status" character varying(10) NOT NULL,
    "created_at" timestamp with time zone DEFAULT "now"(),
    "updated_at" timestamp with time zone DEFAULT "now"(),
    CONSTRAINT "players_health_status_check" CHECK ((("health_status")::"text" = ANY ((ARRAY['Pewny'::character varying, 'Wątpliwy'::character varying, 'Nie zagra'::character varying])::"text"[]))),
    CONSTRAINT "players_name_not_empty" CHECK (("length"(TRIM(BOTH FROM "name")) > 0)),
    CONSTRAINT "players_position_check" CHECK ((("position")::"text" = ANY ((ARRAY['GK'::character varying, 'DEF'::character varying, 'MID'::character varying, 'FWD'::character varying])::"text"[]))),
    CONSTRAINT "players_price_range" CHECK ((("price" > (0)::numeric) AND ("price" <= (50000000)::numeric)))
);


ALTER TABLE "public"."players" OWNER TO "postgres";


COMMENT ON CONSTRAINT "players_name_not_empty" ON "public"."players" IS 'Ensures player name is not empty or whitespace only';



COMMENT ON CONSTRAINT "players_price_range" ON "public"."players" IS 'Ensures player price is within valid range (0 < price <= 50M)';



CREATE SEQUENCE IF NOT EXISTS "public"."players_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."players_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."players_id_seq" OWNED BY "public"."players"."id";



CREATE TABLE IF NOT EXISTS "public"."scrape_runs" (
    "id" integer NOT NULL,
    "run_type" character varying(20) NOT NULL,
    "gameweek_id" integer,
    "started_at" timestamp with time zone NOT NULL,
    "finished_at" timestamp with time zone,
    "success" boolean NOT NULL,
    "error_message" "text",
    CONSTRAINT "scrape_runs_run_type_check" CHECK ((("run_type")::"text" = ANY ((ARRAY['daily_main'::character varying, 'pre_gameweek'::character varying, 'post_gameweek'::character varying, 'manual'::character varying])::"text"[])))
);


ALTER TABLE "public"."scrape_runs" OWNER TO "postgres";


CREATE SEQUENCE IF NOT EXISTS "public"."scrape_runs_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."scrape_runs_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."scrape_runs_id_seq" OWNED BY "public"."scrape_runs"."id";



CREATE TABLE IF NOT EXISTS "public"."teams" (
    "id" integer NOT NULL,
    "name" "text" NOT NULL,
    "short_code" character varying(10) NOT NULL,
    "crest_url" "text",
    "is_active" boolean DEFAULT true NOT NULL
);


ALTER TABLE "public"."teams" OWNER TO "postgres";


COMMENT ON COLUMN "public"."teams"."crest_url" IS 'URL do herbu drużyny - obrazek 100x100px z CloudFront CDN';



CREATE SEQUENCE IF NOT EXISTS "public"."teams_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."teams_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."teams_id_seq" OWNED BY "public"."teams"."id";



CREATE TABLE IF NOT EXISTS "public"."transfer_tips" (
    "id" integer NOT NULL,
    "user_id" "uuid" NOT NULL,
    "gameweek_id" integer NOT NULL,
    "out_player_id" integer NOT NULL,
    "in_player_id" integer NOT NULL,
    "reason" "text" NOT NULL,
    "status" character varying(10) DEFAULT 'pending'::character varying NOT NULL,
    "created_at" timestamp with time zone DEFAULT "now"(),
    "applied_at" timestamp with time zone,
    CONSTRAINT "transfer_tips_status_check" CHECK ((("status")::"text" = ANY ((ARRAY['pending'::character varying, 'applied'::character varying, 'rejected'::character varying])::"text"[])))
);


ALTER TABLE "public"."transfer_tips" OWNER TO "postgres";


CREATE SEQUENCE IF NOT EXISTS "public"."transfer_tips_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE "public"."transfer_tips_id_seq" OWNER TO "postgres";


ALTER SEQUENCE "public"."transfer_tips_id_seq" OWNED BY "public"."transfer_tips"."id";



CREATE TABLE IF NOT EXISTS "public"."tutorial_status" (
    "user_id" "uuid" NOT NULL,
    "last_step" smallint DEFAULT 0 NOT NULL,
    "skipped" boolean DEFAULT false NOT NULL,
    "updated_at" timestamp with time zone DEFAULT "now"()
);


ALTER TABLE "public"."tutorial_status" OWNER TO "postgres";


CREATE TABLE IF NOT EXISTS "public"."users" (
    "id" "uuid" NOT NULL,
    "created_at" timestamp with time zone DEFAULT "now"(),
    "updated_at" timestamp with time zone DEFAULT "now"()
);


ALTER TABLE "public"."users" OWNER TO "postgres";


ALTER TABLE ONLY "public"."bonuses" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."bonuses_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."gameweeks" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."gameweeks_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."generation_logs" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."generation_logs_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."lineup_bonuses" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."lineup_bonuses_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."matches" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."matches_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."player_stats" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."player_stats_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."players" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."players_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."scrape_runs" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."scrape_runs_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."teams" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."teams_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."transfer_tips" ALTER COLUMN "id" SET DEFAULT "nextval"('"public"."transfer_tips_id_seq"'::"regclass");



ALTER TABLE ONLY "public"."bonuses"
    ADD CONSTRAINT "bonuses_name_key" UNIQUE ("name");



ALTER TABLE ONLY "public"."bonuses"
    ADD CONSTRAINT "bonuses_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."gameweeks"
    ADD CONSTRAINT "gameweeks_number_key" UNIQUE ("number");



ALTER TABLE ONLY "public"."gameweeks"
    ADD CONSTRAINT "gameweeks_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."generation_logs"
    ADD CONSTRAINT "generation_logs_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."import_logs"
    ADD CONSTRAINT "import_logs_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."lineup_bonuses"
    ADD CONSTRAINT "lineup_bonuses_lineup_id_bonus_id_key" UNIQUE ("lineup_id", "bonus_id");



ALTER TABLE ONLY "public"."lineup_bonuses"
    ADD CONSTRAINT "lineup_bonuses_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."lineup_players"
    ADD CONSTRAINT "lineup_players_pkey" PRIMARY KEY ("lineup_id", "player_id");



ALTER TABLE ONLY "public"."lineups"
    ADD CONSTRAINT "lineups_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."lineups"
    ADD CONSTRAINT "lineups_user_id_gameweek_id_name_key" UNIQUE ("user_id", "gameweek_id", "name");



ALTER TABLE ONLY "public"."matches"
    ADD CONSTRAINT "matches_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."player_stats"
    ADD CONSTRAINT "player_stats_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."player_stats"
    ADD CONSTRAINT "player_stats_player_id_match_id_key" UNIQUE ("player_id", "match_id");



ALTER TABLE ONLY "public"."players"
    ADD CONSTRAINT "players_name_team_unique" UNIQUE ("name", "team_id");



COMMENT ON CONSTRAINT "players_name_team_unique" ON "public"."players" IS 'Ensures player names are unique within each team';



ALTER TABLE ONLY "public"."players"
    ADD CONSTRAINT "players_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."scrape_runs"
    ADD CONSTRAINT "scrape_runs_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."teams"
    ADD CONSTRAINT "teams_name_unique" UNIQUE ("name");



ALTER TABLE ONLY "public"."teams"
    ADD CONSTRAINT "teams_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."teams"
    ADD CONSTRAINT "teams_short_code_key" UNIQUE ("short_code");



ALTER TABLE ONLY "public"."transfer_tips"
    ADD CONSTRAINT "transfer_tips_pkey" PRIMARY KEY ("id");



ALTER TABLE ONLY "public"."tutorial_status"
    ADD CONSTRAINT "tutorial_status_pkey" PRIMARY KEY ("user_id");



ALTER TABLE ONLY "public"."users"
    ADD CONSTRAINT "users_pkey" PRIMARY KEY ("id");



CREATE INDEX "idx_gameweeks_number" ON "public"."gameweeks" USING "btree" ("number");



CREATE INDEX "idx_gameweeks_start_date" ON "public"."gameweeks" USING "btree" ("start_date");



CREATE INDEX "idx_generation_logs_created_at" ON "public"."generation_logs" USING "btree" ("created_at");



CREATE INDEX "idx_generation_logs_user_id" ON "public"."generation_logs" USING "btree" ("user_id");



CREATE INDEX "idx_import_logs_user_id_gameweek_id" ON "public"."import_logs" USING "btree" ("user_id", "gameweek_id") WHERE ("gameweek_id" IS NOT NULL);



CREATE INDEX "idx_import_logs_user_id_status_started_at" ON "public"."import_logs" USING "btree" ("user_id", "status", "started_at");



CREATE INDEX "idx_import_logs_validation_id" ON "public"."import_logs" USING "btree" ("validation_id") WHERE ("validation_id" IS NOT NULL);



CREATE INDEX "idx_lineup_bonuses_bonus_id" ON "public"."lineup_bonuses" USING "btree" ("bonus_id");



CREATE INDEX "idx_lineup_bonuses_lineup_id" ON "public"."lineup_bonuses" USING "btree" ("lineup_id");



CREATE INDEX "idx_lineup_players_lineup_id" ON "public"."lineup_players" USING "btree" ("lineup_id");



CREATE INDEX "idx_lineup_players_player_id" ON "public"."lineup_players" USING "btree" ("player_id");



CREATE INDEX "idx_lineups_gameweek_id" ON "public"."lineups" USING "btree" ("gameweek_id");



CREATE INDEX "idx_lineups_user_gameweek_active" ON "public"."lineups" USING "btree" ("user_id", "gameweek_id") WHERE ("is_active" = true);



CREATE INDEX "idx_lineups_user_id" ON "public"."lineups" USING "btree" ("user_id");



CREATE INDEX "idx_matches_away_team_id" ON "public"."matches" USING "btree" ("away_team_id");



CREATE INDEX "idx_matches_gameweek_id" ON "public"."matches" USING "btree" ("gameweek_id");



CREATE INDEX "idx_matches_gameweek_status" ON "public"."matches" USING "btree" ("gameweek_id", "status");



COMMENT ON INDEX "public"."idx_matches_gameweek_status" IS 'Composite index for gameweek and status queries';



CREATE INDEX "idx_matches_gameweek_status_date" ON "public"."matches" USING "btree" ("gameweek_id", "status", "match_date");



CREATE INDEX "idx_matches_home_team_id" ON "public"."matches" USING "btree" ("home_team_id");



CREATE INDEX "idx_matches_match_date" ON "public"."matches" USING "btree" ("match_date");



CREATE INDEX "idx_matches_status" ON "public"."matches" USING "btree" ("status");



COMMENT ON INDEX "public"."idx_matches_status" IS 'Index for filtering matches by status';



CREATE INDEX "idx_player_stats_created_at" ON "public"."player_stats" USING "btree" ("created_at");






CREATE INDEX "idx_player_stats_health_status" ON "public"."player_stats" USING "btree" ("health_status");



CREATE INDEX "idx_player_stats_match_id" ON "public"."player_stats" USING "btree" ("match_id");



CREATE INDEX "idx_player_stats_player_health" ON "public"."player_stats" USING "btree" ("player_id", "health_status");



CREATE INDEX "idx_player_stats_player_id" ON "public"."player_stats" USING "btree" ("player_id");



CREATE INDEX "idx_players_health_status" ON "public"."players" USING "btree" ("health_status");



CREATE INDEX "idx_players_name_team" ON "public"."players" USING "btree" ("name", "team_id");



CREATE INDEX "idx_players_name_trgm" ON "public"."players" USING "gin" ("name" "public"."gin_trgm_ops");



CREATE INDEX "idx_players_position" ON "public"."players" USING "btree" ("position");



CREATE INDEX "idx_players_price" ON "public"."players" USING "btree" ("price");



CREATE INDEX "idx_players_team_id" ON "public"."players" USING "btree" ("team_id");



CREATE INDEX "idx_players_team_position" ON "public"."players" USING "btree" ("team_id", "position");



CREATE INDEX "idx_players_team_price" ON "public"."players" USING "btree" ("team_id", "price");



CREATE INDEX "idx_scrape_runs_gameweek_id" ON "public"."scrape_runs" USING "btree" ("gameweek_id");



CREATE INDEX "idx_scrape_runs_started_at" ON "public"."scrape_runs" USING "btree" ("started_at");



CREATE INDEX "idx_teams_active" ON "public"."teams" USING "btree" ("is_active") WHERE ("is_active" = true);



CREATE INDEX "idx_teams_is_active" ON "public"."teams" USING "btree" ("is_active");



CREATE INDEX "idx_teams_name" ON "public"."teams" USING "btree" ("name");



CREATE INDEX "idx_teams_short_code" ON "public"."teams" USING "btree" ("short_code");



CREATE INDEX "idx_transfer_tips_gameweek_id" ON "public"."transfer_tips" USING "btree" ("gameweek_id");



CREATE INDEX "idx_transfer_tips_user_gameweek" ON "public"."transfer_tips" USING "btree" ("user_id", "gameweek_id");



CREATE INDEX "idx_transfer_tips_user_id" ON "public"."transfer_tips" USING "btree" ("user_id");



ALTER TABLE ONLY "public"."generation_logs"
    ADD CONSTRAINT "generation_logs_gameweek_id_fkey" FOREIGN KEY ("gameweek_id") REFERENCES "public"."gameweeks"("id") ON DELETE SET NULL;



ALTER TABLE ONLY "public"."generation_logs"
    ADD CONSTRAINT "generation_logs_lineup_id_fkey" FOREIGN KEY ("lineup_id") REFERENCES "public"."lineups"("id") ON DELETE SET NULL;



ALTER TABLE ONLY "public"."generation_logs"
    ADD CONSTRAINT "generation_logs_user_id_fkey" FOREIGN KEY ("user_id") REFERENCES "public"."users"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."import_logs"
    ADD CONSTRAINT "import_logs_gameweek_id_fkey" FOREIGN KEY ("gameweek_id") REFERENCES "public"."gameweeks"("id") ON DELETE SET NULL;



ALTER TABLE ONLY "public"."import_logs"
    ADD CONSTRAINT "import_logs_user_id_fkey" FOREIGN KEY ("user_id") REFERENCES "public"."users"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."lineup_bonuses"
    ADD CONSTRAINT "lineup_bonuses_bonus_id_fkey" FOREIGN KEY ("bonus_id") REFERENCES "public"."bonuses"("id") ON DELETE RESTRICT;



ALTER TABLE ONLY "public"."lineup_bonuses"
    ADD CONSTRAINT "lineup_bonuses_lineup_id_fkey" FOREIGN KEY ("lineup_id") REFERENCES "public"."lineups"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."lineup_players"
    ADD CONSTRAINT "lineup_players_lineup_id_fkey" FOREIGN KEY ("lineup_id") REFERENCES "public"."lineups"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."lineup_players"
    ADD CONSTRAINT "lineup_players_player_id_fkey" FOREIGN KEY ("player_id") REFERENCES "public"."players"("id") ON DELETE RESTRICT;



ALTER TABLE ONLY "public"."lineups"
    ADD CONSTRAINT "lineups_gameweek_id_fkey" FOREIGN KEY ("gameweek_id") REFERENCES "public"."gameweeks"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."lineups"
    ADD CONSTRAINT "lineups_user_id_fkey" FOREIGN KEY ("user_id") REFERENCES "public"."users"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."matches"
    ADD CONSTRAINT "matches_away_team_id_fkey" FOREIGN KEY ("away_team_id") REFERENCES "public"."teams"("id") ON DELETE RESTRICT;



ALTER TABLE ONLY "public"."matches"
    ADD CONSTRAINT "matches_gameweek_id_fkey" FOREIGN KEY ("gameweek_id") REFERENCES "public"."gameweeks"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."matches"
    ADD CONSTRAINT "matches_home_team_id_fkey" FOREIGN KEY ("home_team_id") REFERENCES "public"."teams"("id") ON DELETE RESTRICT;






ALTER TABLE ONLY "public"."player_stats"
    ADD CONSTRAINT "player_stats_match_id_fkey" FOREIGN KEY ("match_id") REFERENCES "public"."matches"("id") ON DELETE SET NULL;



ALTER TABLE ONLY "public"."player_stats"
    ADD CONSTRAINT "player_stats_player_id_fkey" FOREIGN KEY ("player_id") REFERENCES "public"."players"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."players"
    ADD CONSTRAINT "players_team_id_fkey" FOREIGN KEY ("team_id") REFERENCES "public"."teams"("id") ON DELETE RESTRICT;



ALTER TABLE ONLY "public"."scrape_runs"
    ADD CONSTRAINT "scrape_runs_gameweek_id_fkey" FOREIGN KEY ("gameweek_id") REFERENCES "public"."gameweeks"("id") ON DELETE SET NULL;



ALTER TABLE ONLY "public"."transfer_tips"
    ADD CONSTRAINT "transfer_tips_gameweek_id_fkey" FOREIGN KEY ("gameweek_id") REFERENCES "public"."gameweeks"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."transfer_tips"
    ADD CONSTRAINT "transfer_tips_in_player_id_fkey" FOREIGN KEY ("in_player_id") REFERENCES "public"."players"("id") ON DELETE RESTRICT;



ALTER TABLE ONLY "public"."transfer_tips"
    ADD CONSTRAINT "transfer_tips_out_player_id_fkey" FOREIGN KEY ("out_player_id") REFERENCES "public"."players"("id") ON DELETE RESTRICT;



ALTER TABLE ONLY "public"."transfer_tips"
    ADD CONSTRAINT "transfer_tips_user_id_fkey" FOREIGN KEY ("user_id") REFERENCES "public"."users"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."tutorial_status"
    ADD CONSTRAINT "tutorial_status_user_id_fkey" FOREIGN KEY ("user_id") REFERENCES "public"."users"("id") ON DELETE CASCADE;



ALTER TABLE ONLY "public"."users"
    ADD CONSTRAINT "users_id_fkey" FOREIGN KEY ("id") REFERENCES "auth"."users"("id") ON DELETE CASCADE;



ALTER TABLE "public"."bonuses" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "bonuses_select_anon" ON "public"."bonuses" FOR SELECT TO "anon" USING (true);



CREATE POLICY "bonuses_select_authenticated" ON "public"."bonuses" FOR SELECT TO "authenticated" USING (true);



ALTER TABLE "public"."gameweeks" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "gameweeks_delete_policy" ON "public"."gameweeks" FOR DELETE USING (true);



CREATE POLICY "gameweeks_insert_policy" ON "public"."gameweeks" FOR INSERT WITH CHECK (true);



CREATE POLICY "gameweeks_select_anon" ON "public"."gameweeks" FOR SELECT TO "anon" USING (true);



CREATE POLICY "gameweeks_select_authenticated" ON "public"."gameweeks" FOR SELECT TO "authenticated" USING (true);



CREATE POLICY "gameweeks_select_policy" ON "public"."gameweeks" FOR SELECT USING (true);



CREATE POLICY "gameweeks_update_policy" ON "public"."gameweeks" FOR UPDATE USING (true);



ALTER TABLE "public"."generation_logs" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "generation_logs_insert_own" ON "public"."generation_logs" FOR INSERT TO "authenticated" WITH CHECK (("auth"."uid"() = "user_id"));



CREATE POLICY "generation_logs_select_own" ON "public"."generation_logs" FOR SELECT TO "authenticated" USING (("auth"."uid"() = "user_id"));



CREATE POLICY "generation_logs_update_own" ON "public"."generation_logs" FOR UPDATE TO "authenticated" USING (("auth"."uid"() = "user_id"));



ALTER TABLE "public"."import_logs" ENABLE ROW LEVEL SECURITY;


ALTER TABLE "public"."lineup_bonuses" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "lineup_bonuses_delete_own" ON "public"."lineup_bonuses" FOR DELETE TO "authenticated" USING (("lineup_id" IN ( SELECT "lineups"."id"
   FROM "public"."lineups"
  WHERE ("lineups"."user_id" = "auth"."uid"()))));



CREATE POLICY "lineup_bonuses_insert_own" ON "public"."lineup_bonuses" FOR INSERT TO "authenticated" WITH CHECK (("lineup_id" IN ( SELECT "lineups"."id"
   FROM "public"."lineups"
  WHERE ("lineups"."user_id" = "auth"."uid"()))));



CREATE POLICY "lineup_bonuses_select_own" ON "public"."lineup_bonuses" FOR SELECT TO "authenticated" USING (("lineup_id" IN ( SELECT "lineups"."id"
   FROM "public"."lineups"
  WHERE ("lineups"."user_id" = "auth"."uid"()))));



CREATE POLICY "lineup_bonuses_update_own" ON "public"."lineup_bonuses" FOR UPDATE TO "authenticated" USING (("lineup_id" IN ( SELECT "lineups"."id"
   FROM "public"."lineups"
  WHERE ("lineups"."user_id" = "auth"."uid"()))));



ALTER TABLE "public"."lineup_players" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "lineup_players_delete_own" ON "public"."lineup_players" FOR DELETE TO "authenticated" USING (("lineup_id" IN ( SELECT "lineups"."id"
   FROM "public"."lineups"
  WHERE ("lineups"."user_id" = "auth"."uid"()))));



CREATE POLICY "lineup_players_insert_own" ON "public"."lineup_players" FOR INSERT TO "authenticated" WITH CHECK (("lineup_id" IN ( SELECT "lineups"."id"
   FROM "public"."lineups"
  WHERE ("lineups"."user_id" = "auth"."uid"()))));



CREATE POLICY "lineup_players_select_own" ON "public"."lineup_players" FOR SELECT TO "authenticated" USING (("lineup_id" IN ( SELECT "lineups"."id"
   FROM "public"."lineups"
  WHERE ("lineups"."user_id" = "auth"."uid"()))));



CREATE POLICY "lineup_players_update_own" ON "public"."lineup_players" FOR UPDATE TO "authenticated" USING (("lineup_id" IN ( SELECT "lineups"."id"
   FROM "public"."lineups"
  WHERE ("lineups"."user_id" = "auth"."uid"()))));



ALTER TABLE "public"."lineups" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "lineups_delete_own" ON "public"."lineups" FOR DELETE TO "authenticated" USING (("auth"."uid"() = "user_id"));



CREATE POLICY "lineups_insert_own" ON "public"."lineups" FOR INSERT TO "authenticated" WITH CHECK (("auth"."uid"() = "user_id"));



CREATE POLICY "lineups_select_own" ON "public"."lineups" FOR SELECT TO "authenticated" USING (("auth"."uid"() = "user_id"));



CREATE POLICY "lineups_update_own" ON "public"."lineups" FOR UPDATE TO "authenticated" USING (("auth"."uid"() = "user_id"));



ALTER TABLE "public"."matches" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "matches_delete_policy" ON "public"."matches" FOR DELETE USING (true);



CREATE POLICY "matches_insert_policy" ON "public"."matches" FOR INSERT WITH CHECK (true);



CREATE POLICY "matches_select_anon" ON "public"."matches" FOR SELECT TO "anon" USING (true);



CREATE POLICY "matches_select_authenticated" ON "public"."matches" FOR SELECT TO "authenticated" USING (true);



CREATE POLICY "matches_select_policy" ON "public"."matches" FOR SELECT USING (true);



CREATE POLICY "matches_update_policy" ON "public"."matches" FOR UPDATE USING (true);



ALTER TABLE "public"."player_stats" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "player_stats_select_anon" ON "public"."player_stats" FOR SELECT TO "anon" USING (true);



CREATE POLICY "player_stats_select_authenticated" ON "public"."player_stats" FOR SELECT TO "authenticated" USING (true);



ALTER TABLE "public"."players" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "players_delete_policy" ON "public"."players" FOR DELETE USING (true);



CREATE POLICY "players_insert_policy" ON "public"."players" FOR INSERT WITH CHECK (true);



CREATE POLICY "players_select_anon" ON "public"."players" FOR SELECT TO "anon" USING (true);



CREATE POLICY "players_select_authenticated" ON "public"."players" FOR SELECT TO "authenticated" USING (true);



CREATE POLICY "players_select_policy" ON "public"."players" FOR SELECT USING (true);



CREATE POLICY "players_update_policy" ON "public"."players" FOR UPDATE USING (true);



ALTER TABLE "public"."scrape_runs" ENABLE ROW LEVEL SECURITY;


ALTER TABLE "public"."teams" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "teams_delete_policy" ON "public"."teams" FOR DELETE USING (true);



CREATE POLICY "teams_insert_policy" ON "public"."teams" FOR INSERT WITH CHECK (true);



CREATE POLICY "teams_select_anon" ON "public"."teams" FOR SELECT TO "anon" USING (true);



CREATE POLICY "teams_select_authenticated" ON "public"."teams" FOR SELECT TO "authenticated" USING (true);



CREATE POLICY "teams_select_policy" ON "public"."teams" FOR SELECT USING (true);



CREATE POLICY "teams_update_policy" ON "public"."teams" FOR UPDATE USING (true);



ALTER TABLE "public"."transfer_tips" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "transfer_tips_delete_own" ON "public"."transfer_tips" FOR DELETE TO "authenticated" USING (("auth"."uid"() = "user_id"));



CREATE POLICY "transfer_tips_insert_own" ON "public"."transfer_tips" FOR INSERT TO "authenticated" WITH CHECK (("auth"."uid"() = "user_id"));



CREATE POLICY "transfer_tips_select_own" ON "public"."transfer_tips" FOR SELECT TO "authenticated" USING (("auth"."uid"() = "user_id"));



CREATE POLICY "transfer_tips_update_own" ON "public"."transfer_tips" FOR UPDATE TO "authenticated" USING (("auth"."uid"() = "user_id"));



ALTER TABLE "public"."tutorial_status" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "tutorial_status_delete_own" ON "public"."tutorial_status" FOR DELETE TO "authenticated" USING (("auth"."uid"() = "user_id"));



CREATE POLICY "tutorial_status_insert_own" ON "public"."tutorial_status" FOR INSERT TO "authenticated" WITH CHECK (("auth"."uid"() = "user_id"));



CREATE POLICY "tutorial_status_select_own" ON "public"."tutorial_status" FOR SELECT TO "authenticated" USING (("auth"."uid"() = "user_id"));



CREATE POLICY "tutorial_status_update_own" ON "public"."tutorial_status" FOR UPDATE TO "authenticated" USING (("auth"."uid"() = "user_id"));



CREATE POLICY "user_import_logs_policy" ON "public"."import_logs" USING (("user_id" = "auth"."uid"()));



ALTER TABLE "public"."users" ENABLE ROW LEVEL SECURITY;


CREATE POLICY "users_insert_own" ON "public"."users" FOR INSERT TO "authenticated" WITH CHECK (("auth"."uid"() = "id"));



CREATE POLICY "users_select_own" ON "public"."users" FOR SELECT TO "authenticated" USING (("auth"."uid"() = "id"));



CREATE POLICY "users_update_own" ON "public"."users" FOR UPDATE TO "authenticated" USING (("auth"."uid"() = "id"));



GRANT USAGE ON SCHEMA "public" TO "postgres";
GRANT USAGE ON SCHEMA "public" TO "anon";
GRANT USAGE ON SCHEMA "public" TO "authenticated";
GRANT USAGE ON SCHEMA "public" TO "service_role";



GRANT ALL ON TABLE "public"."bonuses" TO "anon";
GRANT ALL ON TABLE "public"."bonuses" TO "authenticated";
GRANT ALL ON TABLE "public"."bonuses" TO "service_role";



GRANT ALL ON SEQUENCE "public"."bonuses_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."bonuses_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."bonuses_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."gameweeks" TO "anon";
GRANT ALL ON TABLE "public"."gameweeks" TO "authenticated";
GRANT ALL ON TABLE "public"."gameweeks" TO "service_role";



GRANT ALL ON SEQUENCE "public"."gameweeks_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."gameweeks_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."gameweeks_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."generation_logs" TO "anon";
GRANT ALL ON TABLE "public"."generation_logs" TO "authenticated";
GRANT ALL ON TABLE "public"."generation_logs" TO "service_role";



GRANT ALL ON SEQUENCE "public"."generation_logs_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."generation_logs_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."generation_logs_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."import_logs" TO "anon";
GRANT ALL ON TABLE "public"."import_logs" TO "authenticated";
GRANT ALL ON TABLE "public"."import_logs" TO "service_role";



GRANT ALL ON TABLE "public"."lineup_bonuses" TO "anon";
GRANT ALL ON TABLE "public"."lineup_bonuses" TO "authenticated";
GRANT ALL ON TABLE "public"."lineup_bonuses" TO "service_role";



GRANT ALL ON SEQUENCE "public"."lineup_bonuses_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."lineup_bonuses_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."lineup_bonuses_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."lineup_players" TO "anon";
GRANT ALL ON TABLE "public"."lineup_players" TO "authenticated";
GRANT ALL ON TABLE "public"."lineup_players" TO "service_role";



GRANT ALL ON TABLE "public"."lineups" TO "anon";
GRANT ALL ON TABLE "public"."lineups" TO "authenticated";
GRANT ALL ON TABLE "public"."lineups" TO "service_role";



GRANT ALL ON TABLE "public"."matches" TO "anon";
GRANT ALL ON TABLE "public"."matches" TO "authenticated";
GRANT ALL ON TABLE "public"."matches" TO "service_role";



GRANT ALL ON SEQUENCE "public"."matches_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."matches_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."matches_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."player_stats" TO "anon";
GRANT ALL ON TABLE "public"."player_stats" TO "authenticated";
GRANT ALL ON TABLE "public"."player_stats" TO "service_role";



GRANT ALL ON SEQUENCE "public"."player_stats_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."player_stats_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."player_stats_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."players" TO "anon";
GRANT ALL ON TABLE "public"."players" TO "authenticated";
GRANT ALL ON TABLE "public"."players" TO "service_role";



GRANT ALL ON SEQUENCE "public"."players_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."players_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."players_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."scrape_runs" TO "anon";
GRANT ALL ON TABLE "public"."scrape_runs" TO "authenticated";
GRANT ALL ON TABLE "public"."scrape_runs" TO "service_role";



GRANT ALL ON SEQUENCE "public"."scrape_runs_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."scrape_runs_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."scrape_runs_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."teams" TO "anon";
GRANT ALL ON TABLE "public"."teams" TO "authenticated";
GRANT ALL ON TABLE "public"."teams" TO "service_role";



GRANT ALL ON SEQUENCE "public"."teams_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."teams_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."teams_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."transfer_tips" TO "anon";
GRANT ALL ON TABLE "public"."transfer_tips" TO "authenticated";
GRANT ALL ON TABLE "public"."transfer_tips" TO "service_role";



GRANT ALL ON SEQUENCE "public"."transfer_tips_id_seq" TO "anon";
GRANT ALL ON SEQUENCE "public"."transfer_tips_id_seq" TO "authenticated";
GRANT ALL ON SEQUENCE "public"."transfer_tips_id_seq" TO "service_role";



GRANT ALL ON TABLE "public"."tutorial_status" TO "anon";
GRANT ALL ON TABLE "public"."tutorial_status" TO "authenticated";
GRANT ALL ON TABLE "public"."tutorial_status" TO "service_role";



GRANT ALL ON TABLE "public"."users" TO "anon";
GRANT ALL ON TABLE "public"."users" TO "authenticated";
GRANT ALL ON TABLE "public"."users" TO "service_role";



ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON SEQUENCES TO "postgres";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON SEQUENCES TO "anon";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON SEQUENCES TO "authenticated";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON SEQUENCES TO "service_role";






ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON FUNCTIONS TO "postgres";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON FUNCTIONS TO "anon";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON FUNCTIONS TO "authenticated";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON FUNCTIONS TO "service_role";






ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON TABLES TO "postgres";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON TABLES TO "anon";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON TABLES TO "authenticated";
ALTER DEFAULT PRIVILEGES FOR ROLE "postgres" IN SCHEMA "public" GRANT ALL ON TABLES TO "service_role";







RESET ALL;
