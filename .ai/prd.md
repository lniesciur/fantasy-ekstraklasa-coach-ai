# Dokument wymagań produktu (PRD) - Fantasy Ekstraklasa Coach AI

## 1. Przegląd produktu

Fantasy Ekstraklasa Coach AI to webowa aplikacja umożliwiająca szybkie i oparte na danych generowanie składów w grze Fantasy Ekstraklasy. Wykorzystuje gotowy model LLM (np. GPT-4) do rekomendowania optymalnego zestawienia zawodników zgodnie z zasadami gry. Aplikacja używa ASP.NET Core (.NET 8) na backendzie, Supabase do uwierzytelniania i przechowywania danych oraz Blazor na frontendzie. Dane statystyczne są importowane ręcznie z plików CSV.

## 2. Problem użytkownika

Gracze Fantasy Ekstraklasy muszą ręcznie tworzyć składy, uwzględniając wiele kryteriów (forma zawodników, cena, terminarz, limity klubowe). Proces jest czasochłonny i podatny na błędy, co zmniejsza satysfakcję i skuteczność przy tworzeniu konkurencyjnych drużyn.

## 3. Wymagania funkcjonalne

- F-001 Przegląd listy zespołów
- F-002 Przegląd listy piłkarzy
- F-003 Przegląd terminarza gier
- F-004 Import statystyk z pliku CSV (minuty, bramki, asysty, punkty, cena)
- F-005 Edycja i aktualizacja pojedynczych meczów przez formularz
- F-006 System kont użytkowników z Supabase (MVP dostęp tylko dla developera)
- F-007 Integracja z modelem LLM generującym rekomendowany skład zgodny z regułami
- F-008 Automatyczny walidator zgodności składów z zasadami Fantasy Ekstraklasy
- F-009 Przechowywanie historii generowanych składów dla analizy i iteracji

## 4. Granice produktu

W ramach MVP nie realizujemy:

- Własnego, zaawansowanego algorytmu rekomendującego (używamy gotowego LLM)
- Importu z wielu formatów (tylko CSV)
- Współdzielenia zestawów między użytkownikami
- Aplikacji mobilnych (tylko web)
- Rozbudowanego systemu ról i uprawnień (MVP: tylko developer)
- Automatycznych aktualizacji danych z zewnętrznych źródeł

## 5. Historyjki użytkowników

- US-001: Logowanie i uwierzytelnianie  
  Opis: Jako developer chcę się zalogować do aplikacji za pomocą Supabase, by uzyskać dostęp do funkcji importu i generowania składów.  
  Kryteria akceptacji:  
    1. Given: strona logowania, When: developer wprowadza prawidłowe dane, Then: uzyskuje token JWT i dostęp do panelu.  
    2. Given: nieprawidłowe dane, When: developer próbuje się zalogować, Then: wyświetlany jest komunikat o błędzie i brak dostępu.

- US-002: Import statystyk z CSV  
  Opis: Jako developer chcę zaimportować plik CSV ze statystykami, by mieć aktualne dane do generowania składu.  
  Kryteria akceptacji:  
    1. Given: formularz importu, When: developer przesyła poprawny plik CSV, Then: dane zostają zapisane i wyświetlony podgląd.  
    2. Given: błędny format CSV, When: plik zawiera brakujące kolumny, Then: wyświetlony komunikat o błędzie i żaden rekord nie jest importowany.

- US-003: Przegląd listy zespołów  
  Opis: Jako developer chcę zobaczyć listę wszystkich drużyn, by zweryfikować dane przed generowaniem składu.  
  Kryteria akceptacji:  
    1. Lista wszystkich drużyn jest widoczna z nazwą i kluczem identyfikacyjnym.  
    2. Możliwość odświeżenia listy po imporcie CSV.

- US-004: Przegląd listy piłkarzy  
  Opis: Jako developer chcę zobaczyć listę piłkarzy z ich statystykami, by ocenić dostępne opcje.  
  Kryteria akceptacji:  
    1. Wyświetlane: imię, nazwisko, drużyna, cena, punkty, forma.  
    2. Możliwość filtrowania po drużynie i sortowania według punktów.

- US-005: Przegląd terminarza gier  
  Opis: Jako developer chcę zobaczyć terminarz nadchodzących kolejek, by określić priorytety w trakcie generowania składu.  
  Kryteria akceptacji:  
    1. Wyświetlane daty i pary zespołów dla kolejki.  
    2. Możliwość wyboru kolejki do generowania składu.

- US-006: Edycja pojedynczego meczu  
  Opis: Jako developer chcę edytować szczegóły meczu (data, godzina, typ), by poprawić dane przed generowaniem.  
  Kryteria akceptacji:  
    1. Formularz edycji poprawnie zapisuje zmiany do bazy.  
    2. Nieprawidłowe dane wyświetlają walidację i nie zapisują.

- US-007: Generowanie składu via LLM  
  Opis: Jako developer chcę wygenerować rekomendację składu dla wybranej kolejki, by szybko uzyskać optymalny zestaw.  
  Kryteria akceptacji:  
    1. Given: wybór kolejki, When: kliknięcie Generuj, Then: wysyłany jest prompt do API LLM z regułami i danymi.  
    2. Otrzymana lista zawiera 2 bramkarzy, 5 obrońców, 5 pomocników, 3 napastników, max 3 z jednego klubu, budżet ≤ 30 mln.

- US-008: Przegląd i edycja wygenerowanego składu  
  Opis: Jako developer chcę zobaczyć wygenerowany skład i wprowadzić ręczne poprawki, by dopasować ostateczne ustawienie.  
  Kryteria akceptacji:  
    1. Ekran przedstawia rekomendowany skład.  
    2. Możliwość zmiany zawodnika i natychmiastowa walidacja reguł.

- US-009: Zapis historii generowanych składów  
  Opis: Jako developer chcę zapisać historię wygenerowanych składów, by analizować efekty wcześniejszych ustawień.  
  Kryteria akceptacji:  
    1. Każde wygenerowanie zapisuje rekord z danymi wejściowymi i wynikami.  
    2. Możliwość przeglądania listy poprzednich generacji.

- US-010: Automatyczna walidacja zgodności składów  
  Opis: Jako developer chcę, aby system automatycznie sprawdzał, czy skład spełnia reguły Fantasy Ekstraklasy, by uniknąć błędów.  
  Kryteria akceptacji:  
    1. System zwraca błędy, jeśli przekroczono budżet lub limity klubowe.  
    2. Przed zapisem historii składów walidacja musi być zakończona i zielony status.

## 6. Metryki sukcesu

- Procent generowanych składów zgodnych z regułami w środowisku testowym: 100%.  
- Średni czas generowania składu < 5 sekund.  
- Dokładność importu CSV: ≥ 99% testowanych plików.  
- Dostępność systemu: ≥ 99,9% uptime.  
- Liczba udanych walidacji składów po generowaniu: 100%.
