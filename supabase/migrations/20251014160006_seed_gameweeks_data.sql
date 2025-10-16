-- SQL INSERT do tabeli gameweeks
-- PKO BP Ekstraklasa 2025/2026
-- Wszystkie 34 kolejki sezonu

-- Struktura tabeli (dla referencji):
-- CREATE TABLE gameweeks (
--     id serial primary key,
--     number integer not null unique,
--     start_date date not null,
--     end_date date not null
-- );

-- INSERT wszystkich kolejek jednym zapytaniem
INSERT INTO gameweeks (number, start_date, end_date) VALUES
  (1, '2025-07-18', '2025-07-20'),   -- Runda jesienna
  (2, '2025-07-26', '2025-07-27'),
  (3, '2025-08-02', '2025-08-03'),
  (4, '2025-08-09', '2025-08-10'),
  (5, '2025-08-16', '2025-08-17'),
  (6, '2025-08-23', '2025-08-24'),
  (7, '2025-08-30', '2025-08-31'),
  (8, '2025-09-13', '2025-09-14'),
  (9, '2025-09-20', '2025-09-21'),
  (10, '2025-09-27', '2025-09-28'),
  (11, '2025-10-04', '2025-10-05'),
  (12, '2025-10-18', '2025-10-19'),
  (13, '2025-10-25', '2025-10-26'),
  (14, '2025-11-01', '2025-11-02'),
  (15, '2025-11-08', '2025-11-09'),
  (16, '2025-11-22', '2025-11-23'),
  (17, '2025-11-29', '2025-11-30'),
  (18, '2025-12-06', '2025-12-07'),  -- Koniec rundy jesiennej
  (19, '2026-01-31', '2026-02-01'),  -- Runda wiosenna
  (20, '2026-02-07', '2026-02-08'),
  (21, '2026-02-14', '2026-02-15'),
  (22, '2026-02-21', '2026-02-22'),
  (23, '2026-02-28', '2026-03-01'),
  (24, '2026-03-07', '2026-03-08'),
  (25, '2026-03-14', '2026-03-15'),
  (26, '2026-03-21', '2026-03-22'),
  (27, '2026-04-04', '2026-04-05'),
  (28, '2026-04-11', '2026-04-12'),
  (29, '2026-04-18', '2026-04-19'),
  (30, '2026-04-25', '2026-04-26'),
  (31, '2026-05-02', '2026-05-03'),
  (32, '2026-05-09', '2026-05-10'),
  (33, '2026-05-16', '2026-05-17'),
  (34, '2026-05-23', '2026-05-24');  -- Zakończenie sezonu

-- Weryfikacja danych
SELECT 
    number,
    start_date,
    end_date,
    end_date - start_date as duration_days
FROM gameweeks
ORDER BY number;

-- Statystyki
SELECT 
    COUNT(*) as total_gameweeks,
    MIN(start_date) as season_start,
    MAX(end_date) as season_end
FROM gameweeks;