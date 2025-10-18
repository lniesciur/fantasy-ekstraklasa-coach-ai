# Architektura UI dla Fantasy Ekstraklasa Coach AI

## 1. Przegląd struktury UI

Aplikacja webowa Fantasy Ekstraklasa Coach AI to intuicyjny interfejs oparty na Blazor Server z MudBlazor, umożliwiający szybkie generowanie składów w grze Fantasy Ekstraklasy przy pomocy AI. Architektura UI jest podzielona na logiczne sekcje nawigacyjne:

- **Dashboard** - główny ekran z przeglądem i szybkim dostępem
- **Zespoły (Teams)** - zarządzanie i przeglądanie drużyn piłkarskich
- **Zawodnicy (Players)** - zaawansowana lista z filtrowaniem i wyszukiwaniem
- **Kolejki (Gameweeks)** - przegląd rund Fantasy Ekstraklasy
- **Mecze (Matches)** - terminarz i zarządzanie meczami
- **Składy (Lineups)** - tworzenie, edycja i historia składów
- **Historia (History)** - analiza poprzednich generacji

Struktura stosuje hierarchię: Layout główny → Strony (Pages) → Komponenty specjalistyczne (Components) → Komponenty wspólne (Shared). Aplikacja implementuje Blazor Server SignalR dla real-time updates, caching na poziomie komponentów z @key directives, oraz optimistic updates dla lepszego doświadczenia użytkownika.

---

## 2. Lista widoków

### 2.1 Dashboard / Strona główna

**Ścieżka:** `/dashboard`  
**Główny cel:** Zaprezentowanie przeglądu systemu, szybkiego dostępu do kluczowych funkcji oraz aktualnych statystyk użytkownika.

**Kluczowe informacje do wyświetlenia:**
- Karta "Mój aktualny skład" - wyświetlanie aktywnego składu z krótkim podglądem (prosta lista z formacjami)
- Karta "Historia składów" - ostatnie 3 wygenerowane składy z datą i wynikami
- Przycisk akcji "Generuj nowy skład"

**Kluczowe komponenty widoku:**
- `DashboardCard` - reusable component do wyświetlania karti z ikonami i informacjami
- `QuickActionButton` - przyciski szybkich akcji
- `RecentLineupPreview` - miniaturowy widok ostatnich składów


**UX, dostępność i względy bezpieczeństwa:**
- Responsywny grid: 2 kolumny na desktop, 1 kolumna na mobile
- Karty zorganizowane w logiczną kolejność (od najważniejszych)
- Lazy loading dla danych historii i statystyk
- Prefetching danych dla często przeglądanych sekcji
- Szybki dostęp do głównych funkcji bez głębokich kliknięć
- Brak wymagań dostępności w MVP
- Przygotowanie struktury do przyszłych ról bez implementacji

---

### 2.2 Lista Zespołów

**Ścieżka:** `/teams`  
**Główny cel:** Wyświetlenie pełnej listy drużyn piłkarskich z możliwością filtrowania, sortowania i przeglądania szczegółów.

**Kluczowe informacje do wyświetlenia:**
- Nazwa drużyny z logotypem (krest)
- Kod skrótowy (3-4 znaki)
- Status aktywności (aktywna/nieaktywna)
- Pozycja w lidze
- Forma drużyny (ostatnie wyniki: W/D/L)
- Liczba zawodników
- Średnia ilość punktów Fantasy
- Przyciski akcji: Szczegóły, Edytuj (admin), Usuń (admin)

**Kluczowe komponenty widoku:**
- `TeamsTable` - główna tabela z danymi zespołów
- `TeamFilterPanel` - panel filtrowania (status, forma, sortowanie)
- `SearchBar` - wyszukiwanie po nazwie/kodzie
- `PaginationControls` - kontrola stronicowania
- `TeamDetailModal` - modal z szczegółami drużyny

**UX, dostępność i względy bezpieczeństwa:**
- Sortowanie po: nazwa, pozycja w lidze, liczba zawodników
- Filtrowanie: aktywne/wszystkie, forma
- Responsive tabela → karta na mobile
- Ikonki dla szybkiej identyfikacji statusu
- Caching listy zespołów (zmienia się rzadko)
- Prefetching szczegółów drużyn przy hovering (desktop)
- Podgląd logotypów drużyn dla lepszego UX

---

### 2.3 Szczegóły Zespołu

**Ścieżka:** `/teams/{id}`  
**Główny cel:** Wyświetlenie pełnych informacji o drużynie, zawodnikach oraz terminarzu.

**Kluczowe informacje do wyświetlenia:**
- Nagłówek z logotypem, nazwą, kodem i formą
- Pozycja w lidze i statystyki zespołu
- Tabela zawodników: imię, pozycja, cena, forma, punkty
- Nadchodzące mecze drużyny
- Historia ostatnich meczów z wynikami
- Przycisk edycji (admin)

**Kluczowe komponenty widoku:**
- `TeamHeader` - nagłówek z informacjami zespołu
- `PlayersSubTable` - tabela zawodników drużyny z filtrami
- `UpcomingFixtures` - lista nadchodzących meczów
- `MatchHistory` - historia meczów
- `TeamStatsCard` - card ze statystykami zespołu

**UX, dostępność i względy bezpieczeństwa:**
- Breadcrumb nawigacja: Dashboard > Zespoły > [Nazwa zespołu]
- Szybki powrót do listy Teams
- Lazy loading zawodników i historii meczów
- Klikalne linki do szczegółów zawodników
- Tab-based layout: Zawodnicy, Mecze, Statystyki

---

### 2.4 Lista Zawodników

**Ścieżka:** `/players`  
**Główny cel:** Zaawansowana lista zawodników z możliwością filtrowania, sortowania i wyszukiwania.

**Kluczowe informacje do wyświetlenia:**
- Imię i nazwisko zawodnika
- Drużyna (z logo)
- Pozycja (GK, DEF, MID, FWD)
- Cena
- Forma (numerycznie i wizualnie - bar)
- Punkty Fantasy
- Status zdrowia (Pewny/Wątpliwy/Nie zagra)
- Przewidywana gra (tak/nie)
- Liczbę gier i średnią punktów
- Przycisk szczegółów

**Kluczowe komponenty widoku:**
- `PlayersTable` - główna tabela z danymi zawodników
- `AdvancedFilterPanel` - zaawansowany panel filtrowania (pozycja, drużyna, cena, forma, status zdrowia)
- `SearchBar` - wyszukiwanie po imieniu/nazwisku
- `PaginationControls` - stronicowanie (domyślnie 50 na stronę)
- `PlayerFormBar` - wizualna reprezentacja formy
- `HealthStatusBadge` - badge statusu zdrowia

**UX, dostępność i względy bezpieczeństwa:**
- Zaawansowane filtry ze składnymi dropdown'ami
- Multi-select dla pozycji i drużyn
- Range sliders dla ceny i formy
- Sortowanie po: nazwa, cena, forma, punkty
- Debouncing wyszukiwania (300ms)
- Responsive tabela → expandable karty na mobile
- Sticky header przy scrollowaniu
- Infinite scroll lub pagination
- Lazy loading obrazów (logotypy)
- Prefetching danych zawodnika przy hovering

---

### 2.5 Szczegóły Zawodnika

**Ścieżka:** `/players/{id}`  
**Główny cel:** Wyświetlenie pełnego profilu zawodnika z historią statystyk i prognozami.

**Kluczowe informacje do wyświetlenia:**
- Nagłówek: zdjęcie, imię, nazwisko, drużyna, pozycja, cena
- Statystyki sezonu: mecze, bramki, asysty, żółte kartki, czerwone kartki
- Aktualna forma i punkty Fantasy
- Status zdrowia i przewidywana gra
- Tabela statystyk z ostatnich 5 gameweek'ów
- Nadchodzące mecze drużyny
- Historia meczów (ostatnie 10)
- Przycisk dodania do obserwowanych (fav)

**Kluczowe komponenty widoku:**
- `PlayerHeader` - nagłówek zawodnika z głównym info
- `PlayerStatsGrid` - grid statystyk sezonu
- `FormChart` - wykres formy zawodnika
- `RecentPerformancesTable` - tabela ostatnich performansów
- `UpcomingFixtures` - nadchodzące mecze
- `AddToWatchlist` - przycisk dodania do obserwowanych

**UX, dostępność i względy bezpieczeństwa:**
- Breadcrumb: Dashboard > Zawodnicy > [Imię]
- Szybki powrót do listy
- Responsive card layout na mobile
- Ikony dla wizualizacji statystyk
- Cytowany poprzedni skład (jeśli zawodnik był użyty)
- Porównanie z innymi zawodnikami z tej samej pozycji

---

### 2.6 Lista Kolejek (Gameweeks)

**Ścieżka:** `/gameweeks`  
**Główny cel:** Przegląd wszystkich rund Fantasy Ekstraklasy z możliwością wyboru do generowania składu.

**Kluczowe informacje do wyświetlenia:**
- Numer kolejki
- Zakres dat (start - end)
- Status (nadchodzące, aktualna, zakończona)
- Liczba meczów w kolejce
- Przycisk: Szczegóły, Generuj skład, Edytuj (admin)

**Kluczowe komponenty widoku:**
- `GameweeksTable` - tabela z danymi kolejek
- `GameweekCard` - card widok (alternatywa na mobile)
- `GameweekFilterPanel` - filtrowanie po statusie (nadchodzące/aktualna/zakończona)
- `StatusBadge` - badge statusu kolejki

**UX, dostępność i względy bezpieczeństwa:**
- Sortowanie: numer kolejki (desc - najnowsze na górze), daty
- Highlight aktualną kolejkę
- Responsive tabela → karty na mobile
- Szybki dostęp do generowania składu z tego widoku
- Caching listy (rzadko się zmienia)

---

### 2.7 Szczegóły Kolejki

**Ścieżka:** `/gameweeks/{id}`  
**Główny cel:** Wyświetlenie pełnych informacji o konkretnej kolejce, terminarzu meczów i opcji generowania składu.

**Kluczowe informacje do wyświetlenia:**
- Nagłówek: numer, daty, status
- Lista meczów: drużyna domowa - wynik - drużyna gośćująca, data/godzina
- Liczba meczów, średnia liczba zawodników grających
- Przycisk: Generuj skład, Edytuj (admin), Wróć do listy

**Kluczowe komponenty widoku:**
- `GameweekHeader` - nagłówek z informacjami
- `MatchesTable` - tabela meczów w kolejce
- `MatchCard` - card widok meczu (na mobile)
- `GenerateLineupButton` - przycisk generowania składu

**UX, dostępność i względy bezpieczeństwa:**
- Breadcrumb: Dashboard > Kolejki > Kolejka #
- Szybki przejścia do generowania składu
- Wizualna reprezentacja meczów (logo drużyn vs logo)
- Klikalny szczegół meczu
- Responsive karty

---

### 2.8 Lista Meczów

**Ścieżka:** `/matches`  
**Główny cel:** Przeglądanie wszystkich meczów z zaawansowanym filtrowaniem.

**Kluczowe informacje do wyświetlenia:**
- Drużyna domowa i gośćująca (z logami)
- Data i godzina meczu
- Kolejka
- Status (zaplanowany, rozegrany, odłożony, anulowany)
- Wynik (jeśli rozegrany)
- Przycisk szczegółów/edycji

**Kluczowe komponenty widoku:**
- `MatchesTable` - tabela meczów
- `MatchCard` - card widok meczu
- `FilterPanel` - filtrowanie (gameweek, drużyna, status, data range)
- `SearchBar` - wyszukiwanie
- `PaginationControls` - stronicowanie

**UX, dostępność i względy bezpieczeństwa:**
- Filtry: kolejka, drużyna, status, zakres dat
- Sortowanie: data (descending), gameweek, status
- Responsive tabela → karty na mobile
- Ikonki dla statusów
- Lazy loading dla dużych list
- Prefetching szczegółów meczów

---

### 2.9 Szczegóły Meczu

**Ścieżka:** `/matches/{id}`  
**Główny cel:** Wyświetlenie pełnych informacji o meczu.

**Kluczowe informacje do wyświetlenia:**
- Drużyna domowa vs gośćująca
- Data, godzina, status
- Wynik (jeśli rozegrany)
- Kolejka
- Przycisk edycji (admin)
- Powrót do listy

**Kluczowe komponenty widoku:**
- `MatchHeader` - nagłówek meczu
- `MatchDetails` - szczegóły i wynik
- `EditMatchForm` - formularz edycji (admin)

**UX, dostępność i względy bezpieczeństwa:**
- Breadcrumb: Dashboard > Mecze > [Mecz]
- Szybki powrót do listy

---

### 2.10 Generowanie Składu (AI)

**Ścieżka:** `/lineups/generate`  
**Główny cel:** Prosty interfejs do generowania rekomendowanego składu za pomocą AI.

**Kluczowe informacje do wyświetlenia:**
- Dropdown wyboru kolejki (gameweek)
- Wyświetlenie obecnego budżetu (30M)
- Przycisk "Generuj skład"
- Wskaźnik postępu/loading
- Komunikaty błędów

**Kluczowe komponenty widoku:**
- `GameweekSelector` - dropdown/select do wyboru kolejki
- `BudgetDisplay` - wyświetlanie dostępnego budżetu
- `GenerateButton` - przycisk z loading'iem
- `LoadingIndicator` - spinner/progress bar
- `ErrorNotification` - toast z błędami

**UX, dostępność i względy bezpieczeństwa:**
- Prosty, intuicyjny formularz
- Walidacja wyboru gameweek'u
- Wskaźnik postępu generowania
- Możliwość anulowania (jeśli długo trwa)
- Clear error messages
- Disabled button podczas generowania
- Optimistic update: szybki feedback

---

### 2.11 Wynik Generowania / Podgląd Składu

**Ścieżka:** `/lineups/generated` lub `/lineups/{id}`  
**Główny cel:** Wyświetlenie wygenerowanego składu z możliwością edycji, zatwierdzenia i zapisania.

**Kluczowe informacje do wyświetlenia:**
- Formacja (1-4-4-2)
- Budżet i pozostałe pieniądze
- Lista zawodników w składzie (podzielona na: bramkarze, obrońcy, pomocnicy, napastnicy, rezerwowi)
- Kapitan i wicekkapitan
- Walidacja zgodności z zasadami (budżet, limity klubowe, formacja)
- Status walidacji (zielony/czerwony)
- Przycisk edycji zawodnika (swap)
- Przycisk zapisania/zatwierdzenia
- Historia zmian

**Kluczowe komponenty widoku:**
- `LineupFormationGrid` - wizualna reprezentacja formacji z zawodnikami
- `PlayerList` - lista zawodników w składzie
- `BudgetBar` - pasek budżetu
- `ValidationStatus` - status walidacji ze szczegółami błędów
- `PlayerSwapModal` - modal do zmiany zawodnika
- `SaveLineupForm` - formularz do zapisu (nazwa, zapisz jako aktywny)
- `ChangeHistory` - historia zmian

**UX, dostępność i względy bezpieczeństwa:**
- Drag-and-drop dla zamiany zawodników (opcjonalnie)
- Klikalne zmiany zawodnika
- Real-time walidacja przy każdej zmianie
- Sugestie zastępstw (najlepsi dostępni zawodnicy)
- Undo/Redo zmian
- Przycisk "Zatwierdź i zapisz"
- Możliwość porzucenia zmian
- Clear error messages
- Responsive layout

---

### 2.12 Lista Składów (Lineups)

**Ścieżka:** `/lineups`  
**Główny cel:** Przeglądanie wszystkich zapisanych składów użytkownika z filtrowaniem.

**Kluczowe informacje do wyświetlenia:**
- Nazwa składu
- Kolejka (gameweek)
- Data utworzenia
- Formacja
- Całkowity koszt
- Status (aktywny/archiwum)
- Liczba zmian
- Przycisk szczegółów, edycji, duplikatu, usunięcia

**Kluczowe komponenty widoku:**
- `LineupsTable` - tabela składów
- `LineupCard` - card widok na mobile
- `FilterPanel` - filtrowanie (aktywne/archiwum, gameweek range, formacja)
- `SearchBar` - wyszukiwanie po nazwie
- `ActionButtons` - przyciski akcji (Szczegóły, Edytuj, Duplikuj, Usuń)

**UX, dostępność i względy bezpieczeństwa:**
- Sortowanie: data utworzenia (desc), nazwa, kolejka
- Highlight aktywny skład
- Responsive tabela → karty na mobile
- Lazy loading dla dużych list
- Potwierdzenie przed usunięciem

---

### 2.13 Szczegóły / Edycja Składu

**Ścieżka:** `/lineups/{id}`  
**Główny cel:** Pełny widok składu z możliwością edycji, zatwierdzenia i analizy.

**Kluczowe informacje do wyświetlenia:**
- (Jak w 2.11 - Wynik Generowania)
- Plus: Historia tego składu, jeśli był modyfikowany
- Przycisk: Edytuj, Duplikuj, Eksportuj, Usuń
- Analiza: jak poszedł ten skład (jeśli już się odbył gameweek)

**Kluczowe komponenty widoku:**
- (Jak w 2.11 - Wynik Generowania)
- `ExportButton` - eksport na tekst/PDF
- `DuplicateLineupModal` - modal do duplikacji na inny gameweek
- `LineupAnalysis` - analiza wyników (jeśli po rozegraniu)

**UX, dostępność i względy bezpieczeństwa:**
- Breadcrumb: Dashboard > Składy > [Nazwa]
- Szybki powrót do listy
- Edycja zawodnika na tym samym widoku
- Optimistic updates przy zapisze
- Toast notifikacje o sukcesie/błędzie

---

### 2.14 Historia Składów

**Ścieżka:** `/history`  
**Główny cel:** Analiza poprzednich generacji składów i wyników.

**Kluczowe informacje do wyświetlenia:**
- Tabela historii: kolejka, data, skład, liczba punktów, zmian, bonusu
- Filtry: zakres kolejek, źródło (AI/ręczny), czy zmodyfikowany
- Porównanie: AI punkty vs użytkownik punkty
- Średnia użytkownika vs średnia AI

**Kluczowe komponenty widoku:**
- `HistoryTable` - tabela z historią
- `FilterPanel` - filtrowanie (range kolejek, źródło, modyfikacje)
- `ComparisonStats` - statystyki porównawcze (user vs AI)
- `TrendChart` - wykres trendu punktów

**UX, dostępność i względy bezpieczeństwa:**
- Sortowanie: kolejka (desc), punkty, data
- Responsive tabela → karty na mobile
- Klikalny wiersz otwiera szczegóły
- Wizualizacja trendów
- Export do CSV (opcjonalnie)

---

### 2.15 Import CSV

**Ścieżka:** `/admin/import` (admin only)  
**Główny cel:** Drag-and-drop uploader dla importu statystyk zawodników z CSV.

**Kluczowe informacje do wyświetlenia:**
- Drag-and-drop obszar do przesyłania pliku
- Wybór gameweek'u
- Podgląd danych do zaimportowania
- Liczba rekordów do importu
- Przycisk potwierdzenia
- Raport po imporcie (powodzenie, błędy, ostrzeżenia)

**Kluczowe komponenty widoku:**
- `DragDropUploader` - drag-and-drop uploader
- `GameweekSelector` - wybór kolejki
- `PreviewTable` - podgląd danych (pierwsze 10 wierszy)
- `ImportReport` - raport po imporcie
- `ProgressBar` - pasek postępu

**UX, dostępność i względy bezpieczeństwa:**
- Dużo wizualne drag-and-drop obszar
- Podświetlenie pliku przy drag'u
- Podgląd przed importem
- Clear error messages
- Szczegółowy raport po imporcie
- Możliwość powtórzenia/anulowania
- Walidacja formatu pliku

---

### 2.16 Layout Główny

**Ścieżka:** Całej aplikacji  
**Główny cel:** Główny layout z nawigacją i wspólnymi elementami.

**Kluczowe informacje do wyświetlenia:**
- Logo aplikacji
- Menu nawigacji (Dashboard, Zespoły, Zawodnicy, Kolejki, Mecze, Składy, Historia, Import)
- Avatar/profil użytkownika (top-right)
- Powiadomienia
- Breadcrumb nawigacja
- Footer (opcjonalnie)

**Kluczowe komponenty widoku:**
- `MainLayout` - główny layout
- `NavigationMenu` - menu nawigacji
- `TopBar` - górny pasek z avatarem i powiadomieniami
- `Breadcrumb` - nawigacja
- `Footer` - footer

**UX, dostępność i względy bezpieczeństwa:**
- Responsive layout (mobile menu - hamburger)
- Sticky navigation
- Aktywny link w menu
- Ikony + tekst dla menu
- Ciemny/jasny motyw (opcjonalnie)
- Struktura CSS: MudBlazor Grid system

---

## 3. Mapa podróży użytkownika

### 3.1 Główny przypadek użycia: Generowanie składu

```
1. Użytkownik loguje się do aplikacji
   ↓
2. Ląduje na Dashboard'zie
   ↓
3. Widzi kart "Generuj nowy skład" → klikuje
   ↓
4. Trafia na stronę Generowania Składu (/lineups/generate)
   ↓
5. Wybiera kolejkę z dropdown'u
   ↓
6. Klika "Generuj skład" → wysyła request do API
   ↓
7. Czeka na wynik (loading indicator)
   ↓
8. Otrzymuje wygenerowany skład na stronie Podglądu
   ↓
9. Przegląda skład, ewentualnie zmienia zawodników
   ↓
10. System waliduje zmiany w real-time
   ↓
11. Zatwierdza i zapisuje skład (modal z nazwą, opcją ustawienia jako aktywny)
   ↓
12. Trafia na stronę Szczegółów Składu
   ↓
13. Może wrócić do Dashboard'u lub Historia
```

### 3.2 Przeglądanie zawodników

```
1. Z Dashboard'u lub Menu → klikuje "Zawodnicy"
   ↓
2. Ląduje na stronie Listy Zawodników (/players)
   ↓
3. Może filtrować: pozycja, drużyna, cena, forma, status zdrowia
   ↓
4. Wyszukuje po imieniu/nazwisku (debounced search)
   ↓
5. Sortuje po: nazwa, cena, forma, punkty
   ↓
6. Klika na zawodnika → Szczegóły Zawodnika
   ↓
7. Widzi historię, statystyki, nadchodzące mecze
   ↓
8. Może wrócić do listy lub Dashboard'u
```

### 3.3 Przegląd meczów i terminacji

```
1. Z Menu → klikuje "Kolejki" lub "Mecze"
   ↓
2. Lista Kolejek lub Lista Meczów
   ↓
3. Filtruje i sortuje
   ↓
4. Klika na konkretną kolejkę/mecz → Szczegóły
   ↓
5. Może generować skład dla tej kolejki (jeśli jeszcze nie rozegrana)
   ↓
6. Wraca do listy lub Dashboard'u
```

### 3.4 Zarządzanie składami

```
1. Z Menu → klikuje "Składy"
   ↓
2. Lista Składów (filtry: aktywne, archiwum, range kolejek)
   ↓
3. Klika na skład → Szczegóły
   ↓
4. Może edytować, duplikować, eksportować, usunąć
   ↓
5. Historia - analiza poprzednich generacji
   ↓
6. Porównanie AI vs użytkownika
```

### 3.5 Import danych (admin)

```
1. Z Menu (admin only) → klikuje "Import"
   ↓
2. Ląduje na stronie Importu (/admin/import)
   ↓
3. Drag-and-drop lub wybiera plik CSV
   ↓
4. Wybiera kolejkę
   ↓
5. Przegląda podgląd danych
   ↓
6. Klika "Importuj" → wysyła request do API
   ↓
7. Czeka na wynik
   ↓
8. Otrzymuje raport (powodzenie, błędy, ostrzeżenia)
   ↓
9. Może powtórzyć lub wrócić
```

---

## 4. Układ i struktura nawigacji

### 4.1 Główne sekcje nawigacji

**Struktura menu (hierarchia):**

```
Dashboard
├── Zespoły
│   ├── Lista Zespołów
│   └── Szczegóły Zespołu
├── Zawodnicy
│   ├── Lista Zawodników
│   └── Szczegóły Zawodnika
├── Kolejki
│   ├── Lista Kolejek
│   └── Szczegóły Kolejki
├── Mecze
│   ├── Lista Meczów
│   └── Szczegóły Meczu
├── Składy
│   ├── Generowanie Składu
│   ├── Podgląd Składu
│   ├── Lista Składów
│   └── Szczegóły Składu
├── Historia
│   ├── Historia Składów
│   └── Analiza Wyników
└── Import (admin only)
    └── Import CSV
```

### 4.2 Nawigacja global'na

- **Top Navigation Bar:**
  - Logo aplikacji (klikalne → Dashboard)
  - Hamburger menu (mobile)
  - Avatar użytkownika (right)
  - Powiadomienia (right)

- **Side Navigation (desktop):**
  - Kolumna z menu
  - Ikony + tekst
  - Aktywny link highlighted
  - Responsive: hide na mobile (→ hamburger menu)

- **Mobile Navigation:**
  - Hamburger menu
  - Drawer menu z pełnym menu
  - Bottom tab navigation (opcjonalnie)

### 4.3 Breadcrumb nawigacja

Każda podstrona (nie homepage) wyświetla breadcrumb:

```
Dashboard > Zawodnicy > Robert Lewandowski
Dashboard > Składy > Kolejka 15 - Mocna forma
Dashboard > Historia > Analiza
```

### 4.4 Nawigacja między widokami

- **Back button:** Na każdej podstronie (jeśli nie home)
- **Quick actions:** Z list do szczegółów, z szczegółów do edycji
- **Szybkie przejścia:** Z Dashboard'u do głównych funkcji
- **Modal flows:** Import CSV, duplikacja składu w modals

---

## 5. Kluczowe komponenty

### 5.1 Komponenty wspólne (Shared)

- **`Button`** - przyciski z wariantami (primary, secondary, danger, loading)
- **`Card`** - card komponenty z header/body/footer
- **`Table`** - reusable tabela z sortowaniem, filtrowaniem, paginacją
- **`Modal`** - modal dialogi z header/body/footer
- **`Toast`** - notyfikacje (success, error, warning, info)
- **`Dropdown`** - select komponenty
- **`Input`** - text inputy
- **`Checkbox`** - checkboxy
- **`RadioButton`** - radio buttons
- **`Spinner`** - loading indicator
- **`ProgressBar`** - progress bar
- **`Badge`** - badge'i dla statusów
- **`Icon`** - ikony (opcjonalnie z biblioteki)
- **`Tabs`** - tab komponenty
- **`Accordion`** - accordion komponenty
- **`Pagination`** - stronicowanie

### 5.2 Komponenty specjalistyczne

- **`DashboardCard`** - card na dashboard'zie
- **`PlayerFormBar`** - wizualna reprezentacja formy
- **`HealthStatusBadge`** - badge statusu zdrowia
- **`FormationGrid`** - wizualna reprezentacja formacji
- **`MatchCard`** - card meczu
- **`PlayerCard`** - card zawodnika
- **`LineupFormationGrid`** - grid formacji składu
- **`BudgetBar`** - pasek budżetu
- **`ValidationStatus`** - status walidacji
- **`TeamCrest`** - logo drużyny
- **`GameweekBadge`** - badge kolejki
- **`TrendChart`** - wykres trendu
- **`DragDropUploader`** - drag-and-drop uploader

### 5.3 Komponenty logiki biznesowej

- **`GameweekSelector`** - selektor kolejki
- **`PlayerSwapModal`** - modal do zamiany zawodnika
- **`LineupValidator`** - walidacja składu
- **`FilterPanel`** - panel filtrowania
- **`SearchBar`** - wyszukiwanie
- **`SortingControls`** - kontrola sortowania
- **`ExportButton`** - eksport funkcjonalności

### 5.4 Strategia zarządzania stanem

**Blazor Server + SignalR:**
- State management przez Dependency Injection
- Services do obsługi business logic
- @key directive do caching komponentów
- Optimistic updates dla lepszego UX
- MemoryCache na client-side
- Debouncing dla wyszukiwania/filtrowania
- Intelligent prefetching

**Implementacja:**
```csharp
// AppState.cs - state management
public class AppState
{
    public User CurrentUser { get; set; }
    public Gameweek CurrentGameweek { get; set; }
    public Lineup ActiveLineup { get; set; }
    // ... itd
    
    public event Action OnStateChange;
    
    public void NotifyStateChanged() => OnStateChange?.Invoke();
}

// Services (DI) do fetching i managementu danych
public class PlayerService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    
    public async Task<List<PlayerDto>> GetPlayersAsync(/* params */)
    {
        // cache logic, API calls, etc.
    }
}
```

---

## 6. Responsywność i breakpointy

**MudBlazor Grid System:**

- **Desktop (1200px+):** 12-kolumnowy grid
- **Tablet (768-1199px):** 8-kolumnowy grid
- **Mobile (< 768px):** 4-kolumnowy grid

**Przykłady responsywności:**

- **Tabele:** 12 kolumn desktop → 8 kolumn tablet → karty mobile
- **Dashboard karty:** 3x2 grid desktop → 2x3 tablet → 1 kolumna mobile
- **Filtry:** Side panel desktop → Top panel tablet → Modal mobile
- **Menu:** Side nav desktop/tablet → Hamburger menu mobile

**Breakpointy MudBlazor:**
```csharp
// Breakpoints
xs: 0px
sm: 600px
md: 960px
lg: 1280px (desktop)
xl: 1920px
xxl: 2560px
```

---

## 7. Strategia caching i optymalizacji

### 7.1 Client-side caching (MemoryCache)
- **Teams** (TTL: 24h) - rzadko się zmienia
- **Bonuses** (TTL: 24h) - statyczne
- **Gameweeks** (TTL: 4h) - zmienia się co 7 dni
- **Players** (TTL: 2h) - zmienia się po imporcie statystyk
- **Player Stats** (TTL: 1h) - zmienia się często

### 7.2 @key directive
```razor
@foreach (var player in players)
{
    <PlayerCard @key="player.Id" Player="player" />
}
```

### 7.3 Prefetching
- Przy hovering na zespół → prefetch zawodników
- Przy hovering na zawodnika → prefetch historii
- Przy otwieraniu listy → prefetch szczegółów dla top 5

### 7.4 Lazy loading
- Infinite scroll dla list zawodników
- Lazy load obrazów (logotypy)
- Lazy load tabel danych

### 7.5 Debouncing
- Wyszukiwanie: 300ms debounce
- Filtry: 500ms debounce
- Auto-save: 1s debounce

---

## 8. Obsługa błędów i notyfikacji

### 8.1 Toast notyfikacje
- **Success:** Zielony toast (2s)
- **Error:** Czerwony toast (5s)
- **Warning:** Żółty toast (3s)
- **Info:** Niebieski toast (3s)

### 8.2 Field-level validation
- Real-time walidacja w formularzach
- Podkreślenie błędnych pól
- Error message poniżej pola
- Disable submit button dopóki nie valid

### 8.3 Global error handler
- Middleware do obsługi nieoczekiwanych błędów
- Fallback error page
- Logging do konsoli

---

## 9. Przystosowanie do przyszłych ról

Struktura UI przygotowana do przyszłego podziału na role:

```csharp
// Future: Role-based UI rendering
@if (user.Role == UserRole.Admin)
{
    <AdminMenu />
}
else if (user.Role == UserRole.Analyst)
{
    <AnalystMenu />
}
else
{
    <PlayerMenu />
}
```

- Menu i dostęp do funkcji będzie filtrowany po rolach
- Admin sekcje (Import, Edycja Zespołów/Meczów) już przygotowane
- User flow bez zmian w MVP
- Łatwa rozszerzalność

---

## 10. Technologia i implementacja

### 10.1 Stack
- **Frontend:** Blazor Server
- **UI Library:** MudBlazor
- **CSS:** MudBlazor built-in + custom CSS
- **State Management:** Dependency Injection + AppState
- **Caching:** MemoryCache

### 10.2 Struktura folderów
```
src/FantasyCoachAI.Web/
├── Components/
│   ├── Shared/
│   │   ├── MainLayout.razor
│   │   ├── NavigationMenu.razor
│   │   ├── TopBar.razor
│   │   └── ... (wspólne komponenty)
│   ├── Pages/
│   │   ├── Dashboard.razor
│   │   ├── Teams/
│   │   │   ├── TeamsList.razor
│   │   │   └── TeamDetail.razor
│   │   ├── Players/
│   │   │   ├── PlayersList.razor
│   │   │   └── PlayerDetail.razor
│   │   ├── Gameweeks/
│   │   │   ├── GameweeksList.razor
│   │   │   └── GameweekDetail.razor
│   │   ├── Matches/
│   │   │   ├── MatchesList.razor
│   │   │   └── MatchDetail.razor
│   │   ├── Lineups/
│   │   │   ├── GenerateLineup.razor
│   │   │   ├── LineupPreview.razor
│   │   │   ├── LineupsList.razor
│   │   │   └── LineupDetail.razor
│   │   ├── History/
│   │   │   ├── LineupHistory.razor
│   │   │   └── PerformanceAnalysis.razor
│   │   └── Admin/
│   │       └── Import.razor
│   └── Forms/
│       ├── LineupForm.razor
│       ├── MatchEditForm.razor
│       └── ... (formularze)
├── Services/
│   ├── ApiClient.cs
│   ├── PlayerService.cs
│   ├── LineupService.cs
│   ├── GameweekService.cs
│   ├── MatchService.cs
│   ├── TeamService.cs
│   ├── AppState.cs
│   └── CacheService.cs
├── Models/
│   ├── (DTOs z Application layer)
├── wwwroot/
│   ├── css/
│   │   ├── app.css (główne style)
│   │   └── responsive.css
│   └── js/
│       └── app.js (interop jeśli potrzebny)
└── Program.cs
```

### 10.3 Konwencje kodowania
- Komponenty .razor z code-behind (.razor.cs)
- PascalCase dla nazw komponentów
- camelCase dla nazw zmiennych
- Komponenty reusable w folderze Shared/Components
- Strony w folderze Pages

### 10.4 Best practices
- Lazy loading dla dużych list
- Caching dla statycznych danych
- Optimistic updates
- Debouncing dla search/filters
- Error handling na każdym API call
- Toast notifikacje dla feedbacku
- Responsive design mobile-first
- Accessibility considerations (structure ready for future)
- Security: JWT token handling, authorization checks
