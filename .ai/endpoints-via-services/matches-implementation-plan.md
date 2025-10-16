# API Endpoint Implementation Plan: Matches

## 1. Przegląd punktu końcowego

Punkty końcowe Matches umożliwiają zarządzanie meczami w systemie Fantasy Ekstraklasa Coach AI. Obejmują one listowanie meczów z filtrowaniem i paginacją, pobieranie szczegółów pojedynczego meczu, tworzenie nowego meczu oraz aktualizację istniejącego meczu. Wszystkie operacje odczytu są dostępne publicznie (zgodne z politykami RLS w Supabase), natomiast operacje zapisu wymagają uwierzytelnienia użytkownika. Implementacja opiera się na warstwie Application bez kontrolerów, korzystając z Minimal API w .NET 8 i Supabase jako backendu bazy danych.

## 2. Szczegóły żądania

### GET /api/matches
- Metoda HTTP: GET
- Struktura URL: /api/matches
- Parametry:
  - Wymagane: Brak
  - Opcjonalne: 
    - `gameweek_id`: integer (filtrowanie po ID kolejki)
    - `team_id`: integer (mecze z udziałem konkretnej drużyny)
    - `status`: enum (scheduled, postponed, cancelled, played)
    - `date_from`: date (YYYY-MM-DD, dolna granica daty meczu)
    - `date_to`: date (YYYY-MM-DD, górna granica daty meczu)
    - `sort`: enum (match_date, gameweek_number; default: match_date)
    - `order`: enum (asc, desc; default: asc)
    - `page`: integer (default: 1)
    - `limit`: integer (default: 50, max: 100)
- Request Body: Brak

### GET /api/matches/{id}
- Metoda HTTP: GET
- Struktura URL: /api/matches/{id}
- Parametry:
  - Wymagane: `id`: integer (ID meczu w ścieżce URL)
  - Opcjonalne: Brak
- Request Body: Brak

### POST /api/matches
- Metoda HTTP: POST
- Struktura URL: /api/matches
- Parametry:
  - Wymagane: Brak
  - Opcjonalne: Brak
- Request Body: 
  ```json
  {
    "gameweek_id": integer (wymagany, ID kolejki),
    "home_team_id": integer (wymagany, ID drużyny gospodarzy),
    "away_team_id": integer (wymagany, ID drużyny gości),
    "match_date": string (wymagany, data i czas meczu w formacie ISO 8601),
    "status": enum (opcjonalny, scheduled/postponed/cancelled/played; default: scheduled)
  }
  ```

### PUT /api/matches/{id}
- Metoda HTTP: PUT
- Struktura URL: /api/matches/{id}
- Parametry:
  - Wymagane: `id`: integer (ID meczu w ścieżce URL)
  - Opcjonalne: Brak
- Request Body: 
  ```json
  {
    "gameweek_id": integer (opcjonalny, zmiana kolejki),
    "home_team_id": integer (opcjonalny, zmiana drużyny gospodarzy),
    "away_team_id": integer (opcjonalny, zmiana drużyny gości),
    "match_date": string (opcjonalny, nowa data i czas),
    "status": enum (opcjonalny, scheduled/postponed/cancelled/played),
    "home_score": integer (opcjonalny, wynik gospodarzy; null jeśli nie rozegrany),
    "away_score": integer (opcjonalny, wynik gości; null jeśli nie rozegrany),
    "reschedule_reason": string (opcjonalny, powód przełożenia)
  }
  ```

## 3. Wykorzystywane typy

- **DTOs:**
  - `MatchDto`: Zawiera ID, gameweek (z id i number), home_team/away_team (z id, name, short_code), match_date, status, home_score, away_score, reschedule_reason, created_at, updated_at (dla szczegółów; uproszczona wersja dla listy bez created/updated).
  - `MatchListDto`: Zawiera listę `MatchDto` oraz `PaginationDto` (page, limit, total, pages).
  - `TeamDto`: Istniejący (id, name, short_code).
  - `GameweekDto`: Istniejący (id, number; dla szczegółów: + start_date, end_date).

- **Command Modele:**
  - `CreateMatchCommand`: gameweek_id (int), home_team_id (int), away_team_id (int), match_date (DateTime), status (MatchStatus? default scheduled).
  - `UpdateMatchCommand`: gameweek_id (int?), home_team_id (int?), away_team_id (int?), match_date (DateTime?), status (MatchStatus?), home_score (int?), away_score (int?), reschedule_reason (string?).
  - `MatchListFilterDto`: gameweek_id (int?), team_id (int?), status (MatchStatus?), date_from (DateTime?), date_to (DateTime?), sort (string), order (string), page (int), limit (int).

- Walidatory: Użyć FluentValidation dla Command/DTO (np. wymagane pola, zakresy, enumy). Dla filtrów: walidacja paginacji (page >=1, limit 1-100).

## 4. Szczegóły odpowiedzi

- **GET /api/matches (200):**
  ```json
  {
    "data": [MatchDto (uproszczony)],
    "pagination": {
      "page": integer,
      "limit": integer,
      "total": integer,
      "pages": integer
    }
  }
  ```

- **GET /api/matches/{id} (200):**
  ```json
  {
    "id": integer,
    "gameweek": { "id": integer, "number": integer, "start_date": string, "end_date": string },
    "home_team": TeamDto,
    "away_team": TeamDto,
    "match_date": string (ISO 8601),
    "status": string (enum),
    "home_score": integer|null,
    "away_score": integer|null,
    "reschedule_reason": string|null,
    "created_at": string (ISO 8601),
    "updated_at": string (ISO 8601)
  }
  ```

- **POST /api/matches (201):**
  Pełny `MatchDto` (z nowo utworzonym ID).

- **PUT /api/matches/{id} (200):**
  Pełny zaktualizowany `MatchDto`.

Błędy:
- 400: Nieprawidłowe parametry (walidacja nieudana).
- 401: Nieautoryzowany dostęp (dla POST/PUT).
- 404: Nie znaleziono (mecz, kolejka lub drużyna).
- 500: Błąd serwera (np. problem z bazą).

## 5. Przepływ danych

1. **Walidacja wejścia:** W warstwie Application (FluentValidation) sprawdzić parametry/Command.
2. **Wywołanie serwisu:** MatchService (nowy) obsłuży logikę:
   - Dla GET list: IMatchRepository.GetMatchesAsync(filter) – zapytanie do Supabase z filtrowaniem (użyć PostgREST z parametrami where/order/limit/offset), mapowanie do DTO via MatchMapper (istniejący lub nowy).
   - Dla GET/{id}: IMatchRepository.GetByIdAsync(id), join z gameweeks i teams, mapowanie.
   - Dla POST: Sprawdź istnienie gameweek i teams (via IGameweekRepository/ITeamRepository), waliduj brak duplikatu (home != away, brak meczu dla tych drużyn w kolejce), IMatchRepository.CreateAsync(command), zwróć utworzony.
   - Dla PUT: Pobierz istniejący, sprawdź istnienie gameweek/teams jeśli zmieniane, zaktualizuj via IMatchRepository.UpdateAsync, obsłuż partial update (tylko podane pola).
3. **Interakcja z bazą:** Supabase Client (Scoped) do CRUD via PostgREST. Użyć transakcji dla create/update jeśli potrzeba (np. walidacja spójności).
4. **Mapowanie:** Użyć AutoMapper lub ręcznych mapperów (GameweekMapper, TeamMapper istnieją; dodać MatchMapper).
5. **Paginacja:** W serwisie obliczyć total count osobno, użyć offset = (page-1)*limit.

## 6. Względy bezpieczeństwa

- **Uwierzytelnienie:** Dla POST/PUT wymagać JWT z Supabase Auth (użyć auth.uid() w RLS). Dla GET – publiczne (RLS pozwala read na matches).
- **Autoryzacja:** RLS na tabeli matches: public read, insert/update/delete tylko dla autoryzowanych (np. admin role lub authenticated). Walidować w serwisie, czy user ma prawa (np. tylko admin może tworzyć/aktualizować).
- **Walidacja danych:** Sanitizacja input (DateTime parsing, enum binding), zapobieganie SQL injection (PostgREST parametrów używa). Ograniczyć limit do 100, aby uniknąć DoS.
- **Zagrożenia:** Brak – Supabase obsługuje RLS i walidację schematu. Monitorować nadużycia via logs.

## 7. Obsługa błędów

- **400 Bad Request:** Walidacja nieudana (np. niepoprawny status, home_team_id == away_team_id, date poza zakresem gameweek). Zwrócić ProblemDetails z błędami walidacji.
- **401 Unauthorized:** Brak tokenu lub nieważny dla POST/PUT.
- **404 Not Found:** Mecz nie istnieje (GET/PUT), gameweek/team nie istnieje (POST/PUT).
- **500 Internal Server Error:** Wyjątki bazy (np. constraint violation), logować z ILogger (strukturalnie: endpoint, user_id, error).
- Inne scenariusze: Duplikat meczu (409 Conflict?), przepełnienie paginacji (400). Użyć globalnego exception handlera w Web layer. Logować do Supabase jeśli tabela błędów istnieje, inaczej do console/file.

## 8. Rozważania dotyczące wydajności

- **Zapytania:** Indeksy na matches: gameweek_id, status, match_date, home_team_id, away_team_id (z db-plan). Dla list: unikać N+1 (użyć joins w PostgREST).
- **Paginacja:** Offset-based, ale dla dużych zbiorów rozważyć cursor-based w przyszłości. Limit max 100.
- **Cache:** Rozważyć Redis dla często odczytywanych list (np. aktualna gameweek), ale nie dla dynamicznych filtrów.
- **Optymalizacja:** Lazy loading w DTO (tylko potrzebne pola). Monitorować via health checks (Supabase connectivity).
- Potencjalne wąskie gardła: Duże filtry po dacie – indeksy daty; joiny z teams/gameweeks – indeksy FK.

## 9. Etapy wdrożenia

1. **Przygotowanie typów:** Dodać/rozszerzyć DTOs (MatchDto, MatchListFilterDto, PaginationDto) i Commands (CreateMatchCommand, UpdateMatchCommand) w FantasyCoachAI.Application/DTOs. Dodać walidatory z FluentValidation.
2. **Repository:** W Domain dodać IMatchRepository z metodami: GetMatchesAsync(filter), GetByIdAsync(id), CreateAsync(command), UpdateAsync(id, command). W Infrastructure dodać MatchRepository implementację z Supabase (użyć _supabase.From<MatchDbModel>.Where...).
3. **Mapper:** Dodać MatchMapper w Infrastructure/Mappers do konwersji MatchDbModel &lt;-&gt; MatchDto (użyć istniejących dla Team/Gameweek).
4. **Service:** W Application dodać IMatchService i MatchService: zaimplementować metody dla endpointów, wstrzyknąć IGameweekRepository/ITeamRepository do walidacji, użyć mappera i repository.
5. **DI Registration:** W Infrastructure/DependencyInjection.cs zarejestrować IMatchRepository, IMatchService (Scoped).
6. **Minimal API:** W Web/Program.cs dodać mapowania:
   - app.MapGet(&quot;/api/matches&quot;, async (IMatchService service, [AsParameters] MatchListFilterDto filter) =&gt; await service.GetMatchesAsync(filter));
   - app.MapGet(&quot;/api/matches/{id}&quot;, async (int id, IMatchService service) =&gt; await service.GetMatchAsync(id));
   - app.MapPost(&quot;/api/matches&quot;, async (CreateMatchCommand cmd, IMatchService service) =&gt; await service.CreateMatchAsync(cmd)).RequireAuthorization();
   - app.MapPut(&quot;/api/matches/{id}&quot;, async (int id, UpdateMatchCommand cmd, IMatchService service) =&gt; await service.UpdateMatchAsync(id, cmd)).RequireAuthorization();
   Dodać walidację i error handling middleware.
7. **Testy:** Dodać unit testy dla Service (Moq dla repo), integration testy dla Supabase (Testcontainers lub mock client).
8. **Walidacja biznesowa:** W Service: dla create/update sprawdzić aktywność teams (is_active=true), unikalność pary teams w gameweek, match_date w zakresie start/end gameweek.
9. **Dokumentacja:** Zaktualizować OpenAPI/Swagger dla nowych endpointów.
10. **Deployment:** Uruchomić migracje Supabase jeśli potrzeba (indeksy istnieją), przetestować RLS.
