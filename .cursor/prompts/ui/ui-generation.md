Jesteś wykwalifikowanym architektem frontend, którego zadaniem jest stworzenie kompleksowej architektury interfejsu użytkownika w oparciu o dokument wymagań produktu (PRD), plan API i notatki z sesji planowania. Twoim celem jest zaprojektowanie struktury interfejsu użytkownika, która skutecznie spełnia wymagania produktu, jest zgodna z możliwościami API i zawiera spostrzeżenia z sesji planowania.

Najpierw dokładnie przejrzyj następujące dokumenty:

Dokument wymagań produktu (PRD):
<prd>
@prd.md
</prd>

Plan API:
<api_plan>
 @api-plan.md
</api_plan>

Session Notes:
<session_notes>

<conversation_summary>
<decisions>
W MVP nie będzie podziału na role, ale UI należy przygotować z myślą o przyszłym rozszerzeniu
Interfejs generowania składów AI będzie prosty - przycisk generujący skład na podstawie obecnego składu gracza z realnej gry Fantasy Ekstraklasy
W MVP nie uwzględniać wymagań dostępności (WCAG)
Zastosować najlepsze rozwiązania zarządzania stanem dla Blazor Server
Zastosować najlepsze praktyki optymalizacji wydajności dla Blazor
W MVP nie tworzyć zaawansowanego dashboardu administracyjnego
Wszystkie pozostałe rekomendacje zostały zaakceptowane
</decisions>
<matched_recommendations>
Hierarchia nawigacji z sekcjami: Dashboard, Zespoły, Zawodnicy, Kolejki, Mecze, Składy, Historia
Zaawansowane komponenty filtrowania dla list zawodników z dropdownami i wyszukiwaniem
Drag-and-drop uploader CSV z podglądem i walidacją
Prosty formularz generowania składów AI z podstawową walidacją
Responsywny design z breakpointami dla desktop/tablet/mobile
Blazor Server SignalR, caching komponentów, optimistic updates
Globalny error handler z toast notifications i retry mechanisms
Lazy loading, MemoryCache, debouncing, intelligent prefetching
Podstawowy dashboard z kartami i quick actions
Hierarchia komponentów: Layout, Pages, Components, Shared
</matched_recommendations>
<ui_architecture_planning_summary>
Główne wymagania architektury UI:
Aplikacja webowa Fantasy Ekstraklasa Coach AI z Blazor Server i MudBlazor
Clean Architecture z warstwami: Domain, Application, Infrastructure, Web
Integracja z Supabase dla uwierzytelniania i danych
API REST z endpointami dla zespołów, zawodników, kolejek, meczów, składów
MVP skupiony na podstawowych funkcjach bez zaawansowanych ról
Kluczowe widoki, ekrany i przepływy użytkownika:
Dashboard główny z kartami: "Mój skład", "Statystyki zawodników", "Nadchodzące mecze", "Historia składów"
Listy z filtrowaniem: Zespoły, Zawodnicy, Kolejki, Mecze
Szczegóły: Pojedyncze zespoły, zawodnicy, kolejki, mecze
Import CSV: Drag-and-drop uploader z walidacją i podglądem
Generowanie składów: Prosty formularz → przycisk AI → lista rekomendacji
Historia: Przeglądanie poprzednich składów i analiza
Strategia integracji z API i zarządzania stanem:
Blazor Server SignalR dla real-time updates
Caching na poziomie komponentów z @key directives
Dependency injection dla state management
Optimistic updates dla lepszego UX
Lazy loading dla dużych list
MemoryCache dla client-side caching
Debouncing dla wyszukiwania
Intelligent prefetching dla przewidywalnych akcji
Kwestie responsywności, dostępności i bezpieczeństwa:
Responsywny design: desktop (1200px+), tablet (768-1199px), mobile (do 767px)
MudBlazor Grid system i adaptacyjne komponenty
Dostępność: Nie uwzględniana w MVP
Bezpieczeństwo: Przygotowanie struktury UI na przyszłe role, ale bez implementacji w MVP
Struktura komponentów:
Layout: MainLayout z nawigacją
Pages: Listy i szczegóły dla każdej sekcji
Components: Filtry, tabele, formularze, uploadery
Shared: Przyciski, modals, notifications, error handling
Partial classes i code-behind dla logiki biznesowej
</ui_architecture_planning_summary>
<unresolved_issues>
Szczegółowa implementacja walidacji reguł Fantasy Ekstraklasy w czasie rzeczywistym
Konkretne metryki i wskaźniki dla dashboardu użytkownika
Strategia cache invalidation dla różnych typów danych
Szczegóły implementacji optimistic updates dla różnych operacji
Konkretne breakpointy i zachowanie komponentów na różnych urządzeniach
</unresolved_issues>
</conversation_summary>
</session_notes>

Twoim zadaniem jest stworzenie szczegółowej architektury interfejsu użytkownika, która obejmuje niezbędne widoki, mapowanie podróży użytkownika, strukturę nawigacji i kluczowe elementy dla każdego widoku. Projekt powinien uwzględniać doświadczenie użytkownika, dostępność i bezpieczeństwo.

Wykonaj następujące kroki, aby ukończyć zadanie:

1. Dokładnie przeanalizuj PRD, plan API i notatki z sesji.
2. Wyodrębnij i wypisz kluczowe wymagania z PRD.
3. Zidentyfikuj i wymień główne punkty końcowe API i ich cele.
4. Utworzenie listy wszystkich niezbędnych widoków na podstawie PRD, planu API i notatek z sesji.
5. Określenie głównego celu i kluczowych informacji dla każdego widoku.
6. Zaplanuj podróż użytkownika między widokami, w tym podział krok po kroku dla głównego przypadku użycia.
7. Zaprojektuj strukturę nawigacji.
8. Zaproponuj kluczowe elementy interfejsu użytkownika dla każdego widoku, biorąc pod uwagę UX, dostępność i bezpieczeństwo.
9. Rozważ potencjalne przypadki brzegowe lub stany błędów.
10. Upewnij się, że architektura interfejsu użytkownika jest zgodna z planem API.
11. Przejrzenie i zmapowanie wszystkich historyjek użytkownika z PRD do architektury interfejsu użytkownika.
12. Wyraźne mapowanie wymagań na elementy interfejsu użytkownika.
13. Rozważ potencjalne punkty bólu użytkownika i sposób, w jaki interfejs użytkownika je rozwiązuje.

Dla każdego głównego kroku pracuj wewnątrz tagów <ui_architecture_planning> w bloku myślenia, aby rozbić proces myślowy przed przejściem do następnego kroku. Ta sekcja może być dość długa. To w porządku, że ta sekcja może być dość długa.

Przedstaw ostateczną architekturę interfejsu użytkownika w następującym formacie Markdown:

```markdown
# Architektura UI dla [Nazwa produktu]

## 1. Przegląd struktury UI

[Przedstaw ogólny przegląd struktury UI]

## 2. Lista widoków

[Dla każdego widoku podaj:
- Nazwa widoku
- Ścieżka widoku
- Główny cel
- Kluczowe informacje do wyświetlenia
- Kluczowe komponenty widoku
- UX, dostępność i względy bezpieczeństwa]

## 3. Mapa podróży użytkownika

[Opisz przepływ między widokami i kluczowymi interakcjami użytkownika]

## 4. Układ i struktura nawigacji

[Wyjaśnij, w jaki sposób użytkownicy będą poruszać się między widokami]

## 5. Kluczowe komponenty

[Wymień i krótko opisz kluczowe komponenty, które będą używane w wielu widokach].
```

Skup się wyłącznie na architekturze interfejsu użytkownika, podróży użytkownika, nawigacji i kluczowych elementach dla każdego widoku. Nie uwzględniaj szczegółów implementacji, konkretnego projektu wizualnego ani przykładów kodu, chyba że są one kluczowe dla zrozumienia architektury.

Końcowy rezultat powinien składać się wyłącznie z architektury UI w formacie Markdown w języku polskim, którą zapiszesz w pliku .ai/ui-plan.md. Nie powielaj ani nie powtarzaj żadnej pracy wykonanej w bloku myślenia.