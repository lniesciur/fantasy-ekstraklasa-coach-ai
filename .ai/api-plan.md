# REST API Plan - Fantasy Ekstraklasa Optimizer

## 1. Resources

| Resource | Database Table | Description |
|----------|----------------|-------------|
| Teams | teams | Football teams in Ekstraklasa |
| Players | players | Individual football players |
| Player Stats | player_stats | Per-gameweek player performance statistics |
| Gameweeks | gameweeks | Fantasy football rounds/gameweeks |
| Matches | matches | Football matches between teams |
| Users | users | User profiles (extends Supabase auth.users) |
| Lineups | lineups | User-created fantasy football lineups |
| Lineup Players | lineup_players | Players within specific lineups |
| Bonuses | bonuses | Available fantasy bonuses |
| Lineup Bonuses | lineup_bonuses | Applied bonuses to lineups |
| Transfer Tips | transfer_tips | AI-generated player transfer recommendations |
| Tutorial Status | tutorial_status | User onboarding progress tracking |
| Generation Logs | generation_logs | AI lineup generation tracking |
| Import Logs | import_logs | Data import operation tracking |
| Scrape Runs | scrape_runs | Automated data collection tracking |

## 2. Endpoints

### 2.1 Authentication & User Management

#### POST /api/auth/profile
Create or update user profile after Supabase authentication
- **Request Body:**
```json
{
  "display_name": "string",
  "preferences": {
    "default_formation": "1-4-4-2",
    "tutorial_completed": false
  }
}
```
- **Response (201):**
```json
{
  "id": "uuid",
  "display_name": "string",
  "preferences": {},
  "created_at": "timestamp",
  "updated_at": "timestamp"
}
```
- **Errors:** 400 (Invalid data), 401 (Unauthorized)

#### GET /api/auth/profile
Get current user profile
- **Response (200):** Same as POST response
- **Errors:** 401 (Unauthorized), 404 (Profile not found)

#### PUT /api/auth/profile
Update user profile
- **Request Body:** Same as POST
- **Response (200):** Updated profile object
- **Errors:** 400 (Invalid data), 401 (Unauthorized)

### 2.2 Tutorial Management

#### GET /api/tutorial/status
Get user's tutorial progress
- **Response (200):**
```json
{
  "user_id": "uuid",
  "last_step": 2,
  "skipped": false,
  "updated_at": "timestamp"
}
```
- **Errors:** 401 (Unauthorized)

#### PUT /api/tutorial/status
Update tutorial progress
- **Request Body:**
```json
{
  "last_step": 3,
  "skipped": false
}
```
- **Response (200):** Updated tutorial status
- **Errors:** 400 (Invalid step), 401 (Unauthorized)

### 2.3 Teams

#### GET /api/teams
List all teams (helper endpoint for dropdowns, etc.)
- **Query Parameters:**
  - `sort`: name, position (default: name)
  - `order`: asc, desc (default: asc)
  - `is_active`: boolean (default: true) - filter active/inactive teams
- **Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "name": "Legia Warszawa",
      "short_code": "LEG",
      "crest_url": "https://...",
      "is_active": true,
      "league_position": 3,
      "form": ["W", "W", "D"],
      "stats": {
        "avg_fantasy_points": 58.5,
        "players_count": 25
      }
    }
  ],
  "total": 18
}
```
- **Errors:** 401 (Unauthorized)

#### GET /api/teams/{id}
Get specific team details (helper endpoint)
- **Response (200):**
```json
{
  "id": 1,
  "name": "Legia Warszawa",
  "short_code": "LEG",
  "crest_url": "https://...",
  "is_active": true,
  "league_position": 3,
  "form": ["W", "W", "D"],
  "upcoming_matches": [
    {
      "id": 123,
      "opponent": "Cracovia",
      "home_away": "H",
      "difficulty": "easy",
      "date": "2025-10-20T15:00:00Z"
    }
  ],
  "players": [
    {
      "id": 456,
      "name": "Jan Kowalski",
      "position": "GK",
      "price": 2.5,
      "form": 8.2
    }
  ]
}
```
- **Errors:** 401 (Unauthorized), 404 (Team not found)

#### POST /api/teams
Create new team
- **Request Body:**
```json
{
  "name": "Śląsk Wrocław",
  "short_code": "SLA",
  "crest_url": "https://example.com/crests/slask.png",
  "is_active": true
}
```
- **Response (201):**
```json
{
  "id": 19,
  "name": "Śląsk Wrocław",
  "short_code": "SLA",
  "crest_url": "https://example.com/crests/slask.png",
  "is_active": true,
  "created_at": "2025-10-16T10:00:00Z",
  "updated_at": "2025-10-16T10:00:00Z"
}
```
- **Errors:** 400 (Validation error), 401 (Unauthorized), 409 (Name or short_code already exists)

#### PUT /api/teams/{id}
Update existing team
- **Request Body:**
```json
{
  "name": "Śląsk Wrocław",
  "short_code": "SLA",
  "crest_url": "https://example.com/crests/slask-new.png",
  "is_active": true
}
```
- **Response (200):**
```json
{
  "id": 19,
  "name": "Śląsk Wrocław",
  "short_code": "SLA",
  "crest_url": "https://example.com/crests/slask-new.png",
  "is_active": true,
  "created_at": "2025-10-16T10:00:00Z",
  "updated_at": "2025-10-16T12:00:00Z"
}
```
- **Errors:** 400 (Validation error), 401 (Unauthorized), 404 (Team not found), 409 (Name or short_code conflict)

### 2.4 Players

#### GET /api/players
List players with filtering and sorting
- **Query Parameters:**
  - `position`: GK, DEF, MID, FWD
  - `team_id`: integer
  - `min_price`: decimal
  - `max_price`: decimal
  - `health_status`: Pewny, Wątpliwy, Nie zagra
  - `min_form`: decimal
  - `max_form`: decimal
  - `search`: string (player name)
  - `sort`: name, price, form, fantasy_points (default: name)
  - `order`: asc, desc (default: asc)
  - `page`: integer (default: 1)
  - `limit`: integer (default: 50, max: 100)
- **Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "name": "Robert Lewandowski",
      "team": {
        "id": 1,
        "name": "Legia Warszawa",
        "short_code": "LEG"
      },
      "position": "FWD",
      "price": 4.1,
      "health_status": "Pewny",
      "predicted_start": true,
      "current_stats": {
        "fantasy_points": 125,
        "form": 9.2,
        "goals": 15,
        "assists": 8,
        "yellow_cards": 2,
        "red_cards": 0
      },
      "upcoming_fixtures": [
        {
          "opponent": "Cracovia",
          "home_away": "H",
          "difficulty": "easy",
          "date": "2025-10-20T15:00:00Z"
        }
      ]
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 50,
    "total": 487,
    "pages": 10
  },
  "filters_applied": {
    "position": "FWD",
    "min_price": 2.0
  }
}
```
- **Errors:** 400 (Invalid parameters), 401 (Unauthorized)

#### GET /api/players/{id}
Get detailed player information
- **Response (200):**
```json
{
  "id": 1,
  "name": "Robert Lewandowski",
  "team": {
    "id": 1,
    "name": "Legia Warszawa",
    "short_code": "LEG"
  },
  "position": "FWD",
  "price": 4.1,
  "health_status": "Pewny",
  "predicted_start": true,
  "current_stats": {
    "fantasy_points": 125,
    "form": 9.2,
    "goals": 15,
    "assists": 8,
    "yellow_cards": 2,
    "red_cards": 0,
    "minutes_played": 1350,
    "saves": 0,
    "penalties_won": 3
  },
  "recent_performances": [
    {
      "gameweek": 14,
      "opponent": "Cracovia",
      "home_away": "H",
      "fantasy_points": 12,
      "minutes_played": 90,
      "goals": 2,
      "assists": 1
    }
  ],
  "upcoming_fixtures": [
    {
      "gameweek": 16,
      "opponent": "Wisła",
      "home_away": "A",
      "difficulty": "medium",
      "date": "2025-10-27T17:00:00Z"
    }
  ],
  "season_stats": {
    "appearances": 15,
    "avg_fantasy_points": 8.3,
    "total_goals": 15,
    "total_assists": 8
  }
}
```
- **Errors:** 401 (Unauthorized), 404 (Player not found)

#### POST /api/players
Create new player
- **Request Body:**
```json
{
  "name": "Arkadiusz Milik",
  "team_id": 1,
  "position": "FWD",
  "price": 3.8,
  "health_status": "Pewny"
}
```
- **Response (201):**
```json
{
  "id": 488,
  "name": "Arkadiusz Milik",
  "team": {
    "id": 1,
    "name": "Legia Warszawa",
    "short_code": "LEG"
  },
  "position": "FWD",
  "price": 3.8,
  "health_status": "Pewny",
  "created_at": "2025-10-16T10:00:00Z",
  "updated_at": "2025-10-16T10:00:00Z"
}
```
- **Errors:** 400 (Validation error), 401 (Unauthorized), 404 (Team not found)

#### PUT /api/players/{id}
Update existing player
- **Request Body:**
```json
{
  "name": "Arkadiusz Milik",
  "team_id": 2,
  "position": "FWD",
  "price": 4.1,
  "health_status": "Wątpliwy"
}
```
- **Response (200):**
```json
{
  "id": 488,
  "name": "Arkadiusz Milik",
  "team": {
    "id": 2,
    "name": "Cracovia",
    "short_code": "CRA"
  },
  "position": "FWD",
  "price": 4.1,
  "health_status": "Wątpliwy",
  "created_at": "2025-10-16T10:00:00Z",
  "updated_at": "2025-10-16T12:00:00Z"
}
```
- **Errors:** 400 (Validation error), 401 (Unauthorized), 404 (Player or Team not found)

### 2.5 Player Statistics

#### GET /api/player-stats
Get player statistics for specific gameweek
- **Query Parameters:**
  - `gameweek_id`: integer (required)
  - `player_id`: integer (optional)
  - `team_id`: integer (optional)
  - `position`: GK, DEF, MID, FWD (optional)
  - `sort`: fantasy_points, form, price (default: fantasy_points)
  - `order`: asc, desc (default: desc)
  - `limit`: integer (default: 50, max: 100)
  - `page`: integer (default: 1)

- **Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "player": {
        "id": 1,
        "name": "Robert Lewandowski",
        "position": "FWD"
      },
      "gameweek": {
        "id": 15,
        "number": 15
      },
      "fantasy_points": 12,
      "minutes_played": 90,
      "goals": 2,
      "assists": 1,
      "yellow_cards": 0,
      "red_cards": 0,
      "saves": 0,
      "penalties_saved": 0,
      "penalties_won": 1,
      "penalties_scored": 1,
      "penalties_caused": 0,
      "penalties_missed": 0,
      "lotto_assists": 0,
      "own_goals": 0,
      "in_team_of_week": true,
      "price": 4.1,
      "predicted_start": true,
      "health_status": "Pewny"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 50,
    "total": 487,
    "pages": 10
  }
}
```

- **Errors:** 400 (Invalid parameters), 401 (Unauthorized), 404 (Gameweek not found)

#### POST /api/player-stats/import
Import player statistics from CSV file
- **Request:** Multipart form data
  - `file`: CSV file with player statistics
  - `gameweek_id`: integer (required)

- **CSV Column Format** (all optional except player_id, gameweek_id, fantasy_points, minutes_played, price):
  - `player_id` - integer
  - `match_id` - integer (optional)
  - `fantasy_points` - integer (0-100)
  - `minutes_played` - integer (0-120)
  - `goals` - integer (default: 0)
  - `assists` - integer (default: 0)
  - `yellow_cards` - integer (default: 0)
  - `red_cards` - integer (default: 0)
  - `saves` - integer (default: 0)
  - `penalties_saved` - integer (default: 0)
  - `penalties_won` - integer (default: 0)
  - `penalties_scored` - integer (default: 0)
  - `penalties_caused` - integer (default: 0)
  - `penalties_missed` - integer (default: 0)
  - `lotto_assists` - integer (default: 0)
  - `own_goals` - integer (default: 0)
  - `in_team_of_week` - boolean (default: false)
  - `price` - decimal
  - `predicted_start` - boolean (default: false)
  - `health_status` - enum (Pewny, Wątpliwy, Nie zagra, default: Pewny)

- **Response (201):**
```json
{
  "success": true,
  "imported_count": 148,
  "skipped_count": 2,
  "errors": [
    "Row 5: Invalid player_id",
    "Row 12: Missing fantasy_points"
  ]
}
```

- **Errors:** 400 (Invalid file or data), 401 (Unauthorized), 413 (File too large)

### 2.6 Gameweeks

#### GET /api/gameweeks
List all gameweeks
- **Query Parameters:**
  - `status`: upcoming, current, completed
  - `sort`: number, start_date (default: number)
  - `order`: asc, desc (default: asc)
- **Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "number": 15,
      "start_date": "2025-10-20",
      "end_date": "2025-10-22",
      "status": "upcoming",
      "matches_count": 9
    }
  ]
}
```
- **Errors:** 401 (Unauthorized)

#### GET /api/gameweeks/current
Get current active gameweek
- **Response (200):** Single gameweek object
- **Errors:** 401 (Unauthorized), 404 (No active gameweek)

#### GET /api/gameweeks/{id}
Get specific gameweek with matches
- **Response (200):**
```json
{
  "id": 1,
  "number": 15,
  "start_date": "2025-10-20",
  "end_date": "2025-10-22",
  "status": "upcoming",
  "matches": [
    {
      "id": 1,
      "home_team": {
        "id": 1,
        "name": "Legia",
        "short_code": "LEG"
      },
      "away_team": {
        "id": 2,
        "name": "Cracovia",
        "short_code": "CRA"
      },
      "match_date": "2025-10-20T15:00:00Z",
      "status": "scheduled"
    }
  ]
}
```
- **Errors:** 401 (Unauthorized), 404 (Gameweek not found)

#### POST /api/gameweeks
Create new gameweek
- **Request Body:**
```json
{
  "number": 16,
  "start_date": "2025-10-27",
  "end_date": "2025-10-29"
}
```
- **Response (201):**
```json
{
  "id": 16,
  "number": 16,
  "start_date": "2025-10-27",
  "end_date": "2025-10-29",
  "status": "upcoming",
  "matches_count": 0,
  "created_at": "2025-10-16T10:00:00Z"
}
```
- **Errors:** 400 (Validation error), 401 (Unauthorized), 409 (Gameweek number already exists)

#### PUT /api/gameweeks/{id}
Update existing gameweek
- **Request Body:**
```json
{
  "number": 16,
  "start_date": "2025-10-28",
  "end_date": "2025-10-30"
}
```
- **Response (200):**
```json
{
  "id": 16,
  "number": 16,
  "start_date": "2025-10-28",
  "end_date": "2025-10-30",
  "status": "upcoming",
  "matches_count": 9,
  "created_at": "2025-10-16T10:00:00Z",
  "updated_at": "2025-10-16T12:00:00Z"
}
```
- **Errors:** 400 (Validation error), 401 (Unauthorized), 404 (Gameweek not found), 409 (Number conflict)

### 2.7 Matches

#### GET /api/matches
List matches with filtering
- **Query Parameters:**
  - `gameweek_id`: integer
  - `team_id`: integer (matches involving specific team)
  - `status`: scheduled, postponed, cancelled, played
  - `date_from`: date (YYYY-MM-DD)
  - `date_to`: date (YYYY-MM-DD)
  - `sort`: match_date, gameweek_number (default: match_date)
  - `order`: asc, desc (default: asc)
  - `page`: integer (default: 1)
  - `limit`: integer (default: 50, max: 100)
- **Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "gameweek": {
        "id": 15,
        "number": 15
      },
      "home_team": {
        "id": 1,
        "name": "Legia Warszawa",
        "short_code": "LEG"
      },
      "away_team": {
        "id": 2,
        "name": "Cracovia",
        "short_code": "CRA"
      },
      "match_date": "2025-10-20T15:00:00Z",
      "status": "scheduled",
      "home_score": null,
      "away_score": null,
      "reschedule_reason": null
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 50,
    "total": 180,
    "pages": 4
  }
}
```
- **Errors:** 400 (Invalid parameters), 401 (Unauthorized)

#### GET /api/matches/{id}
Get specific match details
- **Response (200):**
```json
{
  "id": 1,
  "gameweek": {
    "id": 15,
    "number": 15,
    "start_date": "2025-10-20",
    "end_date": "2025-10-22"
  },
  "home_team": {
    "id": 1,
    "name": "Legia Warszawa",
    "short_code": "LEG"
  },
  "away_team": {
    "id": 2,
    "name": "Cracovia",
    "short_code": "CRA"
  },
  "match_date": "2025-10-20T15:00:00Z",
  "status": "scheduled",
  "home_score": null,
  "away_score": null,
  "reschedule_reason": null,
  "created_at": "2025-10-16T08:00:00Z",
  "updated_at": "2025-10-16T08:00:00Z"
}
```
- **Errors:** 401 (Unauthorized), 404 (Match not found)

#### POST /api/matches
Create new match
- **Request Body:**
```json
{
  "gameweek_id": 15,
  "home_team_id": 1,
  "away_team_id": 2,
  "match_date": "2025-10-20T15:00:00Z",
  "status": "scheduled"
}
```
- **Response (201):**
```json
{
  "id": 181,
  "gameweek": {
    "id": 15,
    "number": 15
  },
  "home_team": {
    "id": 1,
    "name": "Legia Warszawa",
    "short_code": "LEG"
  },
  "away_team": {
    "id": 2,
    "name": "Cracovia",
    "short_code": "CRA"
  },
  "match_date": "2025-10-20T15:00:00Z",
  "status": "scheduled",
  "home_score": null,
  "away_score": null,
  "reschedule_reason": null,
  "created_at": "2025-10-16T10:00:00Z",
  "updated_at": "2025-10-16T10:00:00Z"
}
```
- **Errors:** 400 (Validation error), 401 (Unauthorized), 404 (Gameweek or Team not found)

#### PUT /api/matches/{id}
Update existing match
- **Request Body:**
```json
{
  "gameweek_id": 15,
  "home_team_id": 1,
  "away_team_id": 2,
  "match_date": "2025-10-20T17:00:00Z",
  "status": "played",
  "home_score": 2,
  "away_score": 1,
  "reschedule_reason": null
}
```
- **Response (200):**
```json
{
  "id": 181,
  "gameweek": {
    "id": 15,
    "number": 15
  },
  "home_team": {
    "id": 1,
    "name": "Legia Warszawa",
    "short_code": "LEG"
  },
  "away_team": {
    "id": 2,
    "name": "Cracovia",
    "short_code": "CRA"
  },
  "match_date": "2025-10-20T17:00:00Z",
  "status": "played",
  "home_score": 2,
  "away_score": 1,
  "reschedule_reason": null,
  "created_at": "2025-10-16T10:00:00Z",
  "updated_at": "2025-10-16T15:00:00Z"
}
```
- **Errors:** 400 (Validation error), 401 (Unauthorized), 404 (Match, Gameweek or Team not found)

### 2.8 Lineups

#### GET /api/lineups
List user's lineups
- **Query Parameters:**
  - `gameweek_id`: integer
  - `is_active`: boolean
  - `sort`: created_at, updated_at, name (default: updated_at)
  - `order`: asc, desc (default: desc)
- **Response (200):**
```json
{
  "data": [
    {
      "id": "uuid",
      "name": "Kolejka 15 - Mocna forma",
      "gameweek_id": 15,
      "is_active": true,
      "total_cost": 28.5,
      "formation": "1-4-4-2",
      "created_at": "timestamp",
      "updated_at": "timestamp",
      "players_count": 15,
      "bonuses_applied": ["Kapitanów 2"],
      "generation_source": "AI",
      "modifications_count": 2
    }
  ],
  "total": 3,
  "limit": 3
}
```
- **Errors:** 401 (Unauthorized)

#### GET /api/lineups/{id}
Get detailed lineup information
- **Response (200):**
```json
{
  "id": "uuid",
  "name": "Kolejka 15 - Mocna forma",
  "gameweek": {
    "id": 15,
    "number": 15,
    "start_date": "2025-10-20"
  },
  "is_active": true,
  "total_cost": 28.5,
  "formation": "1-4-4-2",
  "players": {
    "starting": [
      {
        "player": {
          "id": 1,
          "name": "Jan Kowalski",
          "team": "Legia",
          "position": "GK",
          "price": 2.5
        },
        "role": "starting",
        "is_captain": false,
        "is_vice": false,
        "is_locked": false
      }
    ],
    "bench": [
      {
        "player": {
          "id": 2,
          "name": "Adam Nowak",
          "team": "Cracovia",
          "position": "GK",
          "price": 1.8
        },
        "role": "bench",
        "is_captain": false,
        "is_vice": false,
        "is_locked": false
      }
    ]
  },
  "captain": {
    "player_id": 5,
    "name": "Robert Lewandowski"
  },
  "vice_captain": {
    "player_id": 8,
    "name": "Kamil Grosicki"
  },
  "bonuses": [
    {
      "id": 2,
      "name": "Kapitanów 2",
      "applied_at": "timestamp"
    }
  ],
  "budget_remaining": 1.5,
  "team_distribution": {
    "Legia": 3,
    "Cracovia": 2,
    "Wisła": 1
  },
  "generation_info": {
    "source": "AI",
    "model": "Claude 3.5 Sonnet",
    "generation_time": 18,
    "modifications_count": 2,
    "strategy": "Balanced"
  },
  "created_at": "timestamp",
  "updated_at": "timestamp"
}
```
- **Errors:** 401 (Unauthorized), 403 (Not owner), 404 (Lineup not found)

#### POST /api/lineups
Create new lineup
- **Request Body:**
```json
{
  "name": "My Lineup",
  "gameweek_id": 15,
  "formation": "1-4-4-2",
  "is_active": true,
  "players": [
    {
      "player_id": 1,
      "role": "starting",
      "is_captain": true,
      "is_vice": false
    }
  ],
  "bonuses": [2]
}
```
- **Response (201):** Created lineup object
- **Errors:** 400 (Validation error), 401 (Unauthorized), 409 (Name conflict)

#### PUT /api/lineups/{id}
Update existing lineup
- **Request Body:** Same as POST
- **Response (200):** Updated lineup object
- **Errors:** 400 (Validation error), 401 (Unauthorized), 403 (Not owner), 404 (Not found)

#### DELETE /api/lineups/{id}
Delete lineup
- **Response (204):** No content
- **Errors:** 401 (Unauthorized), 403 (Not owner), 404 (Not found)

#### POST /api/lineups/{id}/duplicate
Create copy of existing lineup
- **Request Body:**
```json
{
  "name": "Copy of Original",
  "gameweek_id": 16
}
```
- **Response (201):** New lineup object
- **Errors:** 400 (Validation error), 401 (Unauthorized), 403 (Not owner)

#### GET /api/lineups/{id}/export
Export lineup as text
- **Response (200):**
```json
{
  "format": "text",
  "content": "SKŁAD - Kolejka 15 (1-4-4-2)\nPODSTAWOWI:\nGK: Jan Kowalski (Legia)\n..."
}
```
- **Errors:** 401 (Unauthorized), 403 (Not owner), 404 (Not found)

### 2.9 AI Lineup Generation

#### POST /api/ai/generate-lineup
Generate optimized lineup using AI
- **Request Body:**
```json
{
  "gameweek_id": 15,
  "formation": "1-4-4-2",
  "budget": 30.0,
  "locked_players": [1, 5, 12],
  "strategy": "balanced",
  "weights": {
    "form": 0.4,
    "fantasy_points": 0.3,
    "budget_optimization": 0.2,
    "team_form": 0.1
  },
  "constraints": {
    "max_players_per_team": 3,
    "require_predicted_start": true,
    "exclude_health_status": ["Nie zagra"]
  }
}
```
- **Response (200):**
```json
{
  "lineup": {
    "formation": "1-4-4-2",
    "total_cost": 28.5,
    "budget_remaining": 1.5,
    "players": {
      "starting": [
        {
          "player": {
            "id": 1,
            "name": "Jan Kowalski",
            "team": "Legia",
            "position": "GK",
            "price": 2.5,
            "form": 8.2
          },
          "role": "starting",
          "is_captain": false,
          "is_vice": false,
          "selection_reason": "High form (8.2), good price value"
        }
      ],
      "bench": []
    },
    "captain": {
      "player_id": 5,
      "name": "Robert Lewandowski",
      "selection_reason": "Highest form (9.2), easy fixtures"
    },
    "vice_captain": {
      "player_id": 8,
      "name": "Kamil Grosicki"
    }
  },
  "generation_info": {
    "model": "Claude 3.5 Sonnet",
    "generation_time": 18,
    "strategy": "balanced",
    "confidence": 0.87
  },
  "recommendations": {
    "suggested_bonus": {
      "id": 2,
      "name": "Kapitanów 2",
      "reason": "2 players with high form (>8) and easy fixtures = ~15-20 extra points",
      "preview": {
        "estimated_additional_points": 18
      }
    },
    "joker_candidates": [
      {
        "player_id": 15,
        "name": "Michał Kowalski",
        "price": 1.8,
        "form": 7.5,
        "reason": "Best form under 2.0M budget"
      }
    ]
  },
  "validation": {
    "budget_valid": true,
    "formation_valid": true,
    "team_limits_valid": true,
    "locked_players_included": true
  }
}
```
- **Errors:** 400 (Invalid parameters), 401 (Unauthorized), 408 (Generation timeout), 500 (AI service error)

#### GET /api/ai/generation-history
Get user's AI generation history
- **Query Parameters:**
  - `limit`: integer (default: 10)
  - `offset`: integer (default: 0)
- **Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "gameweek_id": 15,
      "model": "Claude 3.5 Sonnet",
      "generation_time": 18,
      "success": true,
      "strategy": "balanced",
      "lineup_saved": true,
      "created_at": "timestamp"
    }
  ],
  "total": 25
}
```
- **Errors:** 401 (Unauthorized)

### 2.10 Bonuses

#### GET /api/bonuses
List available bonuses
- **Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "name": "Ławka punktuje",
      "description": "Punkty zawodników rezerwowych liczą się dodatkowo"
    },
    {
      "id": 2,
      "name": "Kapitanów 2",
      "description": "2 zawodników punktuje podwójnie"
    },
    {
      "id": 3,
      "name": "Joker",
      "description": "Najlepszy zawodnik ≤2.0M (nie kapitan) punktuje podwójnie"
    }
  ]
}
```
- **Errors:** 401 (Unauthorized)

#### GET /api/bonuses/status
Get user's bonus usage status for current round
- **Response (200):**
```json
{
  "current_round": 1,
  "bonuses": [
    {
      "id": 1,
      "name": "Ławka punktuje",
      "status": "available",
      "used_in_gameweek": null
    },
    {
      "id": 2,
      "name": "Kapitanów 2",
      "status": "used",
      "used_in_gameweek": 14
    }
  ],
  "ai_recommendation": {
    "bonus_id": 1,
    "reason": "You have strong bench players this gameweek"
  }
}
```
- **Errors:** 401 (Unauthorized)

#### POST /api/bonuses/preview
Preview bonus effects on lineup
- **Request Body:**
```json
{
  "lineup_id": "uuid",
  "bonus_id": 2
}
```
- **Response (200):**
```json
{
  "bonus": {
    "id": 2,
    "name": "Kapitanów 2"
  },
  "preview": {
    "affected_players": [
      {
        "player_id": 5,
        "name": "Robert Lewandowski",
        "estimated_points": 12,
        "doubled_points": 24
      }
    ],
    "estimated_additional_points": 18,
    "total_estimated_points": 73
  },
  "recommendation": {
    "should_use": true,
    "confidence": 0.85,
    "reason": "High form players with easy fixtures"
  }
}
```
- **Errors:** 400 (Invalid bonus/lineup), 401 (Unauthorized)

### 2.11 Transfer Tips

#### GET /api/transfer-tips
Get AI-generated transfer recommendations
- **Query Parameters:**
  - `gameweek_id`: integer (default: current)
  - `limit`: integer (default: 5)
- **Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "priority": 1,
      "transfer_type": "upgrade",
      "out_player": {
        "id": 10,
        "name": "Michał Słaby",
        "team": "Wisła",
        "position": "MID",
        "price": 2.8,
        "recent_form": 2.5,
        "reason": "Poor form (2 gameweeks with 1 point), difficult fixtures"
      },
      "in_player": {
        "id": 25,
        "name": "Kamil Mocny",
        "team": "Legia",
        "position": "MID",
        "price": 3.0,
        "recent_form": 8.5,
        "reason": "Excellent form (8, 11 points), easy fixtures ahead"
      },
      "price_difference": 0.2,
      "confidence": 0.89,
      "potential_points_gain": 8,
      "fixtures_comparison": {
        "out_player_difficulty": "hard",
        "in_player_difficulty": "easy"
      }
    }
  ],
  "summary": {
    "total_recommendations": 5,
    "high_priority": 2,
    "potential_budget_change": -0.3
  }
}
```
- **Errors:** 401 (Unauthorized)

#### POST /api/transfer-tips/{id}/apply
Apply transfer recommendation to active lineup
- **Response (200):**
```json
{
  "success": true,
  "updated_lineup_id": "uuid",
  "transfer_applied": {
    "out_player": "Michał Słaby",
    "in_player": "Kamil Mocny",
    "price_difference": 0.2
  },
  "new_total_cost": 28.7
}
```
- **Errors:** 400 (Invalid transfer), 401 (Unauthorized), 404 (No active lineup)

#### POST /api/transfer-tips/{id}/dismiss
Dismiss transfer recommendation
- **Response (204):** No content
- **Errors:** 401 (Unauthorized), 404 (Tip not found)

### 2.12 History & Analytics

#### GET /api/history/lineups
Get user's lineup history
- **Query Parameters:**
  - `limit`: integer (default: 10)
  - `gameweek_from`: integer
  - `gameweek_to`: integer
  - `filter`: all, ai_only, modified_only
- **Response (200):**
```json
{
  "data": [
    {
      "gameweek": 14,
      "lineup": {
        "id": "uuid",
        "name": "Kolejka 14",
        "total_points": 67,
        "generation_source": "AI",
        "modifications_count": 2
      },
      "ai_comparison": {
        "ai_points": 63,
        "user_points": 67,
        "difference": 4,
        "user_performed_better": true
      },
      "bonus_used": "Kapitanów 2",
      "bonus_effectiveness": 16
    }
  ],
  "summary": {
    "total_gameweeks": 10,
    "average_points": 62.3,
    "ai_acceptance_rate": 0.75,
    "average_ai_points": 58.1,
    "average_user_points": 62.3,
    "improvement_over_ai": 4.2
  }
}
```
- **Errors:** 401 (Unauthorized)

#### GET /api/history/performance
Get detailed performance analytics
- **Response (200):**
```json
{
  "user_stats": {
    "total_lineups": 15,
    "ai_acceptance_rate": 0.75,
    "average_points": 62.3,
    "lineups_over_50_points": 0.87,
    "best_gameweek": {
      "gameweek": 12,
      "points": 89
    },
    "worst_gameweek": {
      "gameweek": 8,
      "points": 32
    }
  },
  "ai_comparison": {
    "ai_average_points": 58.1,
    "user_average_points": 62.3,
    "user_improvement": 4.2,
    "times_user_better": 12,
    "times_ai_better": 3
  },
  "modification_analysis": {
    "most_modified_position": "FWD",
    "most_profitable_modification": {
      "gameweek": 12,
      "player_out": "Jan Słaby",
      "player_in": "Robert Mocny",
      "points_gained": 15
    },
    "average_modifications_per_lineup": 1.8
  },
  "trend": {
    "recent_form": "improving",
    "last_5_average": 68.2,
    "season_average": 62.3
  }
}
```
- **Errors:** 401 (Unauthorized)

#### GET /api/history/ai-effectiveness
Get AI system performance metrics
- **Response (200):**
```json
{
  "overall_metrics": {
    "lineups_generated": 1250,
    "acceptance_rate": 0.78,
    "lineups_over_50_points": 0.82,
    "average_points": 59.2
  },
  "user_satisfaction": {
    "average_modification_count": 1.3,
    "users_improving_ai": 0.68,
    "average_improvement": 3.1
  },
  "recent_trends": {
    "last_30_days": {
      "acceptance_rate": 0.81,
      "average_points": 61.5
    }
  }
}
```
- **Errors:** 401 (Unauthorized)

### 2.13 Data Management

#### GET /api/data/status
Get data freshness and import status
- **Response (200):**
```json
{
  "last_update": "2025-10-16T06:00:00Z",
  "status": "fresh",
  "age_hours": 2,
  "next_scheduled_update": "2025-10-17T06:00:00Z",
  "data_completeness": {
    "players": 0.98,
    "player_stats": 0.95,
    "matches": 1.0
  },
  "last_scrape": {
    "success": true,
    "duration_seconds": 45,
    "updated_players": 487,
    "updated_stats": 450
  }
}
```
- **Errors:** 401 (Unauthorized)

#### POST /api/data/refresh
Manually trigger data update
- **Response (202):**
```json
{
  "message": "Data refresh initiated",
  "job_id": "uuid",
  "estimated_duration": 60
}
```
- **Errors:** 401 (Unauthorized), 429 (Rate limited)

#### GET /api/data/refresh/{job_id}
Check data refresh job status
- **Response (200):**
```json
{
  "job_id": "uuid",
  "status": "completed",
  "started_at": "timestamp",
  "completed_at": "timestamp",
  "duration_seconds": 52,
  "result": {
    "success": true,
    "players_updated": 487,
    "stats_updated": 450,
    "errors": []
  }
}
```
- **Errors:** 401 (Unauthorized), 404 (Job not found)

#### POST /api/data/import
Upload file for data import
- **Request:** Multipart form data with file
- **Response (202):**
```json
{
  "import_id": "uuid",
  "filename": "players_data.xlsx",
  "file_size": 156789,
  "validation_id": "uuid",
  "status": "validating"
}
```
- **Errors:** 400 (Invalid file), 401 (Unauthorized), 413 (File too large)

#### GET /api/data/import/{import_id}
Check import job status
- **Response (200):**
```json
{
  "id": "uuid",
  "filename": "players_data.xlsx",
  "status": "completed",
  "validation_results": {
    "valid_rows": 487,
    "warnings": 5,
    "errors": 0,
    "issues": [
      {
        "row": 15,
        "column": "price",
        "message": "Price seems high but valid",
        "type": "warning"
      }
    ]
  },
  "import_results": {
    "players_total": 487,
    "players_imported": 487,
    "players_updated": 245,
    "players_created": 242,
    "duration_seconds": 12
  }
}
```
- **Errors:** 401 (Unauthorized), 404 (Import not found)

#### POST /api/data/import/{import_id}/confirm
Confirm and execute import after validation
- **Request Body:**
```json
{
  "overwrite_mode": "update_only"
}
```
- **Response (200):**
```json
{
  "success": true,
  "import_executed": true,
  "summary": {
    "players_imported": 487,
    "players_updated": 245,
    "players_created": 242
  }
}
```
- **Errors:** 400 (Validation failed), 401 (Unauthorized)

### 2.14 Admin & Monitoring

#### GET /api/admin/dashboard
Get system health dashboard (admin only)
- **Response (200):**
```json
{
  "data_quality": {
    "freshness": {
      "status": "good",
      "last_update": "2025-10-16T06:00:00Z",
      "age_hours": 2
    },
    "completeness": 0.98,
    "scraping_success_rate": 0.97
  },
  "users": {
    "total_registered": 156,
    "monthly_active": 89,
    "new_last_7_days": 12,
    "retention_rate": 0.67
  },
  "ai_performance": {
    "acceptance_rate": 0.78,
    "success_rate": 0.82,
    "average_points": 59.2,
    "api_cost_month": 45.67
  },
  "system": {
    "api_calls_today": 2341,
    "average_response_time": 245,
    "error_rate": 0.012,
    "uptime": 0.9987
  }
}
```
- **Errors:** 401 (Unauthorized), 403 (Not admin)

#### GET /api/admin/logs
Get system logs (admin only)
- **Query Parameters:**
  - `level`: error, warn, info, debug
  - `service`: api, scraper, ai
  - `limit`: integer (default: 100)
  - `from`: timestamp
  - `to`: timestamp
- **Response (200):**
```json
{
  "data": [
    {
      "timestamp": "2025-10-16T08:30:15Z",
      "level": "error",
      "service": "scraper",
      "message": "Failed to parse player data for team Legia",
      "details": {
        "error": "Timeout after 30s",
        "retry_count": 3
      }
    }
  ],
  "total": 1205
}
```
- **Errors:** 401 (Unauthorized), 403 (Not admin)

#### POST /api/admin/scrape/trigger
Manually trigger scraping job (admin only)
- **Request Body:**
```json
{
  "run_type": "manual",
  "gameweek_id": 15
}
```
- **Response (202):**
```json
{
  "job_id": "uuid",
  "message": "Scraping job started"
}
```
- **Errors:** 401 (Unauthorized), 403 (Not admin)

## 3. Authentication and Authorization

### 3.1 Authentication Method
- **Primary Authentication:** Supabase Auth with JWT tokens
- **Session Management:** Supabase handles token refresh automatically
- **Token Validation:** Middleware validates JWT on all protected endpoints

### 3.2 Authorization Levels
1. **Public Access:** Data status, team information (read-only)
2. **Authenticated Users:** All player data, lineup management, AI generation
3. **Admin Users:** System monitoring, manual operations, logs access, data management (teams, players, gameweeks, matches CRUD)

### 3.3 Row Level Security (RLS)
- Implemented at database level using Supabase RLS
- Users can only access their own lineups, transfer tips, tutorial status
- Admin users have elevated permissions for monitoring tables

### 3.4 Rate Limiting
- **Manual Data Refresh:** 1 request per hour per user
- **AI Lineup Generation:** 10 requests per hour per user
- **Data Import:** 5 files per day per user
- **API Calls:** 1000 requests per hour per user (general limit)

## 4. Validation and Business Logic

### 4.1 Database Constraints Validation
- **Player Position:** Must be one of ['GK', 'DEF', 'MID', 'FWD']
- **Health Status:** Must be one of ['Pewny', 'Wątpliwy', 'Nie zagra']
- **Match Status:** Must be one of ['scheduled', 'postponed', 'cancelled', 'played']
- **Lineup Role:** Must be one of ['starting', 'bench']
- **Price:** Must be positive decimal with 2 decimal places
- **Team Name:** Required text field, must be unique
- **Team Short Code:** Required varchar(10), must be unique, alphanumeric
- **Team Is Active:** Boolean field, defaults to true
- **Player Name:** Required text field
- **Player Position:** Must be one of ['GK', 'DEF', 'MID', 'FWD']
- **Player Price:** Required numeric(6,2), must be positive
- **Gameweek Number:** Required integer, must be unique
- **Gameweek Dates:** start_date and end_date required, end_date must be after start_date
- **Match Date:** Required timestamp with time zone
- **Match Teams:** home_team_id and away_team_id required, must be different teams
- **Match Scores:** Optional smallint, only for played matches
- **Unique Constraints:** Team names, team short codes, gameweek numbers, user lineup names per gameweek

### 4.2 Business Logic Rules

#### Team Management
- **Team Creation:** Name and short_code must be unique across all teams
- **Team Deletion:** Cannot delete team if it has active players (referential integrity)
- **Team Status:** is_active field allows soft deletion instead of hard deletion
- **Short Code Format:** Must be alphanumeric, maximum 10 characters, typically 3-4 characters
- **Crest URL:** Optional field, should be valid URL when provided

#### Player Management
- **Player Creation:** Name and team_id are required
- **Player Position:** Must be valid position (GK, DEF, MID, FWD)
- **Player Price:** Must be positive, two decimal places precision
- **Health Status:** Must be one of valid statuses (Pewny, Wątpliwy, Nie zagra)
- **Team Assignment:** Referenced team must exist and be active
- **Price Updates:** Should track historical changes for fantasy calculations

#### Gameweek Management
- **Gameweek Number:** Must be unique, typically sequential (1-38 for season)
- **Date Validation:** end_date must be after start_date
- **Date Logic:** Gameweeks cannot overlap in dates
- **Current Gameweek:** Only one gameweek can be "current" at a time
- **Gameweek Deletion:** Cannot delete if has associated matches or lineups

#### Match Management
- **Team Validation:** home_team_id and away_team_id must be different teams
- **Team References:** Both teams must exist and be active
- **Gameweek Association:** Referenced gameweek must exist
- **Score Logic:** Scores only valid when status = 'played'
- **Status Transitions:** scheduled → played/postponed/cancelled (no backwards)
- **Reschedule Logic:** reschedule_reason required when status = 'postponed'

#### Lineup Validation
- **Budget Constraint:** Total cost ≤ 30.0M
- **Formation Validation:** Must match selected formation (e.g., 1-4-4-2 requires 1 GK, 4 DEF, 4 MID, 2 FWD)
- **Team Limits:** Maximum 3 players from any single team
- **Captain Rules:** Exactly 1 captain and 1 vice-captain, both must be in starting XI
- **Squad Size:** Exactly 15 players (11 starting + 4 bench)
- **Position Requirements:** Minimum players per position based on formation

#### AI Generation Constraints
- **Predicted Start Priority:** Prefer players with predicted_start = true
- **Health Status Filter:** Exclude players with health_status = 'Nie zagra'
- **Locked Players:** Must include all specified locked players
- **Budget Optimization:** Maximize value within budget constraints
- **Form Weighting:** Apply specified weights to form, points, budget, team form

#### Bonus System Rules
- **Usage Limit:** Each bonus can be used only once per round (half-season)
- **Joker Eligibility:** Only players with price ≤ 2.0M, cannot be captain
- **Captain Bonus Validation:** Kapitanów 2 requires 2 different starting players

#### Transfer Tips Logic
- **Form Analysis:** Identify players with declining form (last 2 gameweeks)
- **Fixture Difficulty:** Consider upcoming match difficulty
- **Price Similarity:** Suggest players within ±0.5M price range
- **Position Matching:** Only suggest same-position replacements
- **Team Balance:** Maintain team distribution limits

### 4.3 Error Handling
- **Validation Errors (400):** Detailed field-level error messages
- **Authentication Errors (401):** Clear authentication required messages
- **Authorization Errors (403):** Resource access denied messages
- **Not Found Errors (404):** Specific resource not found messages
- **Conflict Errors (409):** Constraint violation explanations
- **Rate Limit Errors (429):** Time until next allowed request
- **Server Errors (500):** Generic error with tracking ID for debugging

### 4.4 Data Consistency
- **Transactions:** Use database transactions for multi-table operations (lineup creation with players)
- **Referential Integrity:** Enforce foreign key constraints
- **Audit Logging:** Track all user actions for analytics and debugging
- **Backup Strategy:** Regular automated backups with point-in-time recovery

### 4.5 Performance Optimizations
- **Pagination:** Default page size 50, maximum 100 for list endpoints
- **Indexing:** Leverage database indexes defined in schema
- **Caching:** Cache frequently accessed data (teams, bonuses, current gameweek)
- **Query Optimization:** Use efficient queries with proper joins and filters
- **Rate Limiting:** Prevent abuse while allowing normal usage patterns

## 5. Implementation Status (Current)

### Implemented Endpoints

#### Teams Management (2.3)
- **GET /api/teams**: List with filtering (sort, order, is_active) - Implemented via TeamsController
- **GET /api/teams/{id}**: Get specific team - Implemented
- **POST /api/teams**: Create new team - Implemented (with uniqueness validation)
- **PUT /api/teams/{id}**: Update team - Implemented (with conflict checks)

#### Gameweeks Management (2.6)
- **GET /api/gameweeks**: List with filtering (status, sort, order) - Implemented via GameweeksController
- **GET /api/gameweeks/current**: Get current gameweek - Implemented
- **GET /api/gameweeks/{id}**: Get with matches - Implemented (via GetByIdWithMatchesAsync)
- **POST /api/gameweeks**: Create - Implemented
- **PUT /api/gameweeks/{id}**: Update - Implemented

#### Matches Management (2.7)
- **GET /api/matches**: List with full filtering/pagination - Implemented via MatchesController
- **GET /api/matches/{id}**: Get specific - Implemented
- **POST /api/matches**: Create - Implemented (with validation)
- **PUT /api/matches/{id}**: Update - Implemented

#### Player Statistics (2.5)
- **GET /api/player-stats**: List with filtering (gameweek_id, player_id, team_id, position, sort, order, limit, page) - **TODO**: Implement via PlayerStatsController
- **POST /api/player-stats/import**: Import from CSV - **TODO**: Implement with CsvHelper

#### Admin & Monitoring (2.14)
- **GET /api/admin/dashboard**: System health (placeholder data) - Implemented via AdminController (requires auth)

### Notes
- **Authentication**: Not yet implemented (endpoints public for now; add [Authorize] later).
- **Error Handling**: Global ApiExceptionMiddleware added for consistent 400/401/404/409/500 responses.
- **Documentation**: Swagger UI available at /swagger in development.
- **Services**: Updated TeamService, GameweekService, MatchService to return DTOs and add missing methods (e.g., GetByIdWithMatchesAsync).
- **Next**: Players, Lineups/AI, Bonuses, etc. (medium priority).

### Testing
- Run `dotnet run` in Web project.
- Access Swagger: http://localhost:5xxx/swagger
- Test endpoints with Postman or curl (e.g., GET /api/teams).