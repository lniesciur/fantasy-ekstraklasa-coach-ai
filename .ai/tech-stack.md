Frontend - Blazor Server z komponentami interaktywnymi:
- Blazor Server - framework internetowy stworzony przez Microsoft, który pozwala tworzyć interaktywne aplikacje internetowe przy użyciu języka C# i .NET, bez potrzeby używania JavaScript
- MudBlazor - biblioteka komponentów UI dla Blazor
- Swagger/OpenAPI - dokumentacja API z automatycznym generowaniem

Backend - .NET 8 z Clean Architecture:
- .NET 8 - framework aplikacji
- Clean Architecture - podział na warstwy: Domain, Application, Infrastructure, Web
- Supabase - baza danych PostgreSQL z SDK
- FluentValidation - walidacja danych wejściowych
- CsvHelper - import danych z plików CSV
- Repository Pattern - abstrakcja dostępu do danych
- Dependency Injection - zarządzanie zależnościami

Testowanie:
- xUnit - framework testowy
- FluentAssertions - czytelne asercje
- Moq - mockowanie zależności
- Coverlet - pokrycie kodu testami

Narzędzia pomocnicze:
- FantasyCoachAI.DataParser - narzędzie do parsowania danych HTML z embedded resources
- Regex - parsowanie danych ze stron HTML
- SQL generation - automatyczne generowanie skryptów SQL

Monitoring i DevOps:
- Health checks - monitoring stanu aplikacji i połączenia z bazą danych
- Structured logging - logowanie z kontekstem
- CORS - obsługa cross-origin requests
- Auto-validation filters - automatyczna walidacja DTOs

AI - Komunikacja z modelami przez usługę Openrouter.ai:
- Dostęp do szerokiej gamy modeli (OpenAI, Anthropic, Google i wiele innych), które pozwolą nam znaleźć rozwiązanie zapewniające wysoką efektywność i niskie koszta
- Pozwala na ustawianie limitów finansowych na klucze API

CI/CD i Hosting:
- Github Actions do tworzenia pipeline'ów CI/CD
- DigitalOcean do hostowania aplikacji za pośrednictwem obrazu docker