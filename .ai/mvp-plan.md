### Główny problem
Manualne tworzenie składów w grze https://fantasy.ekstraklasa.org/ jest czasochłonne i trudne ze względu na wiele kryteriów (forma, cena, terminarz, limity zawodników z drużyn itp.), które trzeba uwzględnić przy wyborze składu na daną kolejkę.

### Cel MVP

Umożliwić użytkownikom szybkie i oparte na danych generowanie składów Fantasy Ekstraklasy, integrując gotowy algorytm rekomendujący najlepsze ustawienie.

### Najmniejszy zestaw funkcjonalności
- Przeglądanie listy zespołów piłkarskich
- Przeglądanie listy piłkarzy
- Przeglądanie terminarza gier
- Przegladanie, edycja i aktualizacja meczy
- Import statystyk (minuty, bramki, asysty, punkty, cena itd.)
- Prosty system kont użytkowników 
- Integracja z modelem LLM generującym rekomendowany skład (np. GPT-4 lub inny model AI dostępny przez REST API)

### Co NIE wchodzi w zakres MVP
- Własny, zaawansowany algorytm
- Import wielu formatów (PDF, DOCX, itp.)
- Współdzielenie zestawów fiszek między użytkownikami
- Aplikacje mobilne (na początek tylko web)

### Kryteria sukcesu
- Składy generowane przez LLM są zgodne z zasadami Fantasy Ekstraklasy (budżet, liczba zawodników, limity klubowe)