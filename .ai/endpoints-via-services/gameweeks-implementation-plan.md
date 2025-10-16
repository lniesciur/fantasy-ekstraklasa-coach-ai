# API Endpoint Implementation Plan: Gameweeks

## 1. Przegląd punktu końcowego
Kolekcja punktów końcowych do zarządzania przebiegami kolejek (gameweeks) w systemie Fantasy Ekstraklasa.

### Cel i funkcjonalność
- Umożliwienie klientom aplikacji:
  - Listowania dostępnych gameweeków z opcją filtrowania i sortowania
  - Pobrania bieżącego aktywnego gameweeka
  - Pobrania szczegółów konkretnego gameweeka wraz z meczami
  - Tworzenia nowych gameweeków
  - Aktualizacji istniejących gameweeków

## 2. Szczegóły żądania

### GET /api/gameweeks
- Metoda HTTP: GET
- Struktura URL: `/api/gameweeks`
- Parametry zapytania:
  - Wymagane: brak
  - Opcjonalne:
    - `status` (string): upcoming, current, completed
    - `sort` (string): number, start_date (domyślnie number)
    - `order` (string): asc, desc (domyślnie asc)

### GET /api/gameweeks/current
- Metoda HTTP: GET
- Struktura URL: `/api/gameweeks/current`
- Parametry: brak

### GET /api/gameweeks/{id}
- Metoda HTTP: GET
- Struktura URL: `/api/gameweeks/{id}`
- Parametry ścieżki:
  - `id` (int) – identyfikator gameweeka

### POST /api/gameweeks
- Metoda HTTP: POST
- Struktura URL: `/api/gameweeks`
- Request Body (JSON):
  ```json
  {
    "number": 16,
    "start_date": "2025-10-27",
    "end_date": "2025-10-29"
  }
  ```

### PUT /api/gameweeks/{id}
- Metoda HTTP: PUT
- Struktura URL: `/api/gameweeks/{id}`
- Parametry ścieżki:
  - `id` (int)
- Request Body (JSON):
  ```json
  {
    "number": 16,
    "start_date": "2025-10-28",
    "end_date": "2025-10-30"
  }
  ```

## 3. Wykorzystywane typy
- DTOs:
  - `GameweekDto`:
    - `id`, `number`, `start_date`, `end_date`, `status` (enum), `matches_count` lub lista `MatchDto`
  - `MatchDto` (używana w GET by id): `id`, `home_team: TeamDto`, `away_team: TeamDto`, `match_date`, `status`
  - `TeamDto`: `id`, `name`, `short_code`
  - `GameweekListFilterDto`: parametry: `status`, `sort`, `order`
- Command Models / Requests:
  - `CreateGameweekCommand`: `number`, `start_date`, `end_date`
  - `UpdateGameweekCommand`: `id`, `number`, `start_date`, `end_date`

## 4. Przepływ danych
1. HTTP Request → Middleware uwierzytelniające (`[Authorize]`)
2. Mapowanie parametrów i/body do DTO/Command
3. Walidacja danych (FluentValidation lub DataAnnotations)
4. Wywołanie serwisu `IGameweekService`:
   - `ListAsync(GameweekListFilterDto)`
   - `GetCurrentAsync()`
   - `GetByIdAsync(id)`
   - `CreateAsync(CreateGameweekCommand)`
   - `UpdateAsync(UpdateGameweekCommand)`
5. Serwis używa repozytorium `IGameweekRepository` do operacji CRUD na bazie danych
6. Mapowanie modeli bazy (`GameweekDbModel`) na `GameweekDto`
7. Zwrócenie odpowiedzi HTTP z odpowiednim kodem i ciałem

## 5. Względy bezpieczeństwa
- Autoryzacja wszystkich punktów końcowych (`[Authorize]`)
- RLS (Row-Level Security) w Supabase gwarantuje tylko uprawnionym użytkownikom dostęp
- Walidacja wejścia zapobiegająca atakom typu SQL Injection
- Sanitizacja danych dat

## 6. Obsługa błędów
- 400 Bad Request: nieprawidłowe dane wejściowe (walidacja pól, data zła kolejność)
- 401 Unauthorized: brak lub nieprawidłowy token JWT
- 404 Not Found:
  - brak aktywnego gameweeka (GET /current)
  - niewystępujący `id` (GET/PUT)
- 409 Conflict: numer gameweeka już istnieje (POST/PUT)
- 500 Internal Server Error: nieprzewidziane wyjątki, logowanie do centralnego loggera

## 7. Wydajność
- Stronicowanie (w razie dużej liczby gameweeków) – przyszłe rozszerzenie
- Indeksy na kolumnach `number`, `start_date`, `status`
- Caching odpowiedzi GET /current przez krótki czas (np. 30s)

## 8. Kroki wdrożenia
1. **Domain**
   - Dodaj `Gameweek` w `FantasyCoachAI.Domain.Entities`
   - Zaktualizuj `IGameweekRepository` interfejs w `Domain.Interfaces`
2. **Infrastructure**
   - Utwórz `GameweekRepository` implementujący `IGameweekRepository`
   - Dodaj `GameweekDbModel` i mapowania w `Mappers`
3. **Application**
   - Stwórz DTO/Command/Filter w `FantasyCoachAI.Application.DTOs`
   - Dodaj walidatory (FluentValidation)
   - Zaimplementuj `GameweekService` i zarejestruj w `DependencyInjection` (katalog Services)
5. **Testy**
   - Napisz testy jednostkowe dla warstw w których wykonywałeś zmiany